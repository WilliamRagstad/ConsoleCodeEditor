using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCodeEditor.Component.Controls
{
    class Text : BaseControl
    {
        public Text(string message, int x, int y)
        {
            Message = message;
            Size.Width = Message.Length;
            Size.Height = 1;
            Position.X = x;
            Position.Y = y;
        }

        public string Message { get; }

        public override void Draw()
        {
            Console.SetCursorPosition(Position.X - Size.Width / 2, Position.Y);
            Console.Write(Message);
        }
    }
}
