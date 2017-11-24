using Microsoft.HockeyApp;
using OpenFoodFactsContract;
using ProductsControllerShared;
using ReadTheIngredientsUWP.Controls;
using ReadTheIngredientsUWP.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.PointOfService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using ZXing.Mobile;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ReadTheIngredientsUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ProductsControllerShared.ProductsController ProductsController = null;
        ScannerController ScannerController = null;
        CameraController CameraController = null;
        MobileBarcodeScanner scanner;
        private bool UseNativeCamera = false;
        public MainPage()
        {
            this.InitializeComponent();
            HockeyClient.Current.TrackTrace("Starting mainpage on platform: " + AnalyticsInfo.VersionInfo.DeviceFamily);

            DetectPlatform();

            scanner = new MobileBarcodeScanner(this.Dispatcher);
            scanner.Dispatcher = this.Dispatcher;

            if (UseNativeCamera)
            {
                ScannerController = new ScannerController();
                ScannerController.StartWatcher(this.Dispatcher);
                CameraController = new CameraController(ScannerController);
                CameraController.OnFixedFocusCameraDetected += CameraController_OnFixedFocusCameraDetected;
            }

            var app = App.Current as App;

            ProductsController = app.ProductsController;
            ProductsController.PropertyChanged += ProductsController_PropertyChanged;
            this.DataContext = ProductsController;

            if (UseNativeCamera)
            {
                this.ProductCameraFeedControl.StartPressed += ProductCameraFeedControl_StartPressed;
                this.ProductCameraFeedControl.StopPressed += ProductCameraFeedControl_StopPressed;

                this.ProductCameraFeedControl.DataContext = CameraController;
            }
            this.ProductSearchControl.DataContext = ProductsController;
            this.ProductSearchControl.IsEnabledChanged += ProductSearchControl_IsEnabledChanged;

            this.ProductSearchControl.StartSearch += ProductSearchControl_StartSearch;
            this.IngredientsCtrl.IngredientDetailRequested += IngredientsCtrl_IngredientDetailRequested;
            this.ProductDetailsCtrl.IngredientDetailRequested += ProductDetailsCtrl_IngredientDetailRequested; ;
            this.IngredientDetailCtrl.Tapped += IngredientDetailCtrl_Tapped;

            this.UnknownProductCtrl.OkPressed += UnknownProductCtrl_OkPressed;
            this.UnknownProductCtrl.CancelPressed += UnknownProductCtrl_CancelPressed;
            //ResetTheScenarioState();
        }

        private void UnknownProductCtrl_CancelPressed(object sender, EventArgs e)
        {
            this.ProductViewer.Opacity = 1;
            var ctrl = sender as UnknownProductControl;
            ctrl.Visibility = Visibility.Collapsed;
        }

        private void UnknownProductCtrl_OkPressed(object sender, EventArgs e)
        {
            this.ProductViewer.Opacity = 1;
            var ctrl = sender as UnknownProductControl;
            // todo: get commands and post them
            ctrl.Visibility = Visibility.Collapsed;
        }

        private void ProductsController_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Product")
            {
                if (!string.IsNullOrEmpty(ProductsController.Product.BarcodePath))
                {
                    this.UnknownProductCtrl.Visibility = Visibility.Visible;
                }
            }
        }

        // todo: make proper control event?
        private void IngredientDetailCtrl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.ProductViewer.Opacity = 1;
        }

        private void ProductDetailsCtrl_IngredientDetailRequested(object sender, PivotedProductDetails.IngredientDetailRequestEventArgs e)
        {
            IngredientDetailCtrl.DataContext = e.Ingredient;
            ProductsController.GetDetails(e.Ingredient);
            this.ProductViewer.Opacity = 0.1;
            IngredientDetailCtrl.Visibility = Visibility.Visible;
        }

        private void IngredientsCtrl_IngredientDetailRequested(object sender, Controls.ProductIngredientsListControl.IngredientDetailRequestEventArgs e)
        {
            IngredientDetailCtrl.DataContext = e.Ingredient;
            ProductsController.GetDetails(e.Ingredient);
            this.ProductViewer.Opacity = 0.1;
            IngredientDetailCtrl.Visibility = Visibility.Visible;
        }

        private void IngredientDetailControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var ctrl = sender as IngredientDetailControl;
            ctrl.Visibility = Visibility.Collapsed;
        }

        private void DetectPlatform()
        {
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Mobile":
                    UseNativeCamera = false;
                    break;
                case "Windows.Desktop":
                    if (UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse)
                    { UseNativeCamera = true; }
                    else
                    {
                        UseNativeCamera = false;
                    }
                    break;
                case "Windows.Universal":
                    //return DeviceFormFactorType.IoT;
                    break;
                case "Windows.Team":
                    break;
                default:
                    break;
                    //return DeviceFormFactorType.Other;
            }
        }

        private void ProductSearchControl_StartSearch(object sender, Controls.SearchControl.SearchEventArgs e)
        {
            GetProductNameByBarCode(e.SearchText);
        }

        private async void CameraController_OnFixedFocusCameraDetected(object sender, EventArgs e)
        {
            HockeyClient.Current.TrackTrace("FixedFocus camera detected");

            await StartFixedFocusScanner();
        }

        private void ProductSearchControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                this.ProductViewer.Opacity = 1;
                this.ProductSearchControl.Visibility = Visibility.Collapsed;
            }
        }

        private void ProductCameraFeedControl_StopPressed(object sender, EventArgs e)
        {
            StopScan();
        }

        private async void StopScan()
        {
            if (UseNativeCamera)
            {
                await CameraController.StopPreviewAsync();
                ScannerController.Stop();
                ScannerController.DataReceived -= ScannerController_DataReceived;
                this.ResetTheScenarioState();
                ScannerController.StartWatcher(Dispatcher);
            }
        }

        private void ProductCameraFeedControl_StartPressed(object sender, EventArgs e)
        {
            StartScan();
        }

        private async void StartScan()
        {
            if (UseNativeCamera)
            {
                ScannerController.StopWatcher();
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    await ScannerController.CreateScanner(Dispatcher);

                    ScannerController.DataReceived += ScannerController_DataReceived;
                });

            }
        }

        private void ScannerController_DataReceived(ClaimedBarcodeScanner sender, BarcodeScannerDataReceivedEventArgs args)
        {
            claimedScanner_DataReceived(sender, args);
        }


        string GetDataLabelString(IBuffer data, uint scanDataType)
        {
            string result = null;
            // Only certain data types contain encoded text.
            //   To keep this simple, we'll just decode a few of them.
            if (data == null)
            {
                result = "No data";
            }
            else
            {
                switch (BarcodeSymbologies.GetName(scanDataType))
                {
                    case "Upca":
                    case "UpcaAdd2":
                    case "UpcaAdd5":
                    case "Upce":
                    case "UpceAdd2":
                    case "UpceAdd5":
                    case "Ean8":
                    case "Ean13":
                    // The UPC, EAN8, EAN13, and 2 of 5 families encode the digits 0..9
                    // which are then sent to the app in a UTF8 string (like "01234")
                    // This is not an exhaustive list of symbologies that can be converted to a string
                    case "Qr":
                        // Need to distinguish the scan data among Numeric, Alphanumeric, Binary/Byte, and Kanji/Kana.
                        // But as current barcode scanner could not distinguish the data is in Binary/Byte format or in rest 3 formats.
                        // So to read the decoded data as string.
                        result = string.Format("{0}", System.Text.Encoding.UTF8.GetString(data.ToArray()));
                        break;
                    default:
                        // Some other unsupport symbologies and just leave it as binary data
                        //result = string.Format("Decoded data unavailable. Raw label data: {0}", GetDataString(data));
                        break;
                }
            }

            return result;
        }
        private string _previousDetectedBarcode;
        /// <summary>
        /// Event handler for the DataReceived event fired when a barcode is scanned by the barcode scanner 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"> Contains the BarcodeScannerReport which contains the data obtained in the scan</param>
        async void claimedScanner_DataReceived(ClaimedBarcodeScanner sender, BarcodeScannerDataReceivedEventArgs args)
        {
            var detectedBarcode = GetDataLabelString(args.Report.ScanDataLabel, args.Report.ScanDataType);
            HandleNewBarcodeDetected(detectedBarcode);
        }

        public OpenFoodFactsResultDTO GetProductNameByBarCode(string barcode)
        {
            var facade = new OpenFoodFactsFacade.OpenFoodFacts();
            var result = facade.GetByBarCode(barcode);
            if (result.Status == 1)
            {
                ProductsController.SetProduct(result.Product, barcode);
                ProductsController.ParseIngredients();
                this.ProductSearchControl.IsEnabled = false;
            }
            return result;
        }

        private async void HandleNewBarcodeDetected(string detectedBarcode)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                if (_previousDetectedBarcode != detectedBarcode)
                {
                    if (ProductsController != null)
                    {
                        if (GetProductNameByBarCode(detectedBarcode).Status == 1)
                        {
                            this.ProductViewer.Opacity = 1;
                            this.UnknownProductCtrl.Visibility = Visibility.Collapsed;
                            if (this.ProductCameraFeedControl.Visibility == Visibility.Visible)
                            {
                                this.ProductCameraFeedControl.Visibility = Visibility.Collapsed;
                            }
                            Dictionary<string, string> properties = new Dictionary<string, string>();
                            properties.Add("Barcode", detectedBarcode);

                            HockeyClient.Current.TrackTrace("Barcode detected", properties);
                        }
                        else
                        {
                            // todo: pass type for more accurate representation
                            // todo: move to separate function
                            if (this.UnknownProductCtrl.Visibility != Visibility.Visible)
                            {
                                //HandleUnknownBarcode(detectedBarcode);
                                Dictionary<string, string> properties = new Dictionary<string, string>();
                                properties.Add("Barcode", detectedBarcode);

                                HockeyClient.Current.TrackTrace("Unknown Barcode detected", properties);
                                // todo: undetected barcode, link to openfoodfacts, then later offer to add from the app
                                var folder = Windows.Storage.ApplicationData.Current.LocalCacheFolder;
                                try
                                {
                                    this.UnknownProductCtrl.Reset();
                                    var pictureFile = await folder.CreateFileAsync($"{detectedBarcode}.bmp", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                                    var path = await GenerateBarcodeImage(detectedBarcode, pictureFile);
                                    ProductsController.SetNewProduct(detectedBarcode, path);
                                    this.UnknownProductCtrl.Product.Barcode = detectedBarcode;
                                    this.UnknownProductCtrl.SetBarcodeImagePath(path);
                                    //this.UnknownProductCtrl.Product.BarcodePath = path;
//                                    var barcodeImage = new BitmapImage(new Uri(path));
                                    //this.UnknownProductCtrl.Product.ProductImage = path;
                                    //this.UnknownProductCtrl.Product.BarcodeImage = barcodeImage;
                                    //this.UnknownProductCtrl.Product.NutrientImage = path;
                                    //this.UnknownProductCtrl.Product.IngredientImage = path;

                                }
                                catch (Exception ex)
                                {
                                    Debug.Write(ex.Message);
                                }
                                this.ProductViewer.Opacity = 0.1;
                                this.UnknownProductCtrl.Visibility = Visibility.Visible;
                            }
                  
                        }
                        _previousDetectedBarcode = detectedBarcode;
                    }
                }
            });
        }


        private async System.Threading.Tasks.Task<string> GenerateBarcodeImage(string detectedBarcode, StorageFile pictureFile)
        {
            var barcodeWriter = new BarcodeWriter();

            barcodeWriter.Format = detectedBarcode.Length == 8 ? ZXing.BarcodeFormat.EAN_8 : ZXing.BarcodeFormat.EAN_13;

            var result = barcodeWriter.Write(detectedBarcode);
            var path = "";
            try
            {

                path = pictureFile.Path;
                using (IRandomAccessStream stream = await pictureFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                    // Get pixels of the WriteableBitmap object 
                    Stream pixelStream = result.PixelBuffer.AsStream();
                    byte[] pixels = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                    // Save the image file with jpg extension 
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)result.PixelWidth, (uint)result.PixelHeight, 96.0, 96.0, pixels);
                    await encoder.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return path;
        }

        /// <summary>
        /// Reset the Scenario state
        /// </summary>
        private async void ResetTheScenarioState()
        {
            if (UseNativeCamera)
            {
                ScannerController.DataReceived -= ScannerController_DataReceived;
                ScannerController.Reset();
                await CameraController.CleanupCameraAsync();

                this.ProductCameraFeedControl.Reset();
            }
        }

        private async void ShowCamera(object sender, RoutedEventArgs e)
        {
            if (UseNativeCamera)
            {
                HockeyClient.Current.TrackTrace("Attempting to show native camera");
                if (this.ProductCameraFeedControl.Visibility == Visibility.Collapsed)
                {
                    // start the feed if necessary
                    if (this.CameraController.MediaCapture == null)
                    {
                        try
                        {
                            await this.ProductCameraFeedControl.Start();
                            if (!this.CameraController.IsFixedFocus)
                            {
                                this.ProductCameraFeedControl.Visibility = Visibility.Visible;
                            }
                        }
                        catch (Exception)
                        {
                            this.ProductCameraFeedControl.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        // camera should still be running
                        this.ProductCameraFeedControl.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    // by default, we leave everything running
                    this.ProductCameraFeedControl.Visibility = Visibility.Collapsed;
                }
            }
            //if (CameraController.IsFixedFocus || this.ProductCameraFeedControl.Visibility == Visibility.Collapsed || CameraController.MediaCapture == null)
            //{
            //    this.ProductCameraFeedControl.Visibility = Visibility.Collapsed;
            //    UseNativeCamera = false;
            //};

            if (!UseNativeCamera)
            {
                await StartFixedFocusScanner();
            }
        }

        private async System.Threading.Tasks.Task StartFixedFocusScanner()
        {
            HockeyClient.Current.TrackTrace("Attempting to show fixed focus camera with ZXing");

            // fallback to the ZXing detection, which works better with non-focus cameras
            // this control creats its own page
            UseNativeCamera = false;
            //Tell our scanner to use the default overlay
            scanner.UseCustomOverlay = false;
            // todo: get proper translations here 
            //scanner.TopText = "Hold camera up to barcode";
            //scanner.BottomText = "Camera will automatically scan barcode\r\n\r\nPress the 'Back' button to Cancel";

            //Start scanning
            await scanner.Scan().ContinueWith(t =>
            {
                if (t.Result != null)
                    HandleScanResult(t.Result);
            });
        }

        async void HandleScanResult(ZXing.Result result)
        {
            HockeyClient.Current.TrackTrace("Received scanresult from ZXing");

            string msg = "";

            if (result != null && !string.IsNullOrEmpty(result.Text))
                HandleNewBarcodeDetected(result.Text);
            //            else
            // todo: report something to user msg = "Scanning Canceled!";

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //ResetTheScenarioState();
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Invoked when this page is no longer displayed.
        /// </summary>
        /// <param name="e">Event data that describes how this page was exited.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ResetTheScenarioState();
            base.OnNavigatedFrom(e);
            if (UseNativeCamera)
            {
                ScannerController.StopWatcher();
            }
        }

        private void ShowSearch(object sender, RoutedEventArgs e)
        {
            HockeyClient.Current.TrackTrace("Show search clicked");

            if (this.ProductSearchControl.Visibility == Visibility.Collapsed)
            {
                // todo: init by triggering IsEnabled as well?
                this.ProductViewer.Opacity = 0.1;
                this.ProductSearchControl.Visibility = Visibility.Visible;
                this.ProductSearchControl.IsEnabled = true;
            }
            else
            {
                // resetting the layout is triggered by IsEnabled
                this.ProductSearchControl.IsEnabled = false;
            }
        }

        private void ChangeLanguage(object sender, RoutedEventArgs e)
        {
            this.ProductsController.NextLanguage();

            HockeyClient.Current.TrackTrace("Language changed to " + ProductsController.LanguageCode);
        }

        private void UnknownProductCtrl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.ProductViewer.Opacity = 1;
            var ctrl = sender as UnknownProductControl;
            ctrl.Visibility = Visibility.Collapsed;
        }
    }
}
