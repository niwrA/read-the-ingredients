using ProductsControllerShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Storage.FileProperties;
using Windows.System.Display;

namespace ReadTheIngredientsUWP
{
    public class CameraRotationHelper
    {
        private EnclosureLocation _cameraEnclosureLocation;
        private DisplayInformation _displayInformation = DisplayInformation.GetForCurrentView();
        private SimpleOrientationSensor _orientationSensor = SimpleOrientationSensor.GetDefault();
        public event EventHandler<bool> OrientationChanged;

        public CameraRotationHelper(EnclosureLocation cameraEnclosureLocation)
        {
            _cameraEnclosureLocation = cameraEnclosureLocation;
            if (!IsEnclosureLocationExternal(_cameraEnclosureLocation))
            {
                _orientationSensor.OrientationChanged += SimpleOrientationSensor_OrientationChanged;
            }
            _displayInformation.OrientationChanged += DisplayInformation_OrientationChanged;
        }

        private void SimpleOrientationSensor_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            if (args.Orientation != SimpleOrientation.Faceup && args.Orientation != SimpleOrientation.Facedown)
            {
                HandleOrientationChanged(false);
            }
        }

        private void DisplayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            HandleOrientationChanged(true);
        }

        private void HandleOrientationChanged(bool updatePreviewStreamRequired)
        {
            var handler = OrientationChanged;
            if (handler != null)
            {
                handler(this, updatePreviewStreamRequired);
            }
        }

        public static bool IsEnclosureLocationExternal(EnclosureLocation enclosureLocation)
        {
            return (enclosureLocation == null || enclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown);
        }

        private bool IsCameraMirrored()
        {
            // Front panel cameras are mirrored by default
            return (_cameraEnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
        }

        private SimpleOrientation GetCameraOrientationRelativeToNativeOrientation()
        {
            // Get the rotation angle of the camera enclosure
            var enclosureAngle = ConvertClockwiseDegreesToSimpleOrientation((int)_cameraEnclosureLocation.RotationAngleInDegreesClockwise);

            // Account for the fact that, on portrait-first devices, the built in camera sensor is read at a 90 degree offset to the native orientation
            if (_displayInformation.NativeOrientation == DisplayOrientations.Portrait && !IsEnclosureLocationExternal(_cameraEnclosureLocation))
            {
                return AddOrientations(SimpleOrientation.Rotated90DegreesCounterclockwise, enclosureAngle);
            }
            else
            {
                return AddOrientations(SimpleOrientation.NotRotated, enclosureAngle);
            }
        }

        // Gets the rotation to rotate ui elements
        public SimpleOrientation GetUIOrientation()
        {
            if (IsEnclosureLocationExternal(_cameraEnclosureLocation))
            {
                // Cameras that are not attached to the device do not rotate along with it, so apply no rotation
                return SimpleOrientation.NotRotated;
            }

            // Return the difference between the orientation of the device and the orientation of the app display
            var deviceOrientation = _orientationSensor.GetCurrentOrientation();
            var displayOrientation = ConvertDisplayOrientationToSimpleOrientation(_displayInformation.CurrentOrientation);
            return SubOrientations(displayOrientation, deviceOrientation);
        }

        // Gets the rotation of the camera to rotate pictures/videos when saving to file
        public SimpleOrientation GetCameraCaptureOrientation()
        {
            if (IsEnclosureLocationExternal(_cameraEnclosureLocation))
            {
                // Cameras that are not attached to the device do not rotate along with it, so apply no rotation
                return SimpleOrientation.NotRotated;
            }

            // Get the device orienation offset by the camera hardware offset
            var deviceOrientation = _orientationSensor.GetCurrentOrientation();
            var result = SubOrientations(deviceOrientation, GetCameraOrientationRelativeToNativeOrientation());

            // If the preview is being mirrored for a front-facing camera, then the rotation should be inverted
            if (IsCameraMirrored())
            {
                result = MirrorOrientation(result);
            }
            return result;
        }

        // Gets the rotation of the camera to display the camera preview
        public SimpleOrientation GetCameraPreviewOrientation()
        {
            if (IsEnclosureLocationExternal(_cameraEnclosureLocation))
            {
                // Cameras that are not attached to the device do not rotate along with it, so apply no rotation
                return SimpleOrientation.NotRotated;
            }

            // Get the app display rotation offset by the camera hardware offset
            var result = ConvertDisplayOrientationToSimpleOrientation(_displayInformation.CurrentOrientation);
            result = SubOrientations(result, GetCameraOrientationRelativeToNativeOrientation());

            // If the preview is being mirrored for a front-facing camera, then the rotation should be inverted
            if (IsCameraMirrored())
            {
                result = MirrorOrientation(result);
            }
            return result;
        }

        public static PhotoOrientation ConvertSimpleOrientationToPhotoOrientation(SimpleOrientation orientation)
        {
            switch (orientation)
            {
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    return PhotoOrientation.Rotate90;
                case SimpleOrientation.Rotated180DegreesCounterclockwise:
                    return PhotoOrientation.Rotate180;
                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                    return PhotoOrientation.Rotate270;
                case SimpleOrientation.NotRotated:
                default:
                    return PhotoOrientation.Normal;
            }
        }

        public static int ConvertSimpleOrientationToClockwiseDegrees(SimpleOrientation orientation)
        {
            switch (orientation)
            {
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    return 270;
                case SimpleOrientation.Rotated180DegreesCounterclockwise:
                    return 180;
                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                    return 90;
                case SimpleOrientation.NotRotated:
                default:
                    return 0;
            }
        }

        private SimpleOrientation ConvertDisplayOrientationToSimpleOrientation(DisplayOrientations orientation)
        {
            SimpleOrientation result;
            switch (orientation)
            {
                case DisplayOrientations.Landscape:
                    result = SimpleOrientation.NotRotated;
                    break;
                case DisplayOrientations.PortraitFlipped:
                    result = SimpleOrientation.Rotated90DegreesCounterclockwise;
                    break;
                case DisplayOrientations.LandscapeFlipped:
                    result = SimpleOrientation.Rotated180DegreesCounterclockwise;
                    break;
                case DisplayOrientations.Portrait:
                default:
                    result = SimpleOrientation.Rotated270DegreesCounterclockwise;
                    break;
            }

            // Above assumes landscape; offset is needed if native orientation is portrait
            if (_displayInformation.NativeOrientation == DisplayOrientations.Portrait)
            {
                result = AddOrientations(result, SimpleOrientation.Rotated90DegreesCounterclockwise);
            }

            return result;
        }

        private static SimpleOrientation MirrorOrientation(SimpleOrientation orientation)
        {
            // This only affects the 90 and 270 degree cases, because rotating 0 and 180 degrees is the same clockwise and counter-clockwise
            switch (orientation)
            {
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    return SimpleOrientation.Rotated270DegreesCounterclockwise;
                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                    return SimpleOrientation.Rotated90DegreesCounterclockwise;
            }
            return orientation;
        }

        private static SimpleOrientation AddOrientations(SimpleOrientation a, SimpleOrientation b)
        {
            var aRot = ConvertSimpleOrientationToClockwiseDegrees(a);
            var bRot = ConvertSimpleOrientationToClockwiseDegrees(b);
            var result = (aRot + bRot) % 360;
            return ConvertClockwiseDegreesToSimpleOrientation(result);
        }

        private static SimpleOrientation SubOrientations(SimpleOrientation a, SimpleOrientation b)
        {
            var aRot = ConvertSimpleOrientationToClockwiseDegrees(a);
            var bRot = ConvertSimpleOrientationToClockwiseDegrees(b);
            //add 360 to ensure the modulus operator does not operate on a negative
            var result = (360 + (aRot - bRot)) % 360;
            return ConvertClockwiseDegreesToSimpleOrientation(result);
        }

        private static VideoRotation ConvertSimpleOrientationToVideoRotation(SimpleOrientation orientation)
        {
            switch (orientation)
            {
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    return VideoRotation.Clockwise270Degrees;
                case SimpleOrientation.Rotated180DegreesCounterclockwise:
                    return VideoRotation.Clockwise180Degrees;
                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                    return VideoRotation.Clockwise90Degrees;
                case SimpleOrientation.NotRotated:
                default:
                    return VideoRotation.None;
            }
        }

        private static SimpleOrientation ConvertClockwiseDegreesToSimpleOrientation(int orientation)
        {
            switch (orientation)
            {
                case 270:
                    return SimpleOrientation.Rotated90DegreesCounterclockwise;
                case 180:
                    return SimpleOrientation.Rotated180DegreesCounterclockwise;
                case 90:
                    return SimpleOrientation.Rotated270DegreesCounterclockwise;
                case 0:
                default:
                    return SimpleOrientation.NotRotated;
            }
        }
    }
    public class CameraController : NotifyPropertyBase
    {
        private ScannerController _scannerController;
        private CameraRotationHelper _rotationHelper;
        public event EventHandler OnFixedFocusCameraDetected;


        public CameraController(ScannerController scannerController)
        {
            _scannerController = scannerController;

        }

        public ScannerController ScannerController
        {
            get { return _scannerController; }
        }
        /// <summary>
        /// Attempts to find the first available camera capture device
        /// </summary>
        /// <returns></returns>
        private async Task<DeviceInformation> FindCameraDeviceAsync()
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Return the first device found
            var device = (allVideoDevices.Count == 1) ? allVideoDevices[0] : null;
            if (allVideoDevices.Count > 1)
            {
                // try to get the back panel first
                device = allVideoDevices.First(w => w.EnclosureLocation.Panel == Panel.Back);
                if (device == null)
                {
                    device = allVideoDevices[0];
                }
            }
            if (device != null)
            {
                var isExternal = (device.EnclosureLocation == null || device.EnclosureLocation.Panel == Panel.Unknown || device.EnclosureLocation.InDock || device.EnclosureLocation.InLid);
                if (!isExternal)
                {
                    if (device.EnclosureLocation.RotationAngleInDegreesClockwise > 0)
                    {
                        //await SetPreviewRotationAsync();
                        //_rotationHelper = new CameraRotationHelper(device.EnclosureLocation);
                        //_rotationHelper.OrientationChanged += RotationHelper_OrientationChanged;
                    }
                    //OnFixedFocusCameraDetected?.Invoke(this, new EventArgs());
                }
            }
            return device;
        }

        private async void RotationHelper_OrientationChanged(object sender, bool updatePreview)
        {
            //if (updatePreview)
            //{
            //    await SetPreviewRotationAsync();
            //}
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    // Rotate the buttons in the UI to match the rotation of the device
            //    var angle = CameraRotationHelper.ConvertSimpleOrientationToClockwiseDegrees(_rotationHelper.GetUIOrientation());
            //    var transform = new RotateTransform { Angle = angle };

            //    // The RenderTransform is safe to use (i.e. it won't cause layout issues) in this case, because these buttons have a 1:1 aspect ratio
            //    CapturePhotoButton.RenderTransform = transform;
            //    CapturePhotoButton.RenderTransform = transform;
            //});
        }
        private async Task SetPreviewRotationAsync()
        {
            // Add rotation metadata to the preview stream to make sure the aspect ratio / dimensions match when rendering and getting preview frames
            var rotation = _rotationHelper.GetCameraPreviewOrientation();
            var props = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
            Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");
            props.Properties.Add(RotationKey, CameraRotationHelper.ConvertSimpleOrientationToClockwiseDegrees(rotation));
            await _mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);
        }

        /// <summary>
        /// Initialize camera capture device
        public async Task InitializeCameraAsync()
        {
            if (_mediaCapture == null)
            {
                // Attempt to get the first available camera 
                var cameraDevice = await FindCameraDeviceAsync();
                if (cameraDevice == null)
                {
                    //Debug.WriteLine("No camera device found!");
                    return;
                }

                // Create MediaCapture and its settings
                _mediaCapture = new MediaCapture();
                var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };

                // Initialize MediaCapture
                try
                {
                    settings.SharingMode = MediaCaptureSharingMode.SharedReadOnly;
                    await _mediaCapture.InitializeAsync(settings);
                    _isInitialized = true;
                }
                catch (UnauthorizedAccessException)
                {
                    //Debug.WriteLine("The app was denied access to the camera");
                }

                // Correctly set the focus settings
                var controller = _mediaCapture.VideoDeviceController;

                if (controller != null && controller.FocusControl != null && controller.FocusControl.Supported)
                {
                    var focusConfig = new Windows.Media.Devices.FocusSettings();
                    focusConfig.AutoFocusRange = Windows.Media.Devices.AutoFocusRange.Macro;

                    var supportContinuousFocus = controller.FocusControl.SupportedFocusModes.Contains(Windows.Media.Devices.FocusMode.Continuous);
                    var supportAutoFocus = controller.FocusControl.SupportedFocusModes.Contains(Windows.Media.Devices.FocusMode.Auto);

                    if (supportContinuousFocus)
                    {
                        focusConfig.Mode = Windows.Media.Devices.FocusMode.Continuous;
                    }
                    else if (supportAutoFocus)
                    {
                        focusConfig.Mode = Windows.Media.Devices.FocusMode.Auto;
                    }

                    controller.FocusControl.Configure(focusConfig);
                    await controller.FocusControl.FocusAsync();
                }
                else
                {
                    if (controller.Focus.Capabilities.AutoModeSupported)
                    {
                        controller.Focus.TrySetAuto(true);
                    }
                    else
                    {
                        IsFixedFocus = true;
                        //throw new FixedFocusNotSupportedException();         
                        OnFixedFocusCameraDetected?.Invoke(this, new EventArgs());
                    }
                }
            }
        }

        public async Task StartPreviewAsync()
        {
            // Prevent the device from sleeping while the preview is running
            _displayRequest.RequestActive();

            // Start the preview
            await _mediaCapture.StartPreviewAsync();
            _isPreviewing = true;
        }

        /// <summary>
        /// Stops the preview and deactivates a display request, to allow the screen to go into power saving modes
        /// </summary>
        /// <returns></returns>
        public async Task StopPreviewAsync()
        {
            // Stop the preview
            _isPreviewing = false;
            await _mediaCapture.StopPreviewAsync();

            // Allow the device screen to sleep now that the preview is stopped
            _displayRequest.RequestRelease();
        }

        public async Task CleanupCameraAsync()
        {
            //Debug.WriteLine("CleanupCameraAsync");

            if (_isInitialized)
            {
                if (_isPreviewing)
                {
                    // The call to stop the preview is included here for completeness, but can be
                    // safely removed if a call to MediaCapture.Dispose() is being made later,
                    // as the preview will be automatically stopped at that point
                    await StopPreviewAsync();
                }

                _isInitialized = false;
            }

            if (_mediaCapture != null)
            {
                _mediaCapture.Dispose();
                _mediaCapture = null;
            }
        }

        private readonly DisplayRequest _displayRequest = new DisplayRequest();
        private MediaCapture _mediaCapture;
        public MediaCapture MediaCapture { get { return _mediaCapture; } set { _mediaCapture = value; OnPropertyChanged(); } }

        public bool IsFixedFocus { get; private set; } = false;

        private bool _isInitialized;
        private bool _isPreviewing;
    }
}
