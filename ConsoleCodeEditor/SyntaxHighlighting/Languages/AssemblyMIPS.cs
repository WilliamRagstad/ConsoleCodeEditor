using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConsoleCodeEditor.SyntaxHighlighting.Languages
{
    public class AssemblyMIPS : LanguageSyntax<AssemblyMIPS>
    {
        public AssemblyMIPS()
        {
            DisplayName = "MIPS ASM";
            PreferredEncoding = Encoding.ASCII;

            // Undefined tokens (raw labels)
            RegexRules.Add(@"\w+", Color.Green);

            // Directives
            RegexRules.Add(@"\.\w+", Color.OrangeRed);

            // Labels
            RegexRules.Add(@"\w+:", Color.SpringGreen);

            // Instruction name
            RegexRules.Add(@"addi|add|addiu|addu|andi|and|beq|bne|jalr|jr|jal|j|lbu|lb|lui|lw|mul|nor|ori|or|sltu|slti|sltiu|slt|sllv|sll|sra|srl|srlv|sb|sw|sub|subu|xor|xori", Color.Cyan);

            // Extra Instructions & psuedo instructions
            RegexRules.Add(@"syscall|la|li|ble|move|nop", Color.Blue);

            // Numbers
            RegexRules.Add(@"0x[0-9abcdefABCDEF]+|(-?([0-9])+(\.[0-9]+)?)", Color.Orange);

            // Strings
            RegexRules.Add("\"([^\"])*\"", Color.Orange);

            // Registers
            RegexRules.Add(@"\$\w+", Color.Yellow);

            // Comments
            RegexRules.Add("#.*", Color.DarkGray);
        }

        public override bool IndentNextLine(string currentLine) => currentLine.EndsWith(":") || currentLine.StartsWith(".") || currentLine.StartsWith(Settings.TabSize);

        public override bool IsExecutable() => true;

        // http://courses.missouristate.edu/kenvollmar/mars/Help/Help_4_1/MarsHelpCommand.html
        public override string ExecutionArguments(string filepath) => $"java -jar \"{AppDomain.CurrentDomain.BaseDirectory + @"\CompilerCollection\"}Mars.jar\" \"{filepath}\" hex nc 10000 v0 v1 a0 a1 a2 a3 t0 t1 t2 t3 t4 t5 t6 t7 t8 t9 s0 s1 s2 s3 s4 s5 s6 s7 ra";
    }
}
