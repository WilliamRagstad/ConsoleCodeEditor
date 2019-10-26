﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConsoleCodeEditor.SyntaxHighlighting.Languages
{
    public class Python : LanguageSyntax<Python>
    {
        public Python()
        {
            DisplayName = "Python";
            // Variable names
            RegexRules.Add(@"(\w)+", Color.Lime);

            // Primitive Data Types
            //RegexRules.Add(@"int|float|char|short|long|double|decimal|signed|unsigned", Color.DarkViolet);

            // true / false
            RegexRules.Add(@"true|false", Color.Azure);

            // Statements
            RegexRules.Add(@"def|return|break|continue", Color.YellowGreen);
            RegexRules.Add(@"(do)(?:[\x20\t]*)(?={)", Color.Yellow);
            RegexRules.Add(@"(case)(?:.*)(?=:)", Color.Yellow);
            RegexRules.Add(@"\W(if|else|while|for|switch|in|not)", Color.Yellow);

            // Functions
            RegexRules.Add(@"(\w+)[\x20\t]*(?=\()", Color.RosyBrown);

            // Operators
            RegexRules.Add(@"\+|-|\*|\/|%|;|=", Color.Aqua);
            RegexRules.Add(@"<|>|&&|\|\||!|<=|>=", Color.DarkGray);
            RegexRules.Add(@"\[|\]|\(|\)|{|}", Color.Brown);

            RegexRules.Add(@"(?![^\w])[\d.\-]+x?[\d]*[fd]?", Color.Orange); // Numbers (f,d suffix) (0x0)
            RegexRules.Add("\"([^\"])*\"", Color.Green); // Strings
            RegexRules.Add("\'([^\'])*\'", Color.LightSeaGreen); // Chars

            // Comments
            RegexRules.Add("#.*", Color.DarkGray);
        }
    }
}