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
            // Variable names
            RegexRules.Add(@"(\w)+", Color.Lime);

            // Rules
            RegexRules.Add(@"@\w+(?=(\x20\w+))", Color.Orange);

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

            RegexRules.Add(@"-?\d+(.\d+)?", Color.Orange); // Numbers (f,d suffix) (0x0)
            RegexRules.Add("\"([^\"])*\"", Color.Green); // Strings

            // Loops
            RegexRules.Add(@":\w+(\[.*\])?", Color.RoyalBlue);
            RegexRules.Add(@"(?::)\w+(?=\[.*\])", Color.AliceBlue);

            // variables
            RegexRules.Add(@"%\w+%", Color.PeachPuff);

            // Comments
            RegexRules.Add("//.*", Color.DarkGray);
        }

        public override bool IndentNextLine(string currentLine) => false;

        public override bool IsExecutable() => false;

        public override string ExecutionArguments(string filepath) => "";
    }
}
