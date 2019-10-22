using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCodeEditor.Editor
{
    class Editor
    {
        /// <summary>
        /// This class is meant to manage file
        /// editing, syntax highlighting, code completion,
        /// line indexes, text typing/moving around in file/console,
        /// mid level input handeling (onSave, onDiscard events etc...),
        /// etc...
        /// </summary>

        public int ID;
        public string Filename;
        public string Filepath;
        public LanguageType LanguageSyntax;

        private string[] contentBuffer;

        public Editor(string filename, string filepath, LanguageType languageSyntax)
        {
            ID = (new Random()).Next();
            Filename = filename;
            Filepath = filepath;
            LanguageSyntax = languageSyntax;
            contentBuffer = new string[] { };
        }

        public Editor(string filename, string filepath)
            : this(filename, filepath, DetectLanguageSyntax(filepath)) { }

        public static LanguageType DetectLanguageSyntax(string filepath) {
            string[] fp = filepath.Split('.');
            string fileExt = fp[fp.Length - 1].ToLower();
            switch(fileExt)
            {
                case "txt": return LanguageType.PlainText;
                case "bat": return LanguageType.Batch;
                case "cmd": return LanguageType.Batch;
                case "c": return LanguageType.C;
                case "cpp": return LanguageType.Cpp;
                case "cs": return LanguageType.CSharp;
                case "css": return LanguageType.CSS;
                case "go": return LanguageType.GO;
                case "svg": return LanguageType.HTML;
                case "html": return LanguageType.HTML;
                case "htm": return LanguageType.HTML;
                case "java": return LanguageType.Java;
                case "class": return LanguageType.Java;
                case "js": return LanguageType.JavaScript;
                case "json": return LanguageType.JSON;
                case "kiw": return LanguageType.KiwiShell;
                case "less": return LanguageType.Less;
                case "lisp": return LanguageType.Lisp;
                case "md": return LanguageType.MarkDown;
                case "php": return LanguageType.PHP;
                case "ps1": return LanguageType.Powershell;
                case "ps2": return LanguageType.Powershell;
                case "py": return LanguageType.Python;
                case "rb": return LanguageType.Ruby;
                case "scss": return LanguageType.Sass;
                case ".sql": return LanguageType.SQL;
                case ".ts": return LanguageType.TypeScript;
                case ".xml": return LanguageType.XML;
                case ".yaml": return LanguageType.YAML;
                default: return LanguageType.PlainText;
            }
        }

        public void Draw()
        {
            Console.SetCursorPosition(0,2);
        }
    }
}
