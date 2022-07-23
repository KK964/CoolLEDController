using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLEDController.Utils
{
    internal class TextData
    {
        public List<string> data;
        public List<string> length;
    }

    internal class TextByteData
    {
        public byte[] data;
        public List<string> length;
    }

    internal class TextEmojiItem
    {
        public string text;
        public bool isText = true;
        public List<LEDState> listData;
    }
}
