using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace CoolLEDController
{
    public class BLECommandWriter
    {
        private static Guid DEVICE_GATT = Guid.Parse("0000fff0-0000-1000-8000-00805f9b34fb");
        private static Guid GATT_CHARACTERISTIC = Guid.Parse("0000fff1-0000-1000-8000-00805f9b34fb");

        // Not used - saved for potential future use
        private static Guid CHARACTERISTIC_DESCRIPTOR = Guid.Parse("00002902-0000-1000-8000-00805f9b34fb");

        private bool looping = true;
        private bool writingCommand = false;

        private Queue CommandQueue = new Queue();

        public void StartThread()
        {
            Thread thread = new Thread(Loop);
            thread.Start();
        }

        public void Write(BLECommands command)
        {
            if (command == null) return;
            foreach (List<byte[]> bytes in command.commands)
            {
                foreach (byte[] data in bytes)
                {
                    AddToQueue(new BLECommand(command.BLEAddress, data));
                }
            }
        }

        private void AddToQueue(BLECommand command)
        {
            try
            {
                Queue queue = Queue.Synchronized(CommandQueue);
                queue.Enqueue(command);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        private void Loop()
        {
            while (looping)
            {
                try
                {
                    Queue queue = Queue.Synchronized(CommandQueue);
                    if (!writingCommand && queue.Count > 0)
                    {
                        if (writingCommand) goto skip;
                        Console.WriteLine("Writing command...");
                        BLECommand command = (BLECommand)queue.Dequeue();
                        if (command == null) goto skip;
                        Write(command.BLEAddress, command.command);
                    }
                }
                catch (Exception e) { }

            skip:
                Thread.Sleep(100);
            }
        }

        private GattCharacteristic lastCharacteristic;
        private ulong lastBLEID;

        private async void Write(ulong address, byte[] bytes, int attempts = 0)
        {
            writingCommand = true;
            bool failed = false;
            try
            {
                if (lastBLEID == address) goto sameAsLast;
                lastBLEID = address;
                BluetoothLEDevice device = null;
                DeviceAccessStatus deviceAccess = DeviceAccessStatus.Unspecified;


                int connectionAttempts = 0;

                while ((device == null || deviceAccess != DeviceAccessStatus.Allowed) && connectionAttempts < 10)
                {
                    device = await BluetoothLEDevice.FromBluetoothAddressAsync(address);
                    deviceAccess = await device.RequestAccessAsync();
                    connectionAttempts++;
                }

                // Couldnt connect to device
                if (device == null || deviceAccess != DeviceAccessStatus.Allowed)
                {
                    writingCommand = false;
                    return;
                }

                connectionAttempts = 0;

                GattDeviceServicesResult gattDevice = null;

                while ((gattDevice == null || gattDevice.Status != GattCommunicationStatus.Success) && connectionAttempts < 10)
                {
                    gattDevice = await device.GetGattServicesForUuidAsync(DEVICE_GATT);
                    connectionAttempts++;
                }

                // Couldnt get device GATT
                if (gattDevice == null || gattDevice.Status != GattCommunicationStatus.Success || gattDevice.Services.Count == 0)
                {
                    writingCommand = false;
                    return;
                }

                GattDeviceService service = gattDevice.Services[0];

                connectionAttempts = 0;

                GattCharacteristicsResult gattCharacteristics = null;

                while ((gattCharacteristics == null || gattCharacteristics.Status != GattCommunicationStatus.Success) && connectionAttempts < 10)
                {
                    gattCharacteristics = await service.GetCharacteristicsForUuidAsync(GATT_CHARACTERISTIC);
                    connectionAttempts++;
                }

                // Couldnt get GATT Characteristic
                if (gattCharacteristics == null || gattCharacteristics.Status != GattCommunicationStatus.Success || gattCharacteristics.Characteristics.Count == 0)
                {
                    writingCommand = false;
                    return;
                }

                lastCharacteristic = gattCharacteristics.Characteristics[0];

            sameAsLast:

                // Writing command

                DataWriter dataWriter = new DataWriter();
                dataWriter.WriteBytes(bytes);

                IBuffer outBuffer = dataWriter.DetachBuffer();

                GattCommunicationStatus status = await lastCharacteristic.WriteValueAsync(outBuffer);

                Console.WriteLine("Finished command!");

                if (status.HasFlag(GattCommunicationStatus.Success)) Console.WriteLine("Success");
                else failed = true;
                if (status.HasFlag(GattCommunicationStatus.Unreachable)) Console.WriteLine("Unreachable");
                if (status.HasFlag(GattCommunicationStatus.AccessDenied)) Console.WriteLine("Access Denied");
                if (status.HasFlag(GattCommunicationStatus.ProtocolError)) Console.WriteLine("Protocol Error");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                failed = true;
            }

            if (failed)
            {
                if (attempts > 5)
                {
                    Console.Error.WriteLine("Failed to write command 5 times. Aboring");
                    writingCommand = false;
                    return;
                }
                Write(address, bytes, ++attempts);
                return;
            }

            writingCommand = false;
        }
    }

}
