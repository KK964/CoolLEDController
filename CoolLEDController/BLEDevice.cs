using CoolLEDController.Utils;
using CoolLEDProtocols;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CoolLEDController
{
    public class BLEDevice
    {
        private ulong bleAddress;
        private BLECommandWriter bleCommandWriter;
        public BLEDevice(ulong address, BLECommandWriter bleCommandWriter)
        {
            bleAddress = address;
            this.bleCommandWriter = bleCommandWriter;
        }

        public void SendText(string text)
        {
            SendTransfer();
            List<TextEmojiItem> txtEmoji = new List<TextEmojiItem>();
            TextEmojiItem item = new TextEmojiItem();
            item.text = Regex.Replace(text, @"[^ -~]", "");
            txtEmoji.Add(item);
            BLECommands commands = new BLECommands(bleAddress, ByteEncoder.getTextDataStringsForTextEmoji(txtEmoji));
            bleCommandWriter.Write(commands);
        }

        public void SendImage(Frame frame)
        {
            SendTransfer();
            BLECommands commands = new BLECommands(bleAddress, ByteEncoder.getIconDataStrings(ByteEncoder.GetDrawList(frame)));
            bleCommandWriter.Write(commands);
        }

        public void SendAnimation(Frames frames)
        {
            SendTransfer();
            BLECommands commands = new BLECommands(bleAddress, ByteEncoder.getSendDataAnimationData(frames, 48, 12));
            bleCommandWriter.Write(commands);
        }

        public void SendMode(AnimationState state)
        {
            SendTransfer();
            BLECommands commands = new BLECommands(bleAddress, ByteEncoder.getModeDataString((int)state));
            bleCommandWriter.Write(commands);
        }

        public void SendSpeed(int speed)
        {
            if (speed > 256) speed = 256;
            SendTransfer();
            BLECommands commands = new BLECommands(bleAddress, ByteEncoder.getSpeedDataString(speed));
            bleCommandWriter.Write(commands);
        }

        private void SendTransfer()
        {
            List<string> rawData = new List<string>();
            rawData.AddRange(CommandBytes.BeginTransferStartString);
            BLECommands commands = new BLECommands(bleAddress, ByteEncoder.GetProperFormattedCmd(rawData));
            bleCommandWriter.Write(commands);
        }
    }

    public class BLECommands
    {
        public ulong BLEAddress;
        public List<List<byte[]>> commands;

        public BLECommands(ulong address, List<List<string>> cmds)
        {
            this.BLEAddress = address;
            List<List<byte[]>> result = new List<List<byte[]>>();
            foreach (List<string> cmd in cmds)
            {
                result.Add(ByteUtils.SplitBytes(ByteUtils.ListStringToByteArray(cmd)));
            }
            commands = result;
        }

        public BLECommands(ulong address, List<string> cmd) : this(address, new List<List<string>> { cmd }) { }
    }

    internal class BLECommand
    {
        public ulong BLEAddress;
        public byte[] command;

        public BLECommand(ulong address, byte[] cmd)
        {
            this.BLEAddress = address;
            this.command = cmd;
        }
    }
}
