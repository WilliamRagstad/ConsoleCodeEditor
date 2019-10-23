using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConsoleCodeEditor.SyntaxHighlighting.Languages
{
    public class PlainText : LanguageSyntax<PlainText>
    {
        public PlainText() { DisplayName = "Plain Text";  }
    }
}
