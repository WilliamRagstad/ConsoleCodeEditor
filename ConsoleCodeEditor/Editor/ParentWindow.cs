using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ParentWindow() { }

        public List<Editor> Editors;
        public void DrawTabs()
        {
            Console.SetCursorPosition(0,0);
            for (int i = 0; i < Editors.Count; i++)
            {
                Console.Write($"\\{Editors[i]}\\");
            }
            Console.Write('\n');
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write("=");
            }
            Console.Write('\n');
        }

    }
}
