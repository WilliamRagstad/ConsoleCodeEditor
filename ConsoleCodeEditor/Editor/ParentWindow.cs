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
        }

        public List<Editor> Editors;
        private int _currentEditorIndex;
        private Thread _runtime;
        private bool _keepRuntimeAlive;
        public static int TabHeight = 2;
        public static int DockHeight = 2;

        public void AddEditor(Editor editor)
        {
            editor.Parent = this;
            Editors.Add(editor);
        }
        public void DrawTabs()
        {
            Console.SetCursorPosition(0,0);
            for (int i = 0; i < Editors.Count; i++)
            {
                if (i == _currentEditorIndex) Console.BackgroundColor = Settings.SelectedTabBackground;
                Console.Write($" {Editors[i].Filename} ");
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
            Editor cEditor = Editors[_currentEditorIndex];
            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write("=");
            }
            if (!cEditor.FileIsSaved) Console.Write("<!>");
            string clearPrevTextPadding = "    ";
            string text = clearPrevTextPadding + $"ln {cEditor.CursorTop}, col {cEditor.CursorLeft}, enc {cEditor.FileEncoding.HeaderName.ToUpper()}, type {cEditor.LanguageSyntax.DisplayName}";
            Console.CursorLeft = Console.WindowWidth - text.Length - 1;
            Console.Write(text);
        }
        public void Start()
        {
            Draw();
            _keepRuntimeAlive = true;
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
            _keepRuntimeAlive = false;
            _runtime.Join();
            _runtime.Abort(); // Is probably unecessary
            Console.Clear();
        }

        private void _runtimeLoop()
        {
            Editors[_currentEditorIndex].Start();
            while (_keepRuntimeAlive)
            {
                Editors[_currentEditorIndex].Runtime();

                //Thread.Sleep(500); // Some delay for debugging
            }
        }
    }
}
