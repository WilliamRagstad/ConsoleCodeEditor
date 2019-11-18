using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCodeEditor
{
    class Program
    {
        public static Component.ParentWindow ParentWindow;

        private static string helpMessage = "Usage:\n" +
                    "  (/new)\t\t\t\tStart editing a new file.\n" +
                    "  (/open) (file1) (file2) ...\tOpen and edit an existing file.\n\n" +
                    "See more help:\n" +
                    "  https://github.com/WilliamRagstad/ConsoleCodeEditor";
        static void Main(string[] args)
        {
            // Initialize a new parent window
            ParentWindow = new Component.ParentWindow();
            Settings.InitialBufferSize = new System.Drawing.Size(Console.BufferWidth, Console.BufferWidth);

            //args = new string[0];
            //args = new[] { "/open" };
            //args = new[] { "/open", @"C:\Users\ewr0327\Desktop\myTestProg.py" };
            //args = new[] { "/open", "exampleFile.c", "exampleFile2.py" };
            //args = new[] { "exampleFile.c", "exampleFile2.py", "/help", "open" };
            Arguments arguments = Arguments.Parse(args);
            
            if (arguments.FindPattern("new") || arguments.Length == 0)
            {
                // Create a tmp file somewhere and later save/remove it on exit
                if (arguments.Length == 0)
                {
                    ParentWindow.NewFileEditor();
                }
                else if (arguments["new"].Count == 0)
                {
                    ParentWindow.NewFileEditor();
                }
                else if(arguments["new"].Count > 0)
                {
                    for (int i = 0; i < arguments["new"].Count; i++)
                    {
                        string filepath = arguments["new"][i];
                        string filename = Component.ParentWindow.ParseFileName(filepath);
                        ParentWindow.NewFileEditor(filename, filepath, Component.Editor.DetectLanguageSyntax(filepath));
                    }
                }
            }
            else // If it's only filepaths passed with or without /open infront
            {
                if (arguments.FindPattern("open"))
                {
                    if (arguments["open"].Count == 0)
                    {
                        Console.WriteLine("Missing file(s) to open!");
                        return;
                    }
                    for (int i = 0; i < arguments["open"].Count; i++)
                    {
                        ParentWindow.OpenFileEditor(arguments["open"][i]);
                    }
                }
                else
                {
                    for (int i = 0; i < arguments.KeylessArguments.Count; i++)
                    {
                        string filename = Component.ParentWindow.ParseFileName(arguments[i]);
                        if (File.Exists(arguments[i])) ParentWindow.OpenFileEditor(arguments[i]);
                        else ParentWindow.NewFileEditor(filename, arguments[i], Component.Editor.DetectLanguageSyntax(filename));
                    }
                }
            }

            // Setup console
            SetupConsole();

            // Start editor
            ParentWindow.Start();
        }

        public static void SetupConsole()
        {
            SetConsoleMode(GetStdHandle(STD_INPUT_HANDLE), (uint)(
                ConsoleInputModes.ENABLE_WINDOW_INPUT |
                ConsoleInputModes.ENABLE_MOUSE_INPUT |
                ConsoleInputModes.ENABLE_EXTENDED_FLAGS
            ));
        }

        #region Low-Level Functions
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        [Flags]
        private enum ConsoleInputModes : uint
        {
            ENABLE_PROCESSED_INPUT = 0x0001,
            ENABLE_LINE_INPUT = 0x0002,
            ENABLE_ECHO_INPUT = 0x0004,
            ENABLE_WINDOW_INPUT = 0x0008,
            ENABLE_MOUSE_INPUT = 0x0010,
            ENABLE_INSERT_MODE = 0x0020,
            ENABLE_QUICK_EDIT_MODE = 0x0040,
            ENABLE_EXTENDED_FLAGS = 0x0080,
            ENABLE_AUTO_POSITION = 0x0100
        }

        [Flags]
        private enum ConsoleOutputModes : uint
        {
            ENABLE_PROCESSED_OUTPUT = 0x0001,
            ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
            ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
            DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
            ENABLE_LVB_GRID_WORLDWIDE = 0x0010
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);
        static int STD_INPUT_HANDLE = -10;
        #endregion
    }
}
