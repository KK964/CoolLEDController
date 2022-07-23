using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLEDController.Utils
{
    internal class CommandBytes
    {
        private static List<string> beginTransferStartString = new List<string>() { "0a" };
        private static List<string> brightStartString = new List<string>() { "08" };
        private static List<string> drawStartString = new List<string>() { "03" };
        private static List<string> endString = new List<string>() { "03" };
        private static List<string> iconStartString = new List<string>() { "05" };
        private static List<string> modeStartString = new List<string>() { "06" };
        private static List<string> musicStartString = new List<string>() { "01" };
        private static List<string> speedStartString = new List<string>() { "07" };
        private static List<string> startString = new List<String>() { "01" };
        private static List<string> switchStartString = new List<string>() { "09" };
        private static List<string> textStartString = new List<string>() { "02" };

        public static List<string> BeginTransferStartString { get { return beginTransferStartString; } }
        public static List<string> BrightStartString { get { return brightStartString; } }
        public static List<string> DrawStartString { get { return drawStartString; } }
        public static List<string> EndString { get { return endString; } }
        public static List<string> IconStartString { get { return iconStartString; } }
        public static List<string> ModeStartString { get { return modeStartString; } }
        public static List<string> MusicStartString { get { return musicStartString; } }
        public static List<string> SpeedStartString { get { return speedStartString; } }
        public static List<string> StartString { get { return startString; } }
        public static List<string> SwitchStartString { get { return switchStartString; } }
        public static List<string> TextStartString { get { return textStartString; } }
    }
}
