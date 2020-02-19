using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConsoleCodeEditor.SyntaxHighlighting.Languages
{
    public class Batch : LanguageSyntax<Batch>
    {
        public Batch()
        {
            DisplayName = "Batch";
            PreferredEncoding = Encoding.ASCII;
            // Variable names
            RegexRules.Add(@"(\w)+", Color.SpringGreen);

            // Rules
            RegexRules.Add(@"@\w+([\x20\t]+\w+)*", Color.Orange);

            // Statements
            RegexRules.Add(@"set|goto|if|else|do", Color.Yellow);

            // Operators
            RegexRules.Add(@"\+|-|\*|\/|%|=|<|>|&&|\|\||!|<=|>=", Color.Aqua);

            // call to executables - parameter values
            RegexRules.Add(@"(-[A-Za-z]\w*)([\x20\t]+[^-\n]+)*", Color.Yellow);
            RegexRules.Add(@"-[A-Za-z]\w*", Color.Magenta);

            RegexRules.Add(@"-?\d+(.\d+)?", Color.Orange); // Numbers (f,d suffix) (0x0)
            RegexRules.Add("[\"']([^\"])*[\"']", Color.Green); // Strings

            // Set value
            RegexRules.Add(@"(?<=set[\x20\t]+\w+[\x20\t]*=[\x20\t]*).*", Color.Fuchsia);

            // Loops
            RegexRules.Add(@":\w+", Color.RoyalBlue);

            // variables
            RegexRules.Add(@"%\w+%", Color.RoyalBlue);

            // Comments
            RegexRules.Add("[rR][eE][mM] .*", Color.DarkGray);
        }

        public override bool IndentNextLine(string currentLine) => false;

        public override bool IsExecutable() => true;

        public override string ExecutionArguments(string filepath) => $"\"{filepath}\"";
    }
}
