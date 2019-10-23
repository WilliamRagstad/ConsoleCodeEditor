using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Console = Colorful.Console;

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
        public Editor(string filename, string filepath, SyntaxHighlighting.LanguageSyntax languageSyntax)
        {
            ID = (new Random()).Next();
            Filename = filename;
            Filepath = filepath;
            LanguageSyntax = languageSyntax;
            contentBuffer = new List<string>();
            FileEncoding = Encoding.UTF8;
            FileIsSaved = true;
            CursorLeft = 0;
            CursorTop = 0;
        }

        public Editor(string filename, string filepath)
            : this(filename, filepath, DetectLanguageSyntax(filepath)) { }

        public int ID;
        public string Filename;
        private string Filepath;
        public SyntaxHighlighting.LanguageSyntax LanguageSyntax;

        public int CursorTop; // Relative to the parent window
        public int CursorLeft;// -||-

        public bool FileIsSaved;
        public Encoding FileEncoding;
        private List<string> contentBuffer;

        private void SaveToFile() => File.WriteAllLines(Filepath, contentBuffer.ToArray(), FileEncoding);
        private List<string> ReadFileContent() => File.ReadAllLines(Filepath, FileEncoding).ToList();
        private int LinesLength => contentBuffer.Count.ToString().Length;

        public static SyntaxHighlighting.LanguageSyntax DetectLanguageSyntax(string filepath) {
            string[] fp = filepath.Split('.');
            string fileExt = fp[fp.Length - 1].ToLower();
            switch(fileExt)
            {
                case "txt": return SyntaxHighlighting.Languages.PlainText.Instance;
                //case "bat": return SyntaxHighlighting.Languages.Batch.Instance;
                //case "cmd": return SyntaxHighlighting.Languages.Batch.Instance;
                case "c": return SyntaxHighlighting.Languages.C.Instance;
                case "cpp": return SyntaxHighlighting.Languages.Cpp.Instance;
                case "cs": return SyntaxHighlighting.Languages.CSharp.Instance;
                /*case "css": return SyntaxHighlighting.Languages.CSS.Instance;
                case "go": return SyntaxHighlighting.Languages.GO.Instance;
                case "svg": return SyntaxHighlighting.Languages.HTML.Instance;
                case "html": return SyntaxHighlighting.Languages.HTML.Instance;
                case "htm": return SyntaxHighlighting.Languages.HTML.Instance;
                case "java": return SyntaxHighlighting.Languages.Java.Instance;
                case "class": return SyntaxHighlighting.Languages.Java.Instance;
                case "js": return SyntaxHighlighting.Languages.JavaScript.Instance;
                case "json": return SyntaxHighlighting.Languages.JSON.Instance;
                case "kiw": return SyntaxHighlighting.Languages.KiwiShell.Instance;
                case "less": return SyntaxHighlighting.Languages.Less.Instance;
                case "lisp": return SyntaxHighlighting.Languages.Lisp.Instance;
                case "md": return SyntaxHighlighting.Languages.MarkDown.Instance;
                case "php": return SyntaxHighlighting.Languages.PHP.Instanc.Instancee;
                case "ps1": return SyntaxHighlighting.Languages.Powershell.Instance;
                case "ps2": return SyntaxHighlighting.Languages.Powershell.Instance;
                case "py": return SyntaxHighlighting.Languages.Python.Instance;
                case "rb": return SyntaxHighlighting.Languages.Ruby.Instance;
                case "scss": return SyntaxHighlighting.Languages.Sass.Instance;
                case ".sql": return SyntaxHighlighting.Languages.SQL.Instance;
                case ".ts": return SyntaxHighlighting.Languages.TypeScript.Instance;
                case ".xml": return SyntaxHighlighting.Languages.XML.Instance;
                case ".yaml": return SyntaxHighlighting.Languages.YAML.Instance;*/
                default: return SyntaxHighlighting.Languages.PlainText.Instance;
            }
        }
        public void WriteSyntaxHighlight(string text)
        {
            if (text == null) return;
            int startIndent = Console.CursorLeft;
            for (int i = 0; i < LanguageSyntax.RegexRules.Count; i++)
            {
                string pattern = LanguageSyntax.RegexRules.ElementAt(i).Key;
                Regex regex = new Regex(pattern);
                Match match = regex.Match(text);
                while (match.Success)
                {
                    Console.CursorLeft = startIndent + match.Index;
                    Console.ForegroundColor = LanguageSyntax.RegexRules.ElementAt(i).Value;
                    Console.Write(match.Value);

                    match = match.NextMatch();
                }
            }
            Console.ForegroundColor = Settings.DefaultForeground;
        }
        public void DrawAllLines()
        {
            for (int i = 0; i < contentBuffer.Count + 1; i++) DrawLine(i);
            contentBuffer.Add(Console.ReadLine());
        }

        private void DrawLine(int index)
        {
            Console.SetCursorPosition(0, index + ParentWindow.TabHeight);
            for (int j = 0; j < LinesLength - index.ToString().Length; j++) Console.Write(" ");
            Console.Write($"{index}| ");
            if (index < contentBuffer.Count - 1) WriteSyntaxHighlight(contentBuffer[index]);
        }

        public void Update()
        {

        }

        public void Run()
        {
            Update();
            DrawAllLines();
        }
        public void Initialize() => contentBuffer = ReadFileContent();
    }
}
