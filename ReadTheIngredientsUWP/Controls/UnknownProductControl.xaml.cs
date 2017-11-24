using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ReadTheIngredientsUWP.Controls
{
    public abstract class NotifyPropertyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ProductEditModel : NotifyPropertyBase
    {
        private string _barcodePath;
        public string BarcodePath { get { return _barcodePath; }  set { _barcodePath = value; OnPropertyChanged(); } }

        private BitmapImage _barcodeImage;
        public BitmapImage BarcodeImage
        {
            get { return _barcodeImage; }
            set { _barcodeImage = value;  OnPropertyChanged(); }
        }

        private string _barcode;

        public string Barcode
        {
            get { return _barcode; }
            set { _barcode = value; OnPropertyChanged(); OnPropertyChanged("FormattedBarcode"); }
        }
        public string FormattedBarcode
        {
            get
            {
                if(!string.IsNullOrWhiteSpace(Barcode))
                {
                    if (Barcode.Length == 8) { return Barcode.Substring(0, 4) + " " + Barcode.Substring(4); }
                    else if (Barcode.Length == 13)
                    {
                        return Barcode.Substring(0, 1) + " " + Barcode.Substring(1, 6) + " " + Barcode.Substring(7);
                    }
                }
                return Barcode;
            }
        }

        public string Name { get; set; }
        public string NamePlaceholder { get { return "Enter product name"; } }
        public string Quantity { get; set; }
        public string QuantityPlaceholder { get { return "Enter quantity"; } }
        public string Brand { get; set; }
        public string BrandPlaceholder { get { return "Enter brand"; } }

        private object _productImage;
        public object ProductImage
        {
            get { return _productImage; }
            set { _productImage = value; OnPropertyChanged(); }
        }


        private object _nutrientsImage;
        public object NutrientsImage
        {
            get { return _nutrientsImage; }
            set { _nutrientsImage = value; OnPropertyChanged(); }
        }


        private object _ingredientsImage;
        public object IngredientsImage
        {
            get { return _ingredientsImage; }
            set { _ingredientsImage = value; OnPropertyChanged(); }
        }

    }
    public sealed partial class UnknownProductControl : UserControl
    {
        private ProductEditModel _editModel;
        public ProductEditModel Product { get { return _editModel; } set { _editModel = value; } }
        public event EventHandler OkPressed;
        public event EventHandler CancelPressed;
        public void OnOkPressed()
        {
            OkPressed?.Invoke(this, null);
        }

        public void OnCancelPressed()
        {
            CancelPressed?.Invoke(this, null);
        }
        public UnknownProductControl()
        {
            this.InitializeComponent();
            _editModel = new ProductEditModel();
            this.DataContext = _editModel;
        }

        public void Reset()
        {
            _editModel = new ProductEditModel();
        }

        private async void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var image = await PickImage(sender);

        }

        private async Task<BitmapImage> PickImage(object sender)
        {
            var imageCtrl = sender as Image;
            var bitmapImage = await PickFile();
            if (bitmapImage != null)
            {
                return bitmapImage;
            }
            return null;
        }

        private async Task<BitmapImage> PickFile()
        {
            // Clear previous returned file name, if it exists, between iterations of this scenario

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap
                    BitmapImage bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    return bitmapImage;
                }
            }
            return null;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            OnCancelPressed();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {

            OnOkPressed();
        }

        private async void SelectProductImage_Click(object sender, RoutedEventArgs e)
        {
            var image = await PickImage(this.ProductImage);
            Product.ProductImage = image;
            this.ProductImage.Source = image;
        }

        private async void SelectIngredientsImage_Click(object sender, RoutedEventArgs e)
        {
            var image = await PickImage(this.IngredientsImage);
            Product.IngredientsImage = image;
            this.IngredientsImage.Source = image;
        }

        private async void SelectNutrientsImage_Click(object sender, RoutedEventArgs e)
        {
            var image = await PickImage(this.NutrientsImage);
            Product.NutrientsImage = image;
            this.NutrientsImage.Source = image;
        }

        internal void SetBarcodeImagePath(string path)
        {
            var imageSource =  new BitmapImage(new Uri(path));
            this.BarcodeImage.Source = imageSource;
        }
    }
}
