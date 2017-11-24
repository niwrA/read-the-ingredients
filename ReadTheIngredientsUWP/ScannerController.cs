using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.PointOfService;
using Windows.Foundation;
using Windows.UI.Core;

namespace ReadTheIngredientsUWP
{
    public class ScannerController
    {
        BarcodeScanner scanner = null;
        public BarcodeScanner Scanner { get { return scanner; } set { scanner = value; } }
        ClaimedBarcodeScanner claimedScanner = null;

        public event TypedEventHandler<ClaimedBarcodeScanner, BarcodeScannerDataReceivedEventArgs> DataReceived;
        private PosDeviceWatcher posDeviceWatcher = null;
        protected virtual void OnDataReceived(ClaimedBarcodeScanner sender, BarcodeScannerDataReceivedEventArgs e)
        {
            DataReceived?.Invoke(sender, e);
        }
        public async Task CreateScanner(CoreDispatcher dispatcher)
        {
            //StartWatcher(dispatcher);
            if (await CreateDefaultScannerObject())
            {
                // after successful creation, claim the scanner for exclusive use and enable it so that data reveived events are received.
                if (await ClaimScanner())
                {

                    // It is always a good idea to have a release device requested event handler. If this event is not handled, there are chances of another app can 
                    // claim ownsership of the barcode scanner.
                    claimedScanner.ReleaseDeviceRequested += claimedScanner_ReleaseDeviceRequested;

                    // after successfully claiming, attach the datareceived event handler.
                    claimedScanner.DataReceived += ClaimedScanner_DataReceived; ; // todo: create 'canreceivedata event' 
                    // Ask the API to decode the data by default. By setting this, API will decode the raw data from the barcode scanner and 
                    // send the ScanDataLabel and ScanDataType in the DataReceived event
                    claimedScanner.IsDecodeDataEnabled = true;

                    // enable the scanner.
                    // Note: If the scanner is not enabled (i.e. EnableAsync not called), attaching the event handler will not be any useful because the API will not fire the event 
                    // if the claimedScanner has not beed Enabled
                    await claimedScanner.EnableAsync();

                    //rootPage.NotifyUser("Ready to scan. Device ID: " + claimedScanner.DeviceId, NotifyType.StatusMessage);

                    await StartSoftwareTrigger();
                }
            }
        }

        private void ClaimedScanner_DataReceived(ClaimedBarcodeScanner sender, BarcodeScannerDataReceivedEventArgs args)
        {
            OnDataReceived(sender, args);
        }

        private async Task<bool> CreateScannerObjectFromVideo()
        {
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
                }
            }
            scanner = await BarcodeScanner.FromIdAsync(device.Id);

            if (scanner == null)
            {
                //rootPage.NotifyUser("Failed to create barcode scanner object.", NotifyType.ErrorMessage);
                return false;
            }
            return true;
        }
        private async Task<bool> CreateDefaultScannerObject()
        {
            if (scanner == null)
            {
                //rootPage.NotifyUser("Creating barcode scanner object.", NotifyType.StatusMessage);

                if (posDeviceWatcher.FoundDeviceList != null && posDeviceWatcher.FoundDeviceList.Count > 0)
                {
                    scanner = await BarcodeScanner.FromIdAsync(posDeviceWatcher.FoundDeviceList[posDeviceWatcher.FoundDeviceList.Count - 1].Id);

                    if (scanner == null)
                    {
                        //rootPage.NotifyUser("Failed to create barcode scanner object.", NotifyType.ErrorMessage);
                        return false;
                    }
                }
                else
                {
                    //rootPage.NotifyUser("Barcode scanner not found. Please connect a barcode scanner.", NotifyType.ErrorMessage);
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> ClaimScanner()
        {
            if (claimedScanner == null)
            {
                // claim the barcode scanner
                claimedScanner = await scanner.ClaimScannerAsync();

                // enable the claimed barcode scanner
                if (claimedScanner == null)
                {
                    //rootPage.NotifyUser("Claim barcode scanner failed.", NotifyType.ErrorMessage);
                    return false;
                }
            }
            return true;
        }



        private async Task StartSoftwareTrigger()
        {

            if (scanner.Capabilities.IsSoftwareTriggerSupported)
            {
                await claimedScanner.StartSoftwareTriggerAsync();
            }
            // reset the trigger buttons' state
            // rootPage.NotifyUser("Start Software Trigger", NotifyType.StatusMessage);
        }

        async void claimedScanner_ReleaseDeviceRequested(object sender, ClaimedBarcodeScanner e)
        {
            // always retain the device
            e.RetainDevice();

            //rootPage.NotifyUser("Event ReleaseDeviceRequested received. Retaining the barcode scanner.", NotifyType.StatusMessage);
        }

        private async Task StopSoftwareTrigger()
        {
            if (scanner.Capabilities.IsSoftwareTriggerSupported)
            {
                await claimedScanner.StopSoftwareTriggerAsync();
            }
            // reset the trigger buttons' state
            //rootPage.NotifyUser("Stop Software Trigger", NotifyType.StatusMessage);
        }

        internal void Reset()
        {
            if (claimedScanner != null)
            {
                // Detach the event handlers
                claimedScanner.DataReceived -= ClaimedScanner_DataReceived;
                claimedScanner.ReleaseDeviceRequested -= claimedScanner_ReleaseDeviceRequested;
                // Release the Barcode Scanner and set to null
                claimedScanner.Dispose();
                claimedScanner = null;
            }

            scanner = null;

        }

        internal async void Stop()
        {
            await StopSoftwareTrigger();
        }

        /// <summary>
        /// Start device watcher.
        /// </summary>
        public void StartWatcher(CoreDispatcher dispatcher)
        {
            if (posDeviceWatcher == null)
            {
                posDeviceWatcher = new PosDeviceWatcher(BarcodeScanner.GetDeviceSelector(), dispatcher);
                posDeviceWatcher.Start();
            }
        }

        /// <summary>
        /// Stop device watcher.
        /// </summary>
        public void StopWatcher()
        {
            if (posDeviceWatcher != null)
            {
                posDeviceWatcher.Stop();
                //posDeviceWatcher = null;
            }
        }
    }
}
