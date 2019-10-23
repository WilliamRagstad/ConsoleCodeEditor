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

            args = new[] { "/new" };
            Arguments arguments = Arguments.Parse(args);
            
            if (arguments.FindPattern("new"))
            {
                // Create a tmp file somewhere and later save/remove it on exit
                string tmpFile = "untitled-" + newTmpFileIndex;
                newTmpFileIndex++;
                ParentWindow.Editors.Add(
                    new Editor.Editor(tmpFile, Settings.tmpFilepath + tmpFile, SyntaxHighlighting.Languages.C.Instance)
                );
            }
            if (arguments.FindPattern("open", typeof(string)))
            {
                for (int i = 0; i < arguments["open"].Count; i++)
                {
                    string filepath = arguments["open"][i];
                    string[] fps = filepath.Split('\\');
                    string filename = fps[fps.Length - 1];

                    Editor.Editor openFileEditor = new Editor.Editor(filename, filepath); // This will trigger the autodetect language method
                    openFileEditor.Initialize();
                    ParentWindow.Editors.Add(
                        openFileEditor
                    );
                }
            }
            // Errors
            else if (arguments.FindPattern("open")) throw new ArgumentException("Missing file(s) to open!");

            // Start editor
            ParentWindow.Start();
        }
    }
}
