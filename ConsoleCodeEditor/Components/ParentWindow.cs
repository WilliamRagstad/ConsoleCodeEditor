using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsoleCodeEditor.Component.Util;
using Console = Colorful.Console;

namespace ConsoleCodeEditor.Component
{
    class ParentWindow
    {
        /// <summary>
        /// This class is meant to contain all editor windows
        /// and manage tabs, file structure tree (if so),
        /// top level input handeling (onExit, onOpen, onNew events etc...),
        /// top level mouse handeling (onMove, onClick, onHover etc...),
        /// etc...
        /// </summary>
        public ParentWindow() {
            Editors = new List<Editor>();
            _currentEditorIndex = 0;
            _runtime = new Thread(_runtimeLoop);
            //_runtime.ApartmentState = ApartmentState.STA;
            _runtime.SetApartmentState(ApartmentState.STA);
            if (Settings.ResponsiveGUI) _responsiveGUI = new Thread(_responsiveGUILoop);

            Viewport = new ConsoleViewport();
            UpdateViewport();
        }

        public List<Editor> Editors;
        public static int TabHeight = 2;
        public static int DockHeight = 2;
        public static ConsoleViewport Viewport;
        private int _currentEditorIndex;
        private Thread _runtime;
        private Thread _responsiveGUI;
        private bool _keepRuntimeAlive;
        private bool _keepResponsiveGUIAlive;
        private static int _prevWindowWidth;
        private static int _prevWindowHeight;

        public void OpenFileEditor(string filepath)
        {
            string filename = ParseFileName(filepath);
            OpenFileEditor(filename, filepath);
        }
        public static string ParseFileName(string filepath)
        {
            string[] fps = filepath.Split('\\');
            string filename = fps[fps.Length - 1];
            return filename;
        }
        public void OpenFileEditor(string filename, string filepath)
        {
            Editor editor = new Editor(filename, filepath);
            editor.Initialize();
            AddEditor(editor);
        }
        public void NewFileEditor(string filename, string filepath, SyntaxHighlighting.LanguageSyntax language)
        {
            Editor newFile = new Editor(filename, filepath, language);
            newFile.AddNewLine();
            newFile.FileIsSaved = false;
            AddEditor(newFile, true);
        }
        public void NewFileEditor() => NewFileEditor("Untitled", null, SyntaxHighlighting.Languages.PlainText.Instance);
        public bool allEditorsSaved()
        {
            for (int i = 0; i < Editors.Count; i++)
            {
                if (!Editors[i].FileIsSaved) return false;
            }
            return true;
        }

        public void SetCurrentEditor(int index)
        {
            if (index < 0) index += 10; // Keyboard numbers can't exceed 10. (0-9)
            if (index < Editors.Count)
            {
                _currentEditorIndex = index;
                SetTitle();
                Draw();
                Editors[_currentEditorIndex].DrawAllLines();
            }
        }
        public void AddEditor(Editor editor, bool newFile = false)
        {
            editor.Parent = this;
            editor.FileIsSaved = !newFile;
            Editors.Add(editor);

            UpdateViewport();
        }
        public void DrawTabs()
        {
            Console.SetCursorPosition(0,0);
            for (int i = 0; i < Editors.Count; i++)
            {
                Console.ForegroundColor = Settings.TabForeground;
                if (i == _currentEditorIndex)
                {
                    Console.BackgroundColor = Settings.SelectedTabBackground;
                    Console.ForegroundColor = Settings.SelectedTabForeround;
                }
                Console.Write($" {Editors[i].Filename}");
                if (!Editors[i].FileIsSaved)
                {
                    Console.ForegroundColor = Settings.UnsavedChangesNotification_Tab_Foreground;
                    Console.Write(Settings.UnsavedChangesNotification_Tab);
                    Console.ForegroundColor = Settings.TabForeground;
                }
                Console.Write(" ");
                Console.BackgroundColor = Settings.DefaultBackground;
                Console.ForegroundColor = Settings.TabForeground;
                Console.Write("|");
            }

            // Clear the rest of the line
            while (Console.CursorLeft < Console.WindowWidth - 1) Console.Write(" ");

            Console.Write('\n');
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                if (Editors.Count > 0 && i == Editors[_currentEditorIndex].LinesLength) Console.Write(Settings.LineIndex_tab_Separator);
                else Console.Write(Settings.TabSeparator);
            }
            Console.Write('\n');
        }
        public void DrawDock(bool fullRedraw = false)
        {
            if (Editors.Count == 0) return;
            Editor cEditor = Editors[_currentEditorIndex];
            if (_prevWindowWidth != Console.WindowWidth || fullRedraw)
            {
                Console.SetCursorPosition(0, Console.WindowHeight - 2);
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.Write("=");
                }
                if (Console.WindowWidth != Console.BufferWidth) Console.Write('\n');

                // Clear the rest of the line
                while (Console.CursorLeft < Console.WindowWidth - 1) Console.Write(" ");
                Console.CursorLeft = 0;

            }
            else
            {
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
            }
            if (!cEditor.FileIsSaved)
            {
                Console.ForegroundColor = Settings.UnsavedChangesNotification_Dock_Foreground;
                Console.Write("<!>");
                Console.ForegroundColor = Settings.DefaultForeground;
            }
            string clearPrevTextPadding = "    "; // Placeholder!!!

            Console.ForegroundColor = Settings.DefaultForeground;
            string languageName = cEditor.LanguageSyntax.DisplayName;
            string text = clearPrevTextPadding + $"ln {cEditor.CursorTop}, col {cEditor.CursorLeft}, enc {cEditor.FileEncoding.HeaderName.ToUpper()}, type {languageName}";
            Console.CursorLeft = Console.WindowWidth - text.Length - 1;
            Console.Write(text);
            // Re-write language name but in color
            Console.CursorLeft = Console.WindowWidth - languageName.Length - 1;
            if (cEditor.LanguageSyntax.IsExecutable()) Console.ForegroundColor = Settings.DockCurrentLanguage_Foreground;
            Console.Write(languageName);
        }
        
        private void UpdateViewport()
        {
            int tabTextLen = 0;
            for (int i = 0; i < Editors.Count; i++)
            {
                tabTextLen += $" {Editors[i].Filename} |".Length;
            }
            Viewport.Top = tabTextLen / Console.WindowWidth + TabHeight;
            Viewport.Left = 0;
            Viewport.Width = (uint)Console.WindowWidth;
            Viewport.Height = (uint)( Console.WindowHeight - Viewport.Top - DockHeight );
            // throw new NotImplementedException();
        }

        public void Start()
        {
            if(Settings.ResponsiveGUI) UpdateGUI();
            SetTitle();
            Draw();
            _keepRuntimeAlive = true;
            if (Settings.ResponsiveGUI)
            {
                _keepResponsiveGUIAlive = true;
                _responsiveGUI.Start();
            }
            _runtime.Start();

            // _lowLevelKeyHook.Hook();
        }

        public void Draw()
        {
            Console.Clear();
            Console.ForegroundColor = Settings.DefaultForeground;
            DrawTabs();
            DrawDock(true);
        }

        public void Stop()
        {
            _keepResponsiveGUIAlive = false;
            _keepRuntimeAlive = false;
            _runtime.Join();
            _runtime.Abort(); // Is probably unecessary
            _responsiveGUI.Join();
            _responsiveGUI.Abort();
            Console.Clear();
        }
        [STAThread]
        private void _runtimeLoop()
        {
            Editors[_currentEditorIndex].Start();
            while (_keepRuntimeAlive)
            {
                Editors[_currentEditorIndex].Runtime();
            }
        }

        private void _responsiveGUILoop()
        {
            while(_keepResponsiveGUIAlive)
            {
                UpdateGUI();
                Thread.Sleep(10);
            }
        }
        public bool UpdateGUI()
        {
            if (_prevWindowWidth != Console.WindowWidth || _prevWindowHeight != Console.WindowHeight )
            {
                Draw();
                Editors[_currentEditorIndex].DrawAllLines();
                _prevWindowWidth = Console.WindowWidth;
                _prevWindowHeight = Console.WindowHeight;
                if (Settings.ResponsiveGUI)
                {
                    Console.BufferWidth = Console.WindowWidth + 3;
                    Console.BufferHeight = Console.BufferHeight;
                }
                return true;
            }
            return false;
        }

        public void SetTitle()
        {
            string title = "CCE";
            string titlebuffer = title; // Initialize to title

            if (Editors.Count > 0)
            {
                string currentFile = Editors[_currentEditorIndex].Filename;
                if (!Editors[_currentEditorIndex].FileIsSaved) currentFile += Settings.UnsavedChangesNotification_Tab;

                int padding = (Console.WindowWidth * 2 - title.Length) / 2 - currentFile.Length / 2;
                for (int i = 0; i < padding; i++) titlebuffer += " ";
                titlebuffer += currentFile;
            }
            Console.Title = titlebuffer;
        }
    }
}
