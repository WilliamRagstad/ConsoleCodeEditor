using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Console = Colorful.Console;

namespace ConsoleCodeEditor.Component
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
            FileEncoding = languageSyntax.PreferredEncoding;
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
                    Parent.DrawDock(true);
                    Parent.DrawTabs();
                    Parent.SetTitle();
                }
            }
        }
        public Encoding FileEncoding;
        private List<string> contentBuffer;

        public ParentWindow Parent;

        private void SaveToFile() {
            if (Filepath != null && !string.IsNullOrEmpty(Filepath))
            {
                File.WriteAllLines(Filepath, contentBuffer.ToArray(), FileEncoding);
                FileIsSaved = true;
            }
            else
            {
                // Ask where to save the file
                SaveFileDialog sfd = new SaveFileDialog();
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Filepath = sfd.FileName;
                    Filename = ParentWindow.ParseFileName(sfd.FileName);
                    SaveToFile(); // Save with the new values
                    SyntaxHighlighting.LanguageSyntax prevLanSyntax = LanguageSyntax;
                    LanguageSyntax = DetectLanguageSyntax();
                    if (LanguageSyntax != prevLanSyntax)
                    {
                        Parent.Draw();
                        DrawAllLines();
                    }
                }
                else
                {
                    _fileIsSaved = false;
                }
            }
        }
        private List<string> ReadFileContent(Encoding encoding = null) {
            if (encoding == null) return File.ReadAllLines(Filepath).ToList();
            else return File.ReadAllLines(Filepath, encoding).ToList();
        }
        public int LinesLength => (contentBuffer.Count - 1).ToString().Length;
        public void AddNewLine() => contentBuffer.Add("");
        public static SyntaxHighlighting.LanguageSyntax DetectLanguageSyntax(string filepath) {
            string[] fp = filepath.Split('.');
            string fileExt = fp[fp.Length - 1].ToLower();
            switch(fileExt)
            {
                case "txt": return SyntaxHighlighting.Languages.PlainText.Instance;
                case "an": return SyntaxHighlighting.Languages.AdvancedNote.Instance;
                case "bat": return SyntaxHighlighting.Languages.Batch.Instance;
                case "cmd": return SyntaxHighlighting.Languages.Batch.Instance;
                case "btm": return SyntaxHighlighting.Languages.Batch.Instance;
                case "c": return SyntaxHighlighting.Languages.C.Instance;
                case "cpp": return SyntaxHighlighting.Languages.Cpp.Instance;
                case "cs": return SyntaxHighlighting.Languages.CSharp.Instance;
                /*case "css": return SyntaxHighlighting.Languages.CSS.Instance;
                case "go": return SyntaxHighlighting.Languages.GO.Instance;
                case "svg": return SyntaxHighlighting.Languages.HTML.Instance;
                case "html": return SyntaxHighlighting.Languages.HTML.Instance;
                case "htm": return SyntaxHighlighting.Languages.HTML.Instance;
                */
                case "java": return SyntaxHighlighting.Languages.Java.Instance;
                case "class": return SyntaxHighlighting.Languages.Java.Instance;/*
                case "js": return SyntaxHighlighting.Languages.JavaScript.Instance;
                case "json": return SyntaxHighlighting.Languages.JSON.Instance;*/
                case "kiw": return SyntaxHighlighting.Languages.KiwiShell.Instance;/*
                case "less": return SyntaxHighlighting.Languages.Less.Instance;
                case "lisp": return SyntaxHighlighting.Languages.Lisp.Instance;
                case "md": return SyntaxHighlighting.Languages.MarkDown.Instance;
                case "php": return SyntaxHighlighting.Languages.PHP.Instanc.Instancee;
                case "ps1": return SyntaxHighlighting.Languages.Powershell.Instance;
                case "ps2": return SyntaxHighlighting.Languages.Powershell.Instance;*/
                case "py": return SyntaxHighlighting.Languages.Python.Instance;
                case "py3": return SyntaxHighlighting.Languages.Python.Instance;
                case "pyt": return SyntaxHighlighting.Languages.Python.Instance;
                /*case "rb": return SyntaxHighlighting.Languages.Ruby.Instance;
                case "scss": return SyntaxHighlighting.Languages.Sass.Instance;
                case ".sql": return SyntaxHighlighting.Languages.SQL.Instance;
                case ".ts": return SyntaxHighlighting.Languages.TypeScript.Instance;
                case ".xml": return SyntaxHighlighting.Languages.XML.Instance;
                case ".yaml": return SyntaxHighlighting.Languages.YAML.Instance;*/
                default: return SyntaxHighlighting.Languages.PlainText.Instance;
            }
        }
        public SyntaxHighlighting.LanguageSyntax DetectLanguageSyntax() => DetectLanguageSyntax(Filepath);
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
                    int curLeft = startIndent + match.Index;
                    if (curLeft >= 0 && curLeft < Console.BufferWidth) Console.CursorLeft = curLeft;
                    else break;
                    Console.ForegroundColor = LanguageSyntax.RegexRules.ElementAt(i).Value;
                    Console.Write(match.Value);

                    match = match.NextMatch();
                }
            }
            Console.ForegroundColor = Settings.DefaultForeground;
        }
        public void DrawAllLines(int startIndex = 0)
        {
            for (int i = startIndex; i < contentBuffer.Count; i++) DrawLine(i, false);
        }

        private void ClearLine(int top, int lineLength)
        {
            // Clear the previous line in chunks at the time
            Console.SetCursorPosition(0, ParentWindow.TabHeight + top);
            string chunk = "      ";
            int times = Console.WindowWidth / chunk.Length;
            for (int i = 0; i < times; i++) Console.Write(chunk);
            for (int i = 0; i < Console.WindowWidth - chunk.Length * times; i++) Console.Write(" ");
        }

        private void DrawLine(int index, bool takeUserInput = true)
        {
            ClearLine(index, contentBuffer[index].Length);
            Console.SetCursorPosition(LinesLength - index.ToString().Length, index + ParentWindow.TabHeight);
            Console.ForegroundColor = Settings.LineNumbersForeground;
            Console.Write($"{index}{Settings.LineIndexSeparator} ");
            Console.ForegroundColor = Settings.DefaultForeground;
            if (index < contentBuffer.Count) WriteSyntaxHighlight(contentBuffer[index]);

            if (takeUserInput && CursorTop == index)
            {
                // Cursor is on the current line
                Console.CursorLeft = LinesLength + 2 + CursorLeft;
                ConsoleKeyInfo key = Console.ReadKey();

                ProcessKey(this, index, key);
            }
        }

        public void ProcessKey(object sender, ConsoleKeyInfo key) => ProcessKey(sender, CursorTop, key);
        public void ProcessKey(object sender, int index, ConsoleKeyInfo key)
        {
            if (key.Key == 0) return; // This causes an fun bug (CTRL+M)
            if (key.Modifiers == ConsoleModifiers.Control)
            {
                // Weird character to hide
                Console.CursorLeft -= 1;
                Console.Write(" ");

                switch (key.Key)
                {
                    case ConsoleKey.S:
                        SaveToFile();
                        FileIsSaved = true;
                        return;
                    case ConsoleKey.Backspace:
                        // Remove all spaces
                        contentBuffer[index] = contentBuffer[index].TrimEnd();
                        CursorLeft = contentBuffer[index].Length;
                        return;
                    case ConsoleKey.O:
                        OpenFileDialog openFile = new OpenFileDialog();
                        if (openFile.ShowDialog() == DialogResult.OK)
                        {
                            Parent.OpenFileEditor(openFile.FileName);
                            Parent.SetCurrentEditor(Parent.Editors.Count - 1);
                            Parent.DrawTabs();
                        }
                        return;
                    case ConsoleKey.N:
                        // SOMETHING IS VERY WRONG HERE
                        Parent.NewFileEditor();
                        // Switch to that editor
                        Parent.SetCurrentEditor(Parent.Editors.Count - 1);
                        return;
                    case ConsoleKey.R:
                        // Run file
                        if (LanguageSyntax.IsExecutable())
                        {
                            if (!FileIsSaved)
                            {
                                SaveToFile();
                                FileIsSaved = true;
                            }
                            FileInfo file = new FileInfo(Filepath);
                            Executor executor = new Executor(LanguageSyntax.ExecutionArguments(Filepath), Filename, file.Directory.FullName);
                            executor.Start();

                            Parent.Draw();
                            DrawAllLines();
                        }
                        return;
                    case ConsoleKey.Z:
                        // Undo

                        return;
                    case ConsoleKey.Y:
                        // Redo

                        return;
                    case ConsoleKey.RightArrow:
                        if (CursorLeft == contentBuffer[index].Length && CursorTop < contentBuffer.Count - 1)
                        {
                            CursorTop++;
                            CursorLeft = 0;
                        }
                        else CursorLeft = contentBuffer[index].Length;
                        return;
                    case ConsoleKey.LeftArrow:
                        if (CursorLeft == 0 && CursorTop > 0)
                        {
                            CursorTop--;
                            CursorLeft = contentBuffer[index - 1].Length;
                            DrawLine(index); // Draw the "previous" line
                        }
                        else CursorLeft = 0;
                        return;
                }
                return;
            }
            else if (key.Modifiers == ConsoleModifiers.Alt)
            {
                // Change Tab
                try
                {
                   int tabIndex = int.Parse(key.KeyChar.ToString()) - 1;
                   Program.ParentWindow.SetCurrentEditor(tabIndex);
                }
                catch { }
                return;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                int prevLineIndexLen = LinesLength;
                FileIsSaved = false;
                string leftHandSide = contentBuffer[index].Substring(CursorLeft, contentBuffer[index].Length - CursorLeft);
                contentBuffer[index] = contentBuffer[index].Remove(CursorLeft, contentBuffer[index].Length - CursorLeft);
                bool autoIndent = LanguageSyntax.IndentNextLine(contentBuffer[index]);
                contentBuffer.Insert(CursorTop + 1, (autoIndent ? Settings.Indents : "") + leftHandSide);
                CursorTop++;
                if (autoIndent) CursorLeft = Settings.Indents.Length;
                else CursorLeft = 0;
                if (prevLineIndexLen != LinesLength)
                {
                    Parent.DrawTabs();
                    DrawAllLines();
                }
                else DrawAllLines(CursorTop - 1);
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
                    int prevLinesLength = LinesLength;
                    int lastLineLen = contentBuffer[contentBuffer.Count - 1].Length;
                    CursorLeft = contentBuffer[index - 1].Length;
                    contentBuffer[index - 1] = contentBuffer[index - 1] + contentBuffer[index];
                    contentBuffer.RemoveAt(index);
                    CursorTop--;
                    Parent.DrawTabs();
                    if (prevLinesLength == LinesLength) DrawAllLines(CursorTop);
                    else DrawAllLines();
                    // Clear the previous line in chunks at the time
                    ClearLine(contentBuffer.Count, contentBuffer[index - 1].Length);
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
                int prevLinesLength = LinesLength;
                FileIsSaved = false;
                if (CursorLeft == contentBuffer[index].Length && CursorTop < contentBuffer.Count - 1)
                {
                    contentBuffer[index] = contentBuffer[index] + contentBuffer[index + 1];
                    contentBuffer.RemoveAt(index + 1);
                    if (prevLinesLength != LinesLength)
                    {
                        Parent.DrawTabs();
                        DrawAllLines();
                    }
                    else DrawAllLines(CursorTop);
                    ClearLine(contentBuffer.Count, contentBuffer[index].Length);
                    return;
                }
                if (CursorLeft < contentBuffer[index].Length) contentBuffer[index] = contentBuffer[index].Remove(CursorLeft, 1);
                return;
            }
            if (key.Key == ConsoleKey.Tab)
            {
                FileIsSaved = false;
                contentBuffer[index] = contentBuffer[index].Insert(CursorLeft, Settings.Indents);
                CursorLeft += Settings.Indents.Length;
                return;
            }
            if (key.Key == ConsoleKey.Escape)
            {
                if (Parent.allEditorsSaved())
                {
                    Console.Clear();
                    Environment.Exit(0);
                }
                else
                {
                    // Make sure to save all editors first
                    if (MessageBox.Show("Some files are not saved. Do you wish to exit?", "Exit without saving?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        return;
                    }
                    foreach (Editor e in Parent.Editors)
                    {
                        if (!e._fileIsSaved)
                        {
                            if (MessageBox.Show("Are you sure about discarding all changes to " + e.Filename + "?", "Discard?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                            {
                                // Save file
                                e.SaveToFile();
                            }
                        }
                    }
                    Console.Clear();
                    Environment.Exit(0);
                }
                return;
            }

            contentBuffer[index] = contentBuffer[index].Insert(CursorLeft, key.KeyChar.ToString());
            CursorLeft++;
            FileIsSaved = false;
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
