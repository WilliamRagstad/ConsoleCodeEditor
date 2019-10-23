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

        public static Color DefaultForeground = Color.LightGray;
        public static Color DefaultBackground = Color.Black;

        #region Color theme

        public static Color TabBackground = Color.DarkBlue;

        #endregion
    }
}
