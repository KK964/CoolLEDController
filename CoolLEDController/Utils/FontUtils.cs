using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLEDController.Utils
{
    internal class FontUtils
    {
        private byte[] FONT_BYTES;

        public FontUtils()
        {
            FONT_BYTES = Properties.Resources.UNICODE12;
        }

        public TextByteData getFontByteData1248(String str)
        {
            TextByteData textByteData = new TextByteData();
            List<string> arrayList = new List<string>();
            byte[] bArr = null;
            for (int i = 0; i < str.Length; i++)
            {
                byte[] bArr2 = readUnicode1248(str[i]);
                arrayList.Add(bArr2.Length.ToString("X"));
                if (bArr == null)
                {
                    bArr = bArr2;
                }
                else
                {
                    bArr = concat(bArr, bArr2);
                }
            }
            textByteData.length = arrayList;
            textByteData.data = bArr;
            return textByteData;
        }

        private byte[] readUnicode1248(char c)
        {
            byte[] bArr = null;
            try
            {
                bArr = new byte[24];
                Array.Copy(FONT_BYTES, (long)(c * 24), bArr, 0, 24);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
            byte[] bArr3 = checkFontData(bArr);
            return bArr3;
        }

        private static byte[] checkFontData(byte[] bArr)
        {
            int i = 0;
            int length = bArr.Length - 1;

            while (true)
            {
                if (length <= 0)
                {
                    i = -1;
                    break;
                }
                else if (bArr[length] == 0 && bArr[length - 1] == 0)
                {
                    length -= 2;
                }
                else
                {
                    i = length + 1;
                    break;
                }
            }
            i = length + 1;
            if (i > 0)
            {
                byte[] bArr2 = new byte[(i + 2)];
                for (int i2 = 0; i2 < i; i2++)
                {
                    bArr2[i2] = bArr[i2];
                }
                bArr2[i] = 0;
                bArr2[i + 1] = 0;
                return bArr2;
            }
            else if (i >= 0)
            {
                return bArr;
            }
            else
            {
                int length2 = bArr.Length / 2;
                byte[] bArr3 = new byte[length2];
                for (int i3 = 0; i3 < length2; i3++)
                {
                    bArr3[i3] = 0;
                }
                return bArr3;
            }
        }

        public static byte[] concat(byte[] bArr, byte[] bArr2)
        {
            byte[] copyOfArr = new byte[bArr.Length + bArr2.Length];
            Array.Copy(bArr, copyOfArr, bArr.Length);
            Array.ConstrainedCopy(bArr2, 0, copyOfArr, bArr.Length, bArr2.Length);
            return copyOfArr;
        }

        public static byte[] copyOf(byte[] bArr, int length)
        {
            byte[] copyOf = new byte[length];
            for (int i = 0; i < length; i++)
            {
                copyOf[i] = 0;
                if (bArr.Length >= i + 1)
                {
                    copyOf[i] = bArr[i];
                }
            }
            return copyOf;
        }

        public TextByteData getFontByteData1248ForTextEmoji(List<TextEmojiItem> list)
        {
            byte[] concated;
            TextByteData textByteData = new TextByteData();
            List<string> arrayList = new List<string>();
            byte[] bArr = null;
            foreach (TextEmojiItem next in list)
            {
                if (next.isText)
                {
                    for (int i = 0; i < next.text.Length; i++)
                    {
                        byte[] unicode = readUnicode1248(next.text[i]);
                        if (bArr == null)
                        {
                            bArr = unicode;
                        }
                        else
                        {
                            bArr = concat(bArr, unicode);
                        }
                        arrayList.Add(unicode.Length.ToString("X"));
                    }
                }
                else
                {
                    byte[] bArr2 = null;
                    foreach (String hexStringToBytes in getDrawListDataForEmoji(next.listData))
                    {
                        byte[] hexStringToBytes2 = HexUtils.hexStringToBytes(hexStringToBytes);
                        if (bArr2 == null)
                        {
                            bArr2 = hexStringToBytes2;
                        }
                        else
                        {
                            bArr2 = concat(bArr2, hexStringToBytes2);
                        }
                    }
                    byte[] emptyColumnForEmoji = addEmptyColumnForEmoji(bArr2);
                    if (bArr == null)
                    {
                        concated = emptyColumnForEmoji;
                    }
                    else
                    {
                        concated = concat(bArr, emptyColumnForEmoji);
                    }
                    arrayList.Add(emptyColumnForEmoji.Length.ToString("X"));
                }
            }
            textByteData.length = arrayList;
            textByteData.data = bArr;
            return textByteData;
        }

        private static byte[] addEmptyColumnForEmoji(byte[] bArr)
        {
            return concat(bArr, new byte[] { 0, 0 });
        }

        public static List<string> getDrawListDataForEmoji(List<LEDState> list)
        {
            List<string> arrayList = new List<string>();
            for (int i = 0; i < 12; i++)
            {
                int i2 = 0;
                while (i2 < 2)
                {
                    int i3 = i2 == 0 ? 8 : i2 == 1 ? 4 : 0;
                    int i4 = 0;
                    for (int i5 = 0; i5 < i3; i5++)
                    {
                        double d = list[(((i2 * 8) + i5) * 12) + i].data ? 1.0d : 0.0d;
                        double pow = Math.Pow(2.0d, (double)(7 - i5));
                        i4 += (int)(d * pow);
                    }
                    arrayList.Add(i4.ToString("X"));
                    i2++;
                }
            }
            return arrayList;
        }
    }
}
