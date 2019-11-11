using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConsoleCodeEditor.SyntaxHighlighting.Languages
{
    public class Cpp : LanguageSyntax<Cpp>
    {
        public Cpp()
        {
            DisplayName = "C++";

            RegexRules = C.Instance.RegexRules; // Inherit all regex rules from C
        }

        public override bool IndentNextLine(string currentLine) => C.Instance.IndentNextLine(currentLine);

        public override bool IsExecutable() => false;

        public override string ExecutionArguments(string filepath) => "";
    }
}
