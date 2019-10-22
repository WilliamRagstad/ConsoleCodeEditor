using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCodeEditor
{
    class Program
    {
        public static Editor.ParentWindow ParentWindow;
        static void Main(string[] args)
        {
            args = new[] { "/new", "123", "/open", "file.txt", "123", "//debug", "true" };
            Arguments arguments = Arguments.Parse(args);
            
            if (arguments.FindPattern("new"))
            {
                // Create a tmp file somewhere and later save/remove it on exit

            }
            if (arguments.FindPattern("open", typeof(string)))
            {

            }

            // Errors
            if (arguments.FindPattern("open")) throw new ArgumentException("Missing file(s) to open!");

            // Show editor
        }
    }
}
