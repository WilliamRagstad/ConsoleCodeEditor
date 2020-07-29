namespace ConsoleCodeEditor.Components
{
    public class TextSelection
    {
        public LineColumnPair Start;
        public LineColumnPair End;
        public TextSelection(int startLineIndex, int startColumnIndex, int endLineIndex, int endColumnIndex)
        {
            Start = new LineColumnPair(startLineIndex, startColumnIndex);
            End = new LineColumnPair(endLineIndex, endColumnIndex);
        }

        public bool InSelection(int line, int column)
        {
            if (Start.Line < line && line < End.Line) return true;
            else if (line == Start.Line && Start.Column <= column) return true;
            else if (line == End.Line && column < End.Column) return true;
            return false;
        }

        public bool LineHasSelection(int line)
        {
            return Start.Line <= line && line <= End.Line;
        }

        public bool WholeLineIsInSelection(int line)
        {
            return Start.Line < line && line < End.Line;
        }
    }
}