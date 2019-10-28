using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCodeEditor.SyntaxHighlighting
{
    public abstract class LanguageSyntax
    {
        public string DisplayName;
        public Dictionary<string, System.Drawing.Color> RegexRules;
        public abstract bool IndentNextLine(string currentLine);
        public LanguageSyntax()
        {
            DisplayName = "[Unknown]";
            RegexRules = new Dictionary<string, System.Drawing.Color>();
            RegexRules.Add(".+", Settings.DefaultForeground); // Default color.
        }
    }
    public abstract class LanguageSyntax<T> : LanguageSyntax where T : new()
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null) _instance = new T();
                return _instance;
            }
        }
    }
}
