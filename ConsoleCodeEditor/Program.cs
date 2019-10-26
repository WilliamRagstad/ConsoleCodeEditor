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
        private static int newTmpFileIndex = 0;
        static void Main(string[] args)
        {
            // Initialize a new parent window
            ParentWindow = new Editor.ParentWindow();
            Settings.InitialBufferSize = new System.Drawing.Size(Console.BufferWidth, Console.BufferWidth);

            args = new[] { "/open", "exampleFile.c", "exampleFile2.py" };
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
                string tmpFile = "untitled-" + newTmpFileIndex;
                newTmpFileIndex++;
                Editor.Editor newFile = new Editor.Editor(tmpFile, Settings.tmpFilepath + tmpFile, SyntaxHighlighting.Languages.PlainText.Instance);
                newFile.AddNewLine();
                ParentWindow.AddEditor(newFile, true);
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
                    string[] fps = filepath.Split('\\');
                    string filename = fps[fps.Length - 1];

                    Editor.Editor openFileEditor = new Editor.Editor(filename, filepath); // This will trigger the autodetect language method
                    openFileEditor.Initialize();
                    ParentWindow.AddEditor(
                        openFileEditor
                    );
                }
            }

            // Start editor
            ParentWindow.Start();
        }
    }
}
