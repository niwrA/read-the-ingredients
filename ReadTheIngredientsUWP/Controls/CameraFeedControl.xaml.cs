using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ReadTheIngredientsUWP.Controls
{
    public sealed partial class CameraFeedControl : UserControl
    {
        private CameraController _ctrl;
        public event EventHandler StartPressed;
        public event EventHandler StopPressed;

        public void OnStartPressed()
        {
            StartPressed?.Invoke(this, null);
        }

        public void OnStopPressed()
        {
            StopPressed?.Invoke(this, null);
        }

        public CameraFeedControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += CameraFeedControl_DataContextChanged;
        }
        public async Task Start()
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    // must run in UI thread in case a user consent prompt needs to be displayed, see:
                    // https://msdn.microsoft.com/en-us/windows/uwp/audio-video-camera/simple-camera-preview-access
                    await StartPreviewAsync();
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async void Stop()
        {
            await StopPreviewAsync();
        }
        public void Reset()
        {

        }
        private void CameraFeedControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue is CameraController && _ctrl == null)
            {
                var ctrl = args.NewValue as CameraController;
                _ctrl = ctrl;
            }
        }

        private async Task StartPreviewAsync()
        {
            bool IsCameraAvailable = false;

            OnStartPressed();

            try
            {
                await _ctrl.InitializeCameraAsync();
                IsCameraAvailable = _ctrl.MediaCapture != null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (!_ctrl.IsFixedFocus)
            {
                if (IsCameraAvailable && _ctrl != null && PreviewControl.Visibility == Visibility.Collapsed)
                {
                    PreviewControl.Source = _ctrl.MediaCapture;
                    PreviewControl.Visibility = Visibility.Visible;
                    await _ctrl.StartPreviewAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Camera not available.");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Camera is fixed focus, try to fall back to ZXing.");
            }
        }

        /// Stops the preview and deactivates a display request, to allow the screen to go into power saving modes
        /// </summary>
        /// <returns></returns>
        private async Task StopPreviewAsync()
        {
            // Stop the preview
            await _ctrl.StopPreviewAsync();

            PreviewControl.Source = null;
            PreviewControl.Visibility = Visibility.Collapsed;

            OnStopPressed();
        }

        //private async void ScenarioStartScanButton_Click(object sender, RoutedEventArgs e)
        //{
        //    await StartPreviewAsync();
        //}

        //private async void ScenarioEndScanButton_Click(object sender, RoutedEventArgs e)
        //{
        //    await StopPreviewAsync();
        //}
    }
}
