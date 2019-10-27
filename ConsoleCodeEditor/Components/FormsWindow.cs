using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCodeEditor.Component
{
    class FormsWindow
    {
        public List<Controls.BaseControl> Controls;
        public FormsWindow(List<Controls.BaseControl> controls)
        {
            Controls = controls;
        }

        public void Show(bool clearScreen = true)
        {
            if (clearScreen) Console.Clear();
            for(int i = 0; i < Controls.Count; i++)
            {
                Controls[i].Draw();
            }
            if (Controls.Count > 0) Controls[0].HasFocus = true;
        }
    }
}
