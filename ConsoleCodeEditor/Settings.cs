using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCodeEditor
{
    struct Settings
    {
        static string userDirectory = $@"C:\Users\{Environment.UserName}\";
        static string cceHome = userDirectory + @".cce\";
        static string tmpFilepath = cceHome + @"tmp\";
    }
}
