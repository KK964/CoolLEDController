using System;
using System.Collections.Generic;

namespace CoolLEDController
{
    public class BLEController
    {
        public BLEDevice GetDevice(string manuID, BLECommandWriter writer)
        {
            List<AdDevice> adDevices = GetNearbyPanels();
            AdDevice device = Array.Find<AdDevice>(adDevices.ToArray(), ad => ad.ManufacturerID == manuID);
            if (device == null) return null;
            BLEDevice bleDevice = new BLEDevice(device.Address, writer);
            return bleDevice;
        }

        public List<AdDevice> GetNearbyPanels()
        {
            BLEScan scanner = new BLEScan();
            return scanner.StartSearch();
        }
    }
}
