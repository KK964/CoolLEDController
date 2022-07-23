using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLEDController.Utils
{
    internal class ByteEncoder
    {
        public static List<string> GetProperFormattedCmd(List<string> data)
        {
            List<string> ret = new List<string>();
            ret.AddRange(CommandBytes.StartString);

            List<string> modifiedData = new List<string>();
            modifiedData.AddRange(getDataStringLength(data));
            modifiedData.AddRange(data);

            ret.AddRange(convertData(modifiedData, 0));
            ret.AddRange(CommandBytes.EndString);
            return ret;
        }

        public static List<string> getDataStringLength(List<String> list)
        {
            List<string> arrayList = new List<string>();
            if (list == null)
            {
                arrayList.Add("00");
                arrayList.Add("00");
                return arrayList;
            }
            String hexString = list.Count.ToString("X");
            if (hexString.Length == 1)
            {
                arrayList.Add("00");
                arrayList.Add("0" + hexString);
            }
            else if (hexString.Length == 2)
            {
                arrayList.Add("00");
                arrayList.Add(hexString);
            }
            else if (hexString.Length == 3)
            {
                arrayList.Add("0" + hexString.Substring(0, 1));
                arrayList.Add(hexString.Substring(1));
            }
            else if (hexString.Length == 4)
            {
                arrayList.Add(hexString.Substring(0, 2));
                arrayList.Add(hexString.Substring(2));
            }
            return arrayList;
        }

        public static List<string> convertData(List<string> list, int i)
        {
            while (i < list.Count())
            {
                int intValue = Convert.ToInt32(list[i], 16);
                if (intValue <= 0 || intValue >= 4)
                {
                    i++;
                }
                else
                {
                    list.Insert(i, "02");
                    list[i + 1] = ByteUtils.IntToHexString(intValue ^ 4);
                    int i2 = i + 2;
                    if (i2 > list.Count - 1)
                    {
                        return list;
                    }
                    return convertData(list, i2);
                }
            }
            return list;
        }

        public static List<String> getModeDataString(int i)
        {
            List<string> arrayList = new List<string>();
            arrayList.AddRange(CommandBytes.ModeStartString);
            arrayList.AddRange(getHexListStringForInt(i));
            return GetProperFormattedCmd(arrayList);
        }

        public static List<String> getSpeedDataString(int i)
        {
            List<string> arrayList = new List<string>();
            arrayList.AddRange(CommandBytes.SpeedStartString);
            arrayList.AddRange(getHexListStringForInt(i));
            return GetProperFormattedCmd(arrayList);
        }

        public static List<string> GetDrawList(Frame frame)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < 48; i++)
            {
                int i2 = 0;
                while (i2 < 2)
                {
                    int i3 = i2 == 0 ? 8 : i2 == 1 ? 4 : 0;
                    int i4 = 0;
                    for (int i5 = 0; i5 < i3; i5++)
                    {
                        double d = frame.panelStates[(((i2 * 8) + i5) * 48) + i].data ? 1.0d : 0.0d;
                        double pow = Math.Pow(2.0d, (double)(7 - i5));
                        if (Double.IsNaN(d)) continue;
                        i4 += (int)(d * pow);
                    }
                    string hexString = ByteUtils.IntToHexString(i4);
                    list.Add(hexString);
                    i2++;
                }
            }
            return list;
        }

        public static List<List<string>> getIconDataStrings(List<string> list)
        {
            List<List<string>> arrayList = new List<List<string>>();
            List<string> arrayList2 = new List<string>();
            arrayList2.Add("00");
            List<string> arrayList3 = new List<string>();
            for (int i = 0; i < 24; i++)
            {
                arrayList3.Add("00");
            }
            List<string> arrayList4 = new List<string>();
            arrayList4.AddRange(arrayList3);
            arrayList4.AddRange(getDataStringLength(list));
            arrayList4.AddRange(list);
            List<String> dataStringLength = getDataStringLength(arrayList4);
            List<List<string>> arrayList5 = new List<List<string>>();
            int size = arrayList4.Count / 128;
            int size2 = arrayList4.Count % 128;
            int i2 = 0;
            while (i2 < size)
            {
                int i3 = i2 * 128;
                i2++;
                arrayList5.Add(arrayList4.GetRange(i3, i2 * 128));
            }
            if (size2 > 0)
            {
                arrayList5.Add(arrayList4.GetRange(size * 128, arrayList4.Count));
            }
            for (int i4 = 0; i4 < arrayList5.Count; i4++)
            {
                List<string> arrayList6 = new List<string>();
                arrayList6.AddRange(arrayList2);
                arrayList6.AddRange(dataStringLength);
                arrayList6.AddRange(getHexListStringForIntWithTwo(i4));
                arrayList6.AddRange(getHexListStringForInt((arrayList5[i4]).Count));
                arrayList6.AddRange(arrayList5[i4]);
                arrayList6.AddRange(convertEnd(arrayList6));
                List<string> arrayList7 = new List<string>();
                arrayList7.AddRange(CommandBytes.DrawStartString);
                arrayList7.AddRange(arrayList6);
                arrayList.Add(GetProperFormattedCmd(arrayList7));
            }
            return arrayList;
        }

        public static List<String> convertEnd(List<String> list)
        {
            int intValue = 00;
            foreach (String valueOf in list)
            {
                intValue ^= Convert.ToInt32(valueOf, 16);
            }
            return getHexListStringForInt(intValue);
        }

        public static List<String> getHexListStringForInt(int i)
        {
            String hexString = i.ToString("X");
            List<string> arrayList = new List<string>();
            if (hexString.Length == 1)
            {
                arrayList.Add("0" + hexString);
            }
            else if (hexString.Length == 2)
            {
                arrayList.Add(hexString);
            }
            return arrayList;
        }

        public static List<String> getHexListStringForIntWithTwo(int i)
        {
            List<string> arrayList = new List<string>();
            String hexString = i.ToString("X");
            if (hexString.Length == 1)
            {
                arrayList.Add("00");
                arrayList.Add("0" + hexString);
            }
            else if (hexString.Length == 2)
            {
                arrayList.Add("00");
                arrayList.Add(hexString);
            }
            else if (hexString.Length == 3)
            {
                arrayList.Add("0" + hexString.Substring(0, 1));
                arrayList.Add(hexString.Substring(1));
            }
            else if (hexString.Length == 4)
            {
                arrayList.Add(hexString.Substring(0, 2));
                arrayList.Add(hexString.Substring(2));
            }
            return arrayList;
        }

        public static List<List<String>> getSendDataWithTypeStrings(String str, List<String> list)
        {
            List<List<string>> arrayList = new List<List<string>>();
            List<string> arrayList2 = new List<string>();
            arrayList2.Add("00");
            List<string> hexListStringForInt = getHexListStringForInt(Convert.ToInt32(str, 16));
            List<string> dataStringLength = getDataStringLength(list);
            List<List<string>> arrayList3 = new List<List<string>>();
            int size = list.Count / 128;
            int size2 = list.Count % 128;
            int i = 0;
            while (i < size)
            {
                int i2 = i * 128;
                i++;
                arrayList3.Add(list.GetRange(i2, i * 128 - i2));
            }
            if (size2 > 0)
            {
                arrayList3.Add(list.GetRange(size * 128, list.Count - size * 128));
            }
            for (int i3 = 0; i3 < arrayList3.Count; i3++)
            {
                List<string> arrayList4 = new List<string>();
                arrayList4.AddRange(arrayList2);
                arrayList4.AddRange(dataStringLength);
                arrayList4.AddRange(getHexListStringForIntWithTwo(i3));
                arrayList4.AddRange(getHexListStringForInt(arrayList3[i3].Count));
                arrayList4.AddRange(arrayList3[i3]);
                arrayList4.AddRange(convertEnd(arrayList4));
                List<string> arrayList5 = new List<string>();
                arrayList5.AddRange(hexListStringForInt);
                arrayList5.AddRange(arrayList4);
                arrayList.Add(GetProperFormattedCmd(arrayList5));
            }
            return arrayList;
        }

        public static List<String> getDrawListDataWithAnimation(Frame animationData)
        {
            List<LEDState> drawItems = animationData.panelStates;
            List<string> arrayList = new List<string>();
            for (int i = 0; i < 48; i++)
            {
                int i2 = 0;
                while (i2 < 2)
                {
                    int i3 = i2 == 0 ? 8 : i2 == 1 ? 4 : 0;
                    int i4 = 0;
                    for (int i5 = 0; i5 < i3; i5++)
                    {
                        double d = drawItems[(((i2 * 8) + i5) * 48) + i].data ? 1.0d : 0.0d;
                        double pow = Math.Pow(2.0d, (double)(7 - i5));
                        if (double.IsNaN(d)) continue;
                        i4 += (int)(d * pow);
                    }
                    arrayList.Add(i4.ToString("X"));
                    i2++;
                }
            }
            return arrayList;
        }

        public static List<List<String>> getSendDataAnimationData(Frames animationDatas, int i, int i2)
        {
            List<string> arrayList = new List<string>();
            for (int i3 = 0; i3 < 24; i3++)
            {
                arrayList.Add(0.ToString("X"));
            }
            arrayList.Add(animationDatas.animationStates.Count.ToString("X"));
            arrayList.Add(((int)(animationDatas.speed / 256)).ToString("X"));
            arrayList.Add(((int)(animationDatas.speed % 256)).ToString("X"));
            foreach (Frame drawListDataWithAnimation in animationDatas.animationStates)
            {
                arrayList.AddRange(getDrawListDataWithAnimation(drawListDataWithAnimation));
            }
            return getSendDataWithTypeStrings("04", arrayList);
        }

        public static TextData getTextData(String str, FontUtils fontUtils)
        {
            TextData textData = new TextData();
            List<string> arrayList = new List<string>();
            try
            {
                TextByteData fontByteData1248 = fontUtils.getFontByteData1248(str);
                arrayList.AddRange(byte2hex(fontByteData1248.data));
                textData.data = arrayList;
                textData.length = fontByteData1248.length;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
            return textData;
        }

        public static List<String> byte2hex(byte[] bArr)
        {
            List<string> arrayList = new List<string>();
            foreach (byte b in bArr)
            {
                String hexString = (b & 255).ToString("X");
                if (hexString.Length == 1)
                {
                    hexString = "0" + hexString;
                }
                arrayList.Add(hexString);
            }
            return arrayList;
        }

        public static TextData getTextDataForTextEmoji(List<TextEmojiItem> list, FontUtils fontUtils)
        {
            TextData textData = new TextData();
            List<string> arrayList = new List<string>();
            try
            {
                TextByteData fontByteData1248ForTextEmoji = fontUtils.getFontByteData1248ForTextEmoji(list);
                arrayList.AddRange(byte2hex(fontByteData1248ForTextEmoji.data));
                textData.data = arrayList;
                textData.length = fontByteData1248ForTextEmoji.length;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
            return textData;
        }

        public static List<List<String>> getTextDataStringsForTextEmoji(List<TextEmojiItem> list)
        {
            List<List<string>> arrayList = new List<List<string>>();
            List<string> arrayList2 = new List<string>();
            arrayList2.Add("00");
            TextData textDataForTextEmoji = getTextDataForTextEmoji(list, new FontUtils());
            List<string> list2 = textDataForTextEmoji.data;
            List<string> list3 = textDataForTextEmoji.length;

            if (list3.Count < 80)
            {
                int size = 80 - list3.Count;
                for (int i = 0; i < size; i++)
                {
                    list3.Add("00");
                }
            }

            List<string> arrayList3 = new List<string>();
            for (int i2 = 0; i2 < 24; i2++)
            {
                arrayList3.Add("00");
            }
            List<string> hexListStringForInt = getHexListStringForInt(list.Count);
            List<string> dataStringLength = getDataStringLength(list2);
            List<string> arrayList4 = new List<string>();
            arrayList4.AddRange(arrayList3);
            arrayList4.AddRange(hexListStringForInt);
            arrayList4.AddRange(list3);
            arrayList4.AddRange(dataStringLength);
            arrayList4.AddRange(list2);

            List<List<string>> arrayList5 = new List<List<string>>();
            int size2 = arrayList4.Count / 128;
            int size3 = arrayList4.Count % 128;
            int i3 = 0;
            while (i3 < size2)
            {
                int i4 = i3 * 128;
                i3++;
                arrayList5.Add(arrayList4.GetRange(i4, (i3 * 128) - i4));
            }
            if (size3 > 0)
            {
                arrayList5.Add(arrayList4.GetRange(size2 * 128, arrayList4.Count - (size2 * 128)));
            }
            for (int i5 = 0; i5 < arrayList5.Count; i5++)
            {
                List<string> arrayList6 = new List<string>();
                arrayList6.AddRange(arrayList2);
                arrayList6.AddRange(getDataStringLength(arrayList4));
                arrayList6.AddRange(getHexListStringForIntWithTwo(i5));
                arrayList6.AddRange(getHexListStringForInt(arrayList5[i5].Count));
                arrayList6.AddRange(arrayList5[i5]);
                arrayList6.AddRange(convertEnd(arrayList6));
                List<string> arrayList7 = new List<string>();
                arrayList7.AddRange(CommandBytes.TextStartString);
                arrayList7.AddRange(arrayList6);
                arrayList.Add(GetProperFormattedCmd(arrayList7));
            }
            return arrayList;
        }
    }
}
