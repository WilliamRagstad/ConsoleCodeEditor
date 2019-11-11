using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConsoleCodeEditor.SyntaxHighlighting.Languages
{
    public class AdvancedNote : LanguageSyntax<AdvancedNote>
    {
        public AdvancedNote()
        {
            DisplayName = "Adv. Note";

            Color listColor = Color.RoyalBlue;

            // Dot lists
            RegexRules.Add(@"\*", listColor);

            // Line lists
            RegexRules.Add(@"-", listColor);

            // Task lists
            RegexRules.Add(@"\[\x20\]", Color.Gray);
            RegexRules.Add(@"\[\?\]", Color.Yellow);
            RegexRules.Add(@"\[!\]", Color.Red);
            RegexRules.Add(@"\[X\]", Color.Lime);
            RegexRules.Add(@"\[O\]", Color.Aqua);

            // Titles
            RegexRules.Add("--[^-]+--", Color.DarkOrange);

            // Sub-titles
            RegexRules.Add(">>.+", Color.Violet);

            // Numbered lists
            RegexRules.Add(@"\d\.", listColor);

        }

        public override bool IndentNextLine(string currentLine) =>
            currentLine.Trim().StartsWith(">>") ||
            (currentLine.Trim().StartsWith("--") && currentLine.Trim().EndsWith("--"));

        public override bool IsExecutable() => false;

        public override string ExecutionArguments(string filepath) => "";
    }
}
