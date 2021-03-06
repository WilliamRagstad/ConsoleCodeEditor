﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsoleCodeEditor.Components;
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

        public TextSelection Selection;

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

        public void SaveToFile() {
            if (Filepath != null && !string.IsNullOrEmpty(Filepath))
            {
                string[] lines = contentBuffer.ToArray();
                File.WriteAllLines(Filepath, lines, FileEncoding);
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
            switch (fileExt)
            {
                case "txt": return SyntaxHighlighting.Languages.PlainText.Instance;
                case "an": return SyntaxHighlighting.Languages.AdvancedNote.Instance;
                case "asm": return SyntaxHighlighting.Languages.AssemblyMIPS.Instance;
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
                case "w": return SyntaxHighlighting.Languages.WordLang.Instance;
                case "wl": return SyntaxHighlighting.Languages.WordLang.Instance;
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
            for (int i = startIndex; i < contentBuffer.Count; i++)
            {
                int relativeTop = i + ParentWindow.TabHeight;
                if (ParentWindow.Viewport.IsTopInside(relativeTop)) DrawLine(i, false);
                else if (i - ParentWindow.Viewport.OffsetY > 0)
                {
                    // Move offset down
                    ParentWindow.Viewport.OffsetY++;
                    DrawAllLines();
                }
                else if (i + ParentWindow.Viewport.OffsetY < 0)
                {
                    ParentWindow.Viewport.OffsetY--;
                    DrawAllLines();
                }
            }
        }

        private bool ClearLine(int top, int lineLength)
        {
            if (ParentWindow.TabHeight + top >= Console.BufferHeight) return false;
            // Clear the previous line in chunks at the time
            Console.SetCursorPosition(0, ParentWindow.TabHeight + top);
            string chunk = "      ";
            int times = Console.WindowWidth / chunk.Length;
            for (int i = 0; i < times; i++) Console.Write(chunk);
            for (int i = 0; i < Console.WindowWidth - chunk.Length * times; i++) Console.Write(" ");
            return true;
        }

        private void DrawLine(int index, bool takeUserInput = true)
        {
            if (!ClearLine(index, contentBuffer[index].Length)) return;
            int lineStartIndex = LinesLength - index.ToString().Length + 3;
            Console.SetCursorPosition(LinesLength - index.ToString().Length, index + ParentWindow.TabHeight);
            Console.ForegroundColor = Settings.LineNumbersForeground;
            Console.Write($"{index}{Settings.LineIndexSeparator} ");
            Console.ForegroundColor = Settings.DefaultForeground;

            if (Selection != null)
            {
                if (Selection.WholeLineIsInSelection(index))
                {
                    Console.ForegroundColor = Settings.SelectionForeground;
                    Console.BackgroundColor = Settings.SelectionBackground;
                    Console.CursorLeft = lineStartIndex;
                    Console.WriteLine(contentBuffer[index]);
                }
                else if (Selection.LineHasSelection(index))
                {
                    WriteSyntaxHighlight(contentBuffer[index]);
                    Console.ForegroundColor = Settings.SelectionForeground;
                    Console.BackgroundColor = Settings.SelectionBackground;
                    string selectedText = "";

                    if (index == Selection.Start.Line && index == Selection.End.Line)
                    {
                        if (Selection.End.Column - Selection.Start.Column != 0)
                        {
                            if (Selection.End.Column > Selection.Start.Column)
                            {
                                selectedText = contentBuffer[index].Substring(Selection.Start.Column, Selection.End.Column - Selection.Start.Column);
                                Console.CursorLeft = lineStartIndex + Selection.Start.Column;
                            }
                            else if (Selection.End.Column < Selection.Start.Column)
                            {
                                selectedText = contentBuffer[index].Substring(Selection.End.Column, Selection.Start.Column - Selection.End.Column);
                                Console.CursorLeft = lineStartIndex + Selection.End.Column;
                            }
                        }
                    }
                    else if (index == Selection.Start.Line)
                    {
                        Console.CursorLeft = lineStartIndex + Selection.Start.Column;
                        selectedText = contentBuffer[index].Substring(Selection.Start.Column);
                    }
                    else if (index == Selection.End.Line) // Formailia
                    {
                        Console.CursorLeft = lineStartIndex;
                        if (Selection.End.Column <= contentBuffer[index].Length)
                            selectedText = contentBuffer[index].Substring(0, Selection.End.Column);
                    }

                    Console.Write(selectedText);
                }
                Console.ForegroundColor = Settings.DefaultForeground;
                Console.BackgroundColor = Settings.DefaultBackground;
            }
            else if (index < contentBuffer.Count) WriteSyntaxHighlight(contentBuffer[index]);

            if (takeUserInput && CursorTop == index)
            {
                // Cursor is on the current line
                Console.CursorLeft = LinesLength + 2 + CursorLeft;
                ConsoleKeyInfo key = Console.ReadKey();

                ProcessKey(this, index, key);
            }
        }

        enum CharType
        {
            Word,
            Space,
            Special
        }

        public void ProcessKey(object sender, ConsoleKeyInfo key) => ProcessKey(sender, CursorTop, key);
        public void ProcessKey(object sender, int index, ConsoleKeyInfo key)
        {
            if (key.Key == 0) return; // This causes an fun bug (CTRL+M)
            if (key.Modifiers == ConsoleModifiers.Control)
            {
                // Weird character to hide
                if (Console.CursorLeft > 0)
                {
                    Console.CursorLeft -= 1;
                    Console.Write(" ");
                }
                bool returnAfterSwitch = true;

                switch (key.Key)
                {
                    case ConsoleKey.S:
                        SaveToFile();
                        return;
                    case ConsoleKey.Backspace:

                        //contentBuffer[index] = contentBuffer[index].TrimEnd();
                        if (CursorLeft == 0) { returnAfterSwitch = false; break; }
                        int removeStart = CursorLeft;

                        CharType rmt = CharType.Special; // Default
                        if (char.IsLetter(contentBuffer[index][CursorLeft-1])) rmt = CharType.Word;
                        else if (char.IsWhiteSpace(contentBuffer[index][CursorLeft-1])) rmt = CharType.Space;

                        if (rmt == CharType.Special)
                        {
                            contentBuffer[index] = contentBuffer[index].Remove(CursorLeft - 1, 1);
                            CursorLeft = contentBuffer[index].Length;
                            FileIsSaved = false;
                            return;
                        }

                        for (int i = CursorLeft - 1; i >= 0; i--)
                        {
                            if ((char.IsWhiteSpace(contentBuffer[index][i]) && rmt == CharType.Space) ||
                                (char.IsLetter(contentBuffer[index][i]) && rmt == CharType.Word))
                                removeStart = i;
                            else
                                break;
                        }
                        contentBuffer[index] = contentBuffer[index].Remove(removeStart, CursorLeft - removeStart);
                        CursorLeft = removeStart;
                        FileIsSaved = false;
                        return;
                    case ConsoleKey.O:
                        OpenFileDialog openFile = new OpenFileDialog();
                        openFile.Multiselect = true;
                        if (openFile.ShowDialog() == DialogResult.OK)
                        {
                            for (int i = 0; i < openFile.FileNames.Length; i++)
                            {
                                Parent.OpenFileEditor(openFile.FileNames[i]);
                                Parent.SetCurrentEditor(Parent.Editors.Count - 1);
                            }

                            if (!FileIsSaved && Filename == Settings.NewFileName && contentBuffer.Count == 1 && string.IsNullOrEmpty(contentBuffer[0].Trim()))
                            {
                                Parent.Editors.Remove(this);
                                Parent.SetCurrentEditor(0);
                            }

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
                    case ConsoleKey.V:
                        string rawText = Clipboard.GetText();
                        string[] texts = rawText.Replace("\r","").Replace("\t", Settings.TabSize).Split('\n');

                        // Extract all the text that's on the right side of the cursor
                        string rhsText = "";
                        if (CursorLeft < contentBuffer[CursorTop].Length)
                        {
                            rhsText = contentBuffer[CursorTop].Substring(CursorLeft);
                            contentBuffer[CursorTop] = contentBuffer[CursorTop].Remove(CursorLeft);
                        }

                        int j;
                        for (j = 0; j < texts.Length; j++)
                        {
                            if (j == 0)
                                contentBuffer[CursorTop] = contentBuffer[CursorTop].Insert(CursorLeft, texts[j]);
                            else
                                contentBuffer.Insert(CursorTop + j, texts[j]);
                        }
                        j--;

                        CursorLeft = contentBuffer[CursorTop + j].Length;
                        contentBuffer[CursorTop + j] += rhsText;
                        CursorTop = CursorTop + j;
                        DrawAllLines();
                        return;
                    case ConsoleKey.Z:
                        // Undo

                        return;
                    case ConsoleKey.Y:
                        // Redo

                        return;
                    case ConsoleKey.RightArrow:
                        if (CursorLeft == contentBuffer[index].Length)
                        {
                            if (CursorTop < contentBuffer.Count - 1)
                            {
                                CursorTop++;
                                CursorLeft = 0;
                            }
                        }
                        else
                        {
                            CharType jct = CharType.Special; // Default
                            if (char.IsLetter(contentBuffer[index][CursorLeft])) jct = CharType.Word;
                            else if (char.IsWhiteSpace(contentBuffer[index][CursorLeft])) jct = CharType.Space;

                            if (jct == CharType.Special)
                            {
                                if (CursorLeft < contentBuffer[index].Length)
                                    CursorLeft++;
                                return;
                            }

                            for (int i = CursorLeft; i < contentBuffer[index].Length; i++)
                            {
                                char c = contentBuffer[index][i];
                                if ((char.IsWhiteSpace(c) && jct == CharType.Space) ||
                                    (char.IsLetter(c) && jct == CharType.Word))
                                    CursorLeft++;
                                else
                                    return;
                            }
                        }
                        return;
                    case ConsoleKey.LeftArrow:
                        if (CursorLeft == 0)
                        {
                            if (CursorTop > 0)
                            {
                                CursorTop--;
                                CursorLeft = contentBuffer[index - 1].Length;
                                DrawLine(index); // Draw the "previous" line
                            }
                        }
                        else
                        {
                            CharType jct = CharType.Special; // Default
                            if (char.IsLetter(contentBuffer[index][CursorLeft-1])) jct = CharType.Word;
                            else if (char.IsWhiteSpace(contentBuffer[index][CursorLeft-1])) jct = CharType.Space;

                            if (jct == CharType.Special)
                            {
                                if (CursorLeft > 0)
                                    CursorLeft--;
                                return;
                            }

                            for (int i = CursorLeft-1; i >= 0; i--)
                            {
                                char c = contentBuffer[index][i];
                                if ((char.IsWhiteSpace(c) && jct == CharType.Space) ||
                                    (char.IsLetter(c) && jct == CharType.Word))
                                    CursorLeft--;
                                else
                                    return;
                            }
                        }
                        return;
                        // Pass through
                    case ConsoleKey.Enter:
                    case ConsoleKey.Delete:
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.UpArrow:
                        returnAfterSwitch = false;
                        break;
                }
                if (returnAfterSwitch) return;
            }

            if (key.Modifiers == ConsoleModifiers.Alt)
            {
                if (char.IsDigit(key.KeyChar))
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
                else if (key.Key == ConsoleKey.F4)
                {
                    // Prompt to Close current tab or close editor
                    Parent.CloseCurrentTab();
                }
            }

            bool isControlChar = false;

            if (key.Key == ConsoleKey.Enter)
            {
                int prevLineIndexLen = LinesLength;
                FileIsSaved = false;
                string leftHandSide = contentBuffer[index].Substring(CursorLeft, contentBuffer[index].Length - CursorLeft);
                contentBuffer[index] = contentBuffer[index].Remove(CursorLeft, contentBuffer[index].Length - CursorLeft);
                bool autoIndent = LanguageSyntax.IndentNextLine(contentBuffer[index]);
                contentBuffer.Insert(CursorTop + 1, (autoIndent ? Settings.TabSize : "") + leftHandSide);
                CursorTop++;
                if (autoIndent) CursorLeft = Settings.TabSize.Length;
                else CursorLeft = 0;
                if (prevLineIndexLen != LinesLength)
                {
                    Parent.DrawTabs();
                    DrawAllLines();
                }
                else DrawAllLines(CursorTop - 1);
                isControlChar = true;
            }
            else if (key.Key == ConsoleKey.UpArrow)
            {
                if (CursorTop > 0)
                {
                    if (CursorLeft > contentBuffer[index - 1].Length) CursorLeft = contentBuffer[index - 1].Length;
                    CursorTop--;
                    DrawLine(index); // Draw the previous line
                }
                isControlChar = true;
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                if (CursorTop < contentBuffer.Count - 1)
                {
                    if (CursorLeft > contentBuffer[index + 1].Length) CursorLeft = contentBuffer[index + 1].Length;
                    CursorTop++;
                    DrawLine(index); // Draw the previous line
                }
                isControlChar = true;
            }
            else if (key.Key == ConsoleKey.LeftArrow)
            {
                if (CursorLeft > 0) CursorLeft -= 1;
                else if (CursorTop > 0 && CursorLeft == 0)
                {
                    CursorTop--;
                    CursorLeft = contentBuffer[index - 1].Length;
                    DrawLine(index); // Draw the previous line
                }
                isControlChar = true;
            }
            else if (key.Key == ConsoleKey.RightArrow)
            {
                if (CursorLeft < contentBuffer[index].Length) CursorLeft += 1;
                else if (CursorTop + 1 < contentBuffer.Count)
                {
                    CursorTop++;
                    CursorLeft = 0;
                    DrawLine(index);
                }
                isControlChar = true;
            }
            else if (key.Key == ConsoleKey.Backspace)
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
                    isControlChar = true;
                }
                if (index < contentBuffer.Count)
                {
                    if (CursorTop > 0 && contentBuffer[index].Length == 0)
                    {
                        contentBuffer.RemoveAt(index);
                        CursorTop--;
                        Parent.Draw();
                        DrawAllLines();
                        isControlChar = true;
                    }
                    if (CursorLeft > 0)
                    {
                        contentBuffer[index] = contentBuffer[index].Remove(CursorLeft - 1, 1);
                        CursorLeft--;
                    }
                }
                
                isControlChar = true;
            }
            else if (key.Key == ConsoleKey.Delete)
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
                isControlChar = true;
            }
            else if (key.Key == ConsoleKey.Tab)
            {
                FileIsSaved = false;
                contentBuffer[index] = contentBuffer[index].Insert(CursorLeft, Settings.TabSize);
                CursorLeft += Settings.TabSize.Length;
                isControlChar = true;
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                Parent.ExitProgram();
                isControlChar = true;
            }
            else if (key.Key == ConsoleKey.PageUp)
            {
                CursorLeft = 0;
                CursorTop = 0;
                DrawLine(index); // Draw previous line
                isControlChar = true;
            }
            else if (key.Key == ConsoleKey.PageDown)
            {
                CursorLeft = contentBuffer[contentBuffer.Count - 1].Length;
                CursorTop = contentBuffer.Count - 1;
                DrawLine(index); // Draw previous line
                isControlChar = true;
            }

            bool CanClearSelection = true;
            if (key.Modifiers == ConsoleModifiers.Shift)
            {
                // Change selection if is a combination with arrow keys
                switch (key.Key)
                {
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.RightArrow:
                        if (Selection == null)
                        {
                            if (key.Key == ConsoleKey.LeftArrow)
                                Selection = new TextSelection(CursorTop, CursorLeft, index, CursorLeft+1);
                            if (key.Key == ConsoleKey.RightArrow)
                                Selection = new TextSelection(CursorTop, CursorLeft-1, index, CursorLeft);
                        }
                        else
                        {
                            // Add support for negative selections (backwards)
                            if (key.Key == ConsoleKey.LeftArrow)
                                Selection.End = new LineColumnPair(CursorTop, CursorLeft+1);
                            if (key.Key == ConsoleKey.RightArrow)
                                Selection.End = new LineColumnPair(CursorTop, CursorLeft);
                        }
                        CanClearSelection = false;
                        DrawLine(index);
                        break;
                }
            }
            if (CanClearSelection && Selection != null)
            {
                Selection = null;
                DrawAllLines();
            }

            if (!isControlChar)
            {
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
            if (!Parent.UpdateGUI()) Parent.DrawDock(true); // If the gui wasn't updated, just update the dock.
        }
        public void Initialize() {
            contentBuffer = ReadFileContent();
            for (int i = 0; i < contentBuffer.Count; i++)
            {
                contentBuffer[i] = contentBuffer[i].Replace("\t", Settings.TabSize);
            }
        }
    }
}
