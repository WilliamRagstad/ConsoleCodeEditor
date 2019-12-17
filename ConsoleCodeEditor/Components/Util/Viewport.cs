using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCodeEditor.Component.Util
{

    class ConsoleViewport
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        public ConsoleViewport(int top, int left, uint width, uint height, int offsetX = 0, int offsetY = 0)
        {
            Top = top;
            Left = left;
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public ConsoleViewport() { }


        public bool IsTopInside(int top) => top >= Top + OffsetY && top <= Top + OffsetY + Height;
        public bool IsLeftInside(int left) => left >= Left + OffsetX && left <= Left + OffsetX + Width;
    }
}
