using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;

namespace CoolLEDController
{
    internal class BLEScan
    {
        private static Guid UUID_SERVICE = Guid.Parse("0000fff0-0000-1000-8000-00805f9b34fb");

        private Dictionary<ulong, AdDevice> NearbyDevices = new Dictionary<ulong, AdDevice>();

        private BluetoothLEAdvertisementWatcher watcher;

        private bool isSearching = false;

        public BLEScan()
        {
            // Filter advertisments to search for CoolLED panels
            BluetoothLEAdvertisementFilter filter = new BluetoothLEAdvertisementFilter();
            filter.Advertisement.ServiceUuids.Add(UUID_SERVICE);
            watcher = new BluetoothLEAdvertisementWatcher(filter);
        }

        public List<AdDevice> StartSearch()
        {
            List<AdDevice> adDevices = new List<AdDevice>();
            Start();

            isSearching = true;
            bool searching = true;
            int attempts = 0;

            // Search for 5 seconds
            // This should be optimized in the future
            while (isSearching && searching && attempts < 50)
            {
                attempts++;
                Thread.Sleep(100);
            }

            foreach (AdDevice adDevice in NearbyDevices.Values)
            {
                adDevices.Add(adDevice);
            }

            return adDevices;
        }

        private void Start()
        {
            // Set search range
            watcher.SignalStrengthFilter.InRangeThresholdInDBm = -110;
            watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -120;

            // Set out of range timeout
            watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);

            // Start event watchers
            watcher.Received += WatcherReceived;
            watcher.Stopped += WatcherStopped;

            // Start watching
            watcher.Start();
        }

        private void WatcherStopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            isSearching = false;
        }

        private void WatcherReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            if (sender != watcher) return;
            if (NearbyDevices.ContainsKey(args.BluetoothAddress)) return;
            AdDevice adDevice = new AdDevice(args.BluetoothAddress, args.Advertisement);
            NearbyDevices.Add(adDevice.Address, adDevice);
        }
    }

    public class AdDevice
    {
        private string manufacturerId;
        public string ManufacturerID { get { return manufacturerId; } }

        private ulong address;
        public ulong Address { get { return address; } }

        private BluetoothLEAdvertisement advertisement;
        public BluetoothLEAdvertisement Advertisement { get { return advertisement; } }

        private static string GetManufacturerId(BluetoothLEAdvertisement advertisement)
        {
            string manufacturerDataString = "";
            var manufacturerSections = advertisement.ManufacturerData;
            if (manufacturerSections.Count > 0)
            {
                var manufacturerData = manufacturerSections[0];
                var data = new byte[manufacturerData.Data.Length];
                using (var reader = DataReader.FromBuffer(manufacturerData.Data))
                {
                    reader.ReadBytes(data);
                }
                manufacturerDataString = string.Format("0x{0}: {1}",
                    manufacturerData.CompanyId.ToString("X"),
                    BitConverter.ToString(data));
            }
            return manufacturerDataString;
        }

        public AdDevice(ulong address, BluetoothLEAdvertisement ad)
        {
            this.address = address;
            this.advertisement = ad;
            this.manufacturerId = GetManufacturerId(ad);
        }
    }
}
