using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCodeEditor
{
    class Program
    {
        public static Component.ParentWindow ParentWindow;
        static void Main(string[] args)
        {
            // Initialize a new parent window
            ParentWindow = new Component.ParentWindow();
            Settings.InitialBufferSize = new System.Drawing.Size(Console.BufferWidth, Console.BufferWidth);

            //args = new[] { "/open", "exampleFile.c", "exampleFile2.py" };
            Arguments arguments = Arguments.Parse(args);

            if (arguments.Length == 0)
            {
                Console.WriteLine(
                    "Usage:\n" +
                    "  /new\t\t\t\tStart editing a new file.\n"+
                    "  /open (file1) [file2] ...\tOpen and edit an existing file.\n\n" +
                    "See more help:\n" +
                    "  https://github.com/WilliamRagstad/ConsoleCodeEditor"
                );
                return;
            }
            
            if (arguments.FindPattern("new"))
            {
                // Create a tmp file somewhere and later save/remove it on exit
                if (arguments["new"].Count == 0)
                {
                    ParentWindow.NewFileEditor();
                }
                else
                {
                    for (int i = 0; i < arguments["new"].Count; i++)
                    {
                        string filepath = arguments["new"][i];
                        string filename = Component.ParentWindow.ParseFileName(filepath);
                        ParentWindow.NewFileEditor(filename, filepath);
                    }
                }
            }
            if (arguments.FindPattern("open"))
            {
                if (arguments["open"].Count == 0)
                {
                    Console.WriteLine("Missing file(s) to open!");
                    return;
                }
                for (int i = 0; i < arguments["open"].Count; i++)
                {
                    string filepath = arguments["open"][i];
                    ParentWindow.OpenFileEditor(filepath);
                }
            }

            // Start editor
            ParentWindow.Start();
        }
    }
}
