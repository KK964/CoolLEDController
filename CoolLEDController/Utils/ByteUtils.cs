using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLEDController.Utils
{
    internal class ByteUtils
    {
        private static string HEX_CODES = "0123456789ABCDEF";

        public static byte[] ListStringToByteArray(List<string> list)
        {
            byte[] byteArray = new byte[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                string hexStr = list[i];
                byteArray[i] = (byte)Convert.ToInt16(hexStr, 16);
            }
            return byteArray;
        }

        public static string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(HEX_CODES[(b & 240) >> 4]);
                sb.Append(HEX_CODES[b & (byte)15]);
            }
            return sb.ToString();
        }

        public static string BytesToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                string hexString = (b & 255).ToString("X");
                if (hexString.Length < 2) sb.Append("0");
                sb.Append(hexString);
            }
            return sb.ToString();
        }

        public static string[] BytesToHexString1(byte[] bytes)
        {
            string[] strArr = new string[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                StringBuilder sb = new StringBuilder();
                string hexStr = (bytes[i] & 255).ToString("X");
                if (hexStr.Length < 2) sb.Append("0");
                sb.Append(hexStr);
                strArr[i] = sb.ToString();
            }
            return strArr;
        }

        public static string BytesToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public static byte[] HexStringToBytes(string str)
        {
            str = str.ToUpper();
            int length = str.Length / 2;
            char[] chars = str.ToCharArray();
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                int index = i * 2;
                bytes[i] = (byte)(CharToByte(chars[index + 1]) | (chars[index] << 4));
            }
            return bytes;
        }

        public static byte CharToByte(char ch)
        {
            return (byte)HEX_CODES.IndexOf(ch);
        }

        public static string IntToHexString(int i)
        {
            string hexString = i.ToString("X");
            if (hexString.Length == 2) hexString = "0" + hexString;
            return hexString;
        }

        public static List<byte[]> SplitBytes(byte[] bArr)
        {
            int i = 20;
            List<byte[]> bl = new List<byte[]>();
            int i2 = 0;
            byte[] bArr2;
            if (bArr.Length % i == 0) i2 = bArr.Length / i;
            else i2 = (int)Math.Round((float)((bArr.Length / i) + 1));
            if (i2 <= 0) return bl;
            byte[] bytes2;
            for (int i3 = 0; i3 < i2; i3++)
            {
                if (i2 == 1 || i3 == i2 - 1)
                {
                    int length = bArr.Length % i == 0 ? i : bArr.Length % i;
                    byte[] bArr3 = new byte[length];
                    Array.Copy(bArr, i3 * i, bArr3, 0, length);
                    bArr2 = bArr3;
                }
                else
                {
                    bArr2 = new byte[i];
                    Array.Copy(bArr, i3 * i, bArr2, 0, i);
                }
                bl.Add(bArr2);
            }
            return bl;
        }
    }
}
