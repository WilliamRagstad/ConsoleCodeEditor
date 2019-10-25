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

        private bool _fileIsSaved;
        public bool FileIsSaved
        {
            get
            {
                return _fileIsSaved;
            }
            set
            {
                if (value != _fileIsSaved && Parent != null)
                {
                    _fileIsSaved = value;
                    Parent.Draw();
                    DrawAllLines();
                }
            }
        }
        public Encoding FileEncoding;
        private List<string> contentBuffer;

        public ParentWindow Parent;

        private void SaveToFile() {
            File.WriteAllLines(Filepath, contentBuffer.ToArray(), FileEncoding);
            FileIsSaved = true;
        }
        private List<string> ReadFileContent() => File.ReadAllLines(Filepath, FileEncoding).ToList();
        private int LinesLength => (contentBuffer.Count - 1).ToString().Length;
        public void AddNewLine() => contentBuffer.Add("");
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
            for (int i = 0; i < contentBuffer.Count; i++) DrawLine(i, false);
        }

        private void ClearLine(int top)
        {
            Console.SetCursorPosition(0, top);
            for (int j = 0; j < Console.BufferWidth; j++) Console.Write(" ");
        }

        private void DrawLine(int index, bool takeUserInput = true)
        {
            int row = index + ParentWindow.TabHeight;
            ClearLine(row);
            Console.SetCursorPosition(LinesLength - index.ToString().Length, row);
            Console.ForegroundColor = Settings.LineNumbersForeground;
            Console.Write($"{index}| ");
            Console.ForegroundColor = Settings.DefaultForeground;
            if (index < contentBuffer.Count) WriteSyntaxHighlight(contentBuffer[index]);

            if (takeUserInput && CursorTop == index)
            {
                // Cursor is on the current line
                Console.CursorLeft = LinesLength + 2 + CursorLeft;
                Console.ForegroundColor = Settings.DefaultForeground;
                ConsoleKeyInfo key = Console.ReadKey();

                if (key.Modifiers == ConsoleModifiers.Control)
                {
                    switch(key.Key)
                    {
                        case ConsoleKey.S:
                            SaveToFile();
                            FileIsSaved = true;
                            return;
                    }
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    FileIsSaved = false;
                    string leftHandSide = contentBuffer[index].Substring(CursorLeft, contentBuffer[index].Length - CursorLeft);
                    contentBuffer[index] = contentBuffer[index].Remove(CursorLeft, contentBuffer[index].Length - CursorLeft);
                    contentBuffer.Insert(CursorTop + 1, leftHandSide);
                    CursorTop++;
                    CursorLeft = 0;
                    DrawAllLines();
                    return;
                }
                if (key.Key == ConsoleKey.UpArrow)
                {
                    if (CursorTop > 0)
                    {
                        if (CursorLeft > contentBuffer[index - 1].Length) CursorLeft = contentBuffer[index - 1].Length;
                        CursorTop--;
                        DrawLine(index); // Draw the previous line
                    }
                    return;
                }
                if (key.Key == ConsoleKey.DownArrow)
                {
                    if (CursorTop < contentBuffer.Count - 1)
                    {
                        if (CursorLeft > contentBuffer[index + 1].Length) CursorLeft = contentBuffer[index + 1].Length;
                        CursorTop++;
                        DrawLine(index); // Draw the previous line
                    }
                    return;
                }
                if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (CursorLeft > 0) CursorLeft -= 1;
                    else if (CursorTop > 0 && CursorLeft == 0)
                    {
                        CursorTop--;
                        CursorLeft = contentBuffer[index - 1].Length;
                        DrawLine(index); // Draw the previous line
                    }
                    return;
                }
                if (key.Key == ConsoleKey.RightArrow)
                {
                    if (CursorLeft < contentBuffer[index].Length) CursorLeft += 1;
                    else if (CursorTop + 1 < contentBuffer.Count)
                    {
                        CursorTop++;
                        CursorLeft = 0;
                    }
                    return;
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    FileIsSaved = false;
                    if (CursorLeft == 0 && CursorTop > 0)
                    {
                        CursorLeft = contentBuffer[index - 1].Length;
                        contentBuffer[index - 1] = contentBuffer[index - 1] + contentBuffer[index];
                        contentBuffer.RemoveAt(index);
                        CursorTop--;
                        Parent.Draw();
                        DrawAllLines();
                        return;
                    }
                    if (contentBuffer[index].Length == 0 && CursorTop > 0)
                    {
                        contentBuffer.RemoveAt(index);
                        CursorTop--;
                        Parent.Draw();
                        DrawAllLines();
                        return;
                    }
                    if (CursorLeft > 0) contentBuffer[index] = contentBuffer[index].Remove(CursorLeft - 1, 1);
                    if (CursorLeft > 0) CursorLeft--;
                    return;
                }
                if (key.Key == ConsoleKey.Delete)
                {
                    FileIsSaved = false;
                    if (CursorLeft == contentBuffer[index].Length && CursorTop < contentBuffer.Count - 1)
                    {
                        contentBuffer[index] = contentBuffer[index] + contentBuffer[index + 1];
                        contentBuffer.RemoveAt(index + 1);
                        Parent.Draw();
                        DrawAllLines();
                        return;
                    }
                    if (CursorLeft < contentBuffer[index].Length) contentBuffer[index] = contentBuffer[index].Remove(CursorLeft, 1);
                    return;
                }
                if (key.Key == ConsoleKey.Tab)
                {
                    FileIsSaved = false;
                    for (int i = 0; i < Settings.tabIndex; i++)
                    {
                        contentBuffer[index] = contentBuffer[index].Insert(CursorLeft, " ");
                        CursorLeft++;
                    }
                    return;
                }

                contentBuffer[index] = contentBuffer[index].Insert(CursorLeft, key.KeyChar.ToString());
                CursorLeft++;
                FileIsSaved = false;
            }
        }

        public void Start()
        {
            DrawAllLines();
        }

        public void Runtime()
        {
            DrawLine(CursorTop);
            if (!Parent.UpdateGUI()) Parent.DrawDock(); // If the gui wasn't updated, just update the dock.
        }
        public void Initialize() => contentBuffer = ReadFileContent();
    }
}
