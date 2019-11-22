using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConsoleCodeEditor.SyntaxHighlighting.Languages
{
    public class KiwiShell : LanguageSyntax<KiwiShell>
    {
        public KiwiShell()
        {
            DisplayName = "KiwiShell";
            PreferredEncoding = Encoding.ASCII;
            RegexRules = Batch.Instance.RegexRules; // Inherit from Batch

            // Primitive Data Types
            RegexRules.Add(@"string|number|bool", Color.DarkViolet);

            // true / false
            RegexRules.Add(@"true|false", Color.Azure);

            // Statements
            RegexRules.Add(@"set|goto|if|else", Color.Yellow);

            // Operators
            RegexRules.Add(@"\+|-|\*|\/|%|;|=", Color.Aqua);
            RegexRules.Add(@"<|>|&&|\|\||!|<=|>=", Color.DarkGray);
            RegexRules.Add(@"\[|\]|\(|\)|{|}", Color.Brown);

            // Loops
            RegexRules.Add(@":\w+(\[.*\])?", Color.RoyalBlue);
            RegexRules.Add(@"(?::)\w+(?=\[.*\])", Color.AliceBlue);

            // Comments
            RegexRules.Add("//.*", Color.DarkGray);
        }

        public override bool IndentNextLine(string currentLine) => false;

        public override bool IsExecutable() => false;

        public override string ExecutionArguments(string filepath) => "";
    }
}
