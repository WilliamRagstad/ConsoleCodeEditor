using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConsoleCodeEditor
{
    struct Settings
    {
        public static string userDirectory = $@"C:\Users\{Environment.UserName}\";
        public static string cceHome = userDirectory + @".cce\";
        public static string tmpFilepath = cceHome + @"tmp\";

        public static int tabIndex = 4;

        public static Size InitialBufferSize;

        public static bool ResponsiveGUI = false;

        #region Color theme

        public static Color DefaultForeground = Color.LightGray;
        public static Color DefaultBackground = Color.Black;

        public static Color SelectedTabBackground = Color.DarkBlue;
        public static Color LineNumbersForeground = Color.DarkGray;

        #endregion
    }
}
