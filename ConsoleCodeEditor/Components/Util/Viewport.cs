using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCodeEditor.Component.Util
{
    class Viewport<T> where T: IComparable
    {
        public T Top;
        public T Left;
        public T Width;
        public T Height;

        public Viewport(T top, T left, T width, T height)
        {
            Top = top;
            Left = left;
            Width = width;
            Height = height;
        }
    }

    class ConsoleViewport : Viewport<int>
    {
        public ConsoleViewport(int top, int left, int width, int height) : base(top, left, width, height) { }
    }
}
