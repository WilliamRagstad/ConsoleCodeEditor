using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCodeEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new[] { "/new", "123", "/open", "file.txt", "123", "//debug", "true" };
            Arguments arguments = Arguments.Parse(args);

            if (arguments.Contains("new"))
            {
                // Create a tmp file somewhere and later save/remove it on exit
                return;
            }
            if (arguments.Contains("open"))
            {
                if (arguments["open"].Count > 0)
                {

                }
                else
                {
                    throw new ArgumentException("Missing file to open!");
                }
            }
        }
    }
}
