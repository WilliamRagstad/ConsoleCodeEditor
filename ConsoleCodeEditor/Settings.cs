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

        public static string Indents = "    ";
        public static bool AutoIndent = true;

        public static Size InitialBufferSize; // Unuseful..?

        public static bool ResponsiveGUI = false;

        #region Color theme

        public static Color DefaultForeground = Color.LightGray;
        public static Color DefaultBackground = Color.Black;

        public static Color TabForeground = Color.Gray;
        public static Color SelectedTabBackground = Color.DarkBlue;
        public static Color SelectedTabForeround = Color.White;
        public static Color LineNumbersForeground = Color.Gray;

        public static Color UnsavedChangesNotification_Dock_Foreground = Color.Yellow;
        public static Color UnsavedChangesNotification_Tab_Foreground = Color.Yellow;

        public static Color DockCurrentLanguage_Foreground = Color.DarkViolet;

        public static Color ExecutorMain_Foreground = Color.Yellow;
        public static Color ExecutorStats_Foreground = Color.Lime;
        public static Color ExecutorStatsValue_Foreground = Color.Yellow;
        public static ConsoleColor ExecutorError_Foreground = ConsoleColor.Magenta;

        #endregion

        #region Skin

        public static char TabSeparator = '=';
        public static char LineIndex_tab_Separator = '#';
        public static char LineIndexSeparator = '|';
        public static string UnsavedChangesNotification_Tab = " *";
        public static string UnsavedChangesNotification_Dock = "<!>";

        #endregion
    }
}
