using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConsoleCodeEditor.Component.Controls
{
    abstract class BaseControl
    {
        public bool HasFocus;
        public Point Position;
        public Size Size;
        public virtual void Draw() { }
    }
}
