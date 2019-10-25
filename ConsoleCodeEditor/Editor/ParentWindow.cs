using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Console = Colorful.Console;

namespace ConsoleCodeEditor.Editor
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
            if (Settings.ResponsiveGUI) _responsiveGUI = new Thread(_responsiveGUILoop);
        }

        public List<Editor> Editors;
        private int _currentEditorIndex;
        private Thread _runtime;
        private Thread _responsiveGUI;
        private bool _keepRuntimeAlive;
        private bool _keepResponsiveGUIAlive;
        public static int TabHeight = 2;
        public static int DockHeight = 2;
        private static int prevWindowWidth;
        private static int prevWindowHeight;

        public void AddEditor(Editor editor, bool newFile = false)
        {
            editor.Parent = this;
            editor.FileIsSaved = !newFile;
            Editors.Add(editor);
        }
        public void DrawTabs()
        {
            Console.SetCursorPosition(0,0);
            for (int i = 0; i < Editors.Count; i++)
            {
                if (i == _currentEditorIndex) Console.BackgroundColor = Settings.SelectedTabBackground;
                Console.Write($" {Editors[i].Filename  +  (!Editors[i].FileIsSaved ? "*" : "")} ");
                Console.BackgroundColor = Settings.DefaultBackground;
                Console.Write("|");
            }
            Console.Write('\n');
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write("=");
            }
            Console.Write('\n');
        }
        public void DrawDock()
        {
            if (Editors.Count == 0) return;
            Editor cEditor = Editors[_currentEditorIndex];
            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write("=");
            }
            Console.Write('\n');
            if (!cEditor.FileIsSaved) Console.Write("<!>");
            string clearPrevTextPadding = "    ";
            string text = clearPrevTextPadding + $"ln {cEditor.CursorTop}, col {cEditor.CursorLeft}, enc {cEditor.FileEncoding.HeaderName.ToUpper()}, type {cEditor.LanguageSyntax.DisplayName}";
            Console.CursorLeft = Console.WindowWidth - text.Length - 1;
            Console.Write(text);
        }
        public void Start()
        {
            if(Settings.ResponsiveGUI) UpdateGUI();
            Draw();
            _keepRuntimeAlive = true;
            if (Settings.ResponsiveGUI)
            {
                _keepResponsiveGUIAlive = true;
                _responsiveGUI.Start();
            }
            _runtime.Start();
        }

        public void Draw()
        {
            Console.Clear();
            Console.ForegroundColor = Settings.DefaultForeground;
            DrawTabs();
            DrawDock();
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
        public void UpdateGUI()
        {
            if (prevWindowWidth != Console.WindowWidth || prevWindowHeight != Console.WindowHeight )
            {
                Draw();
                Editors[_currentEditorIndex].DrawAllLines();
                prevWindowWidth = Console.WindowWidth;
                prevWindowHeight = Console.WindowHeight;
                if (Settings.ResponsiveGUI)
                {
                    Console.BufferWidth = Console.WindowWidth + 3;
                    Console.BufferHeight = Console.BufferHeight;
                }
            }
        }
    }
}
