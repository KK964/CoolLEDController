using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLEDController.Utils
{
    internal class HexUtils
    {
        private static char[] DIGITS_LOWER = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
        private static char[] DIGITS_UPPER = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        public static char[] encodeHex(byte[] bArr)
        {
            return encodeHex(bArr, true);
        }

        public static char[] encodeHex(byte[] bArr, bool z)
        {
            return encodeHex(bArr, z ? DIGITS_LOWER : DIGITS_UPPER);
        }

        protected static char[] encodeHex(byte[] bArr, char[] cArr)
        {
            if (bArr == null)
            {
                return null;
            }
            int length = bArr.Length;
            char[] cArr2 = new char[(length << 1)];
            int i = 0;
            for (int i2 = 0; i2 < length; i2++)
            {
                int i3 = i + 1;
                cArr2[i] = cArr[((uint)(bArr[i2] & 240)) >> 4];
                i = i3 + 1;
                cArr2[i3] = cArr[bArr[i2] & 15];
            }
            return cArr2;
        }

        public static String encodeHexStr(byte[] bArr)
        {
            return encodeHexStr(bArr, true);
        }

        public static String encodeHexStr(byte[] bArr, bool z)
        {
            return encodeHexStr(bArr, z ? DIGITS_LOWER : DIGITS_UPPER);
        }

        protected static String encodeHexStr(byte[] bArr, char[] cArr)
        {
            return new String(encodeHex(bArr, cArr));
        }

        public static String formatHexString(byte[] bArr)
        {
            return formatHexString(bArr, false);
        }

        public static String formatHexString(byte[] bArr, bool z)
        {
            if (bArr == null || bArr.Length < 1)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bArr)
            {
                String hexString = (b & 255).ToString("X");
                if (hexString.Length == 1)
                {
                    hexString = '0' + hexString;
                }
                sb.Append(hexString);
                if (z)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString().Trim();
        }

        public static byte[] decodeHex(char[] cArr)
        {
            int length = cArr.Length;
            if ((length & 1) == 0)
            {
                byte[] bArr = new byte[(length >> 1)];
                int i = 0;
                int i2 = 0;
                while (i < length)
                {
                    int i3 = i + 1;
                    i = i3 + 1;
                    bArr[i2] = (byte)(((toDigit(cArr[i], i) << 4) | toDigit(cArr[i3], i3)) & 255);
                    i2++;
                }
                return bArr;
            }
            return null;
        }

        protected static int toDigit(char c, int i)
        {
            int digit = Convert.ToInt32(c.ToString(), 16);
            return digit;
        }

        public static byte[] hexStringToBytes(String str)
        {
            if (str == null || str == "")
            {
                return null;
            }
            string upperCase = str.Trim().ToUpper();
            int length = upperCase.Length / 2;
            char[] charArray = upperCase.ToCharArray();
            byte[] bArr = new byte[length];
            for (int i = 0; i < length; i++)
            {
                int i2 = i * 2;
                bArr[i] = (byte)(charToByte(charArray[i2 + 1]) | (charToByte(charArray[i2]) << 4));
            }
            return bArr;
        }

        public static byte charToByte(char c)
        {
            return (byte)"0123456789ABCDEF".IndexOf(c);
        }

        public static String extractData(byte[] bArr, int i)
        {
            return formatHexString(new byte[] { bArr[i] });
        }
    }

}
