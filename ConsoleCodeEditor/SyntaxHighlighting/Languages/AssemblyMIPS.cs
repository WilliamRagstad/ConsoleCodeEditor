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

            // Undefined tokens (raw labels)
            RegexRules.Add(@"\w+", Color.Green);

            // Directives
            RegexRules.Add(@"\.\w+", Color.OrangeRed);

            // Labels
            RegexRules.Add(@"\w+:", Color.SpringGreen);

            // Instruction name
            RegexRules.Add(@"addi|add|addiu|addu|andi|and|beq|bne|jalr|jr|jal|j|lbu|lb|lui|lw|mul|nor|ori|or|sltu|slti|sltiu|slt|sllv|sll|sra|srl|srlv|sb|sw|sub|subu|xor|xori|nop", Color.Cyan);

            // Extra Instructions
            RegexRules.Add(@"syscall", Color.Blue);

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

        public override bool IsExecutable() => false;

        public override string ExecutionArguments(string filepath) => $"";
    }
}
