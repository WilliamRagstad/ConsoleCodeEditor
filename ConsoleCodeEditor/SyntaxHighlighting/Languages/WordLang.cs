using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConsoleCodeEditor.SyntaxHighlighting.Languages
{
    public class WordLang : LanguageSyntax<WordLang>
    {
        public WordLang()
        {
            DisplayName = "WordLang";

            // Operations
            RegexRules.Add("[,\\.<]", Color.RoyalBlue);
            RegexRules.Add(">[\x20\t]*\\w+", Color.Lime);
            RegexRules.Add("-[\x20\t]*\\w+", Color.Yellow);

            // Debug
            RegexRules.Add("\\?", Color.Red);

            // Escaped Characters
            RegexRules.Add("\\\\.", Color.Turquoise);

            // Variables
            RegexRules.Add("'[^\']+'", Color.Orange);

            // Conditions
            RegexRules.Add("\\w+!+", Color.Yellow);

            // Comments
            RegexRules.Add("\"[^\"]+\"", Color.DarkGreen);

        }

        public override bool IndentNextLine(string currentLine) => false;

        public override bool IsExecutable() => false;

        public override string ExecutionArguments(string filepath) => "";
    }
}
