using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConsoleCodeEditor.SyntaxHighlighting.Languages
{
    public class CSharp : LanguageSyntax<CSharp>
    {
        public CSharp()
        {
            DisplayName = "C#";
            RegexRules.Add("if", Color.Yellow);
        }

        public override bool IndentNextLine(string currentLine) => C.Instance.IndentNextLine(currentLine);
    }
}
