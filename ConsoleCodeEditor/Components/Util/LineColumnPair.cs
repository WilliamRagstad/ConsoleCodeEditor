using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCodeEditor.Components
{
   public class LineColumnPair
    {
        public LineColumnPair(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public int Line { get; }
        public int Column { get; }
    }
}
