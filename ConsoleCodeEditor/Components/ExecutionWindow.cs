using System.Diagnostics;
using System.Drawing;
using System;
using Console = Colorful.Console;
using System.IO;

namespace ConsoleCodeEditor.Component
{
    class Executor
    {
        /// <summary>
        /// This class is meant to show the output and
        /// execute the runnable files in a contaminated
        /// environment.
        /// It also provides tools for debugging and
        /// optimizing.
        /// </summary>

        public Executor(string shellcommand, string filename, string filedirectory)
        {
            Shellcommand = shellcommand;
            Filename = filename;
            Filedirectory = filedirectory;
        }

        public string Shellcommand { get; }
        public string Filename { get; }
        public string Filedirectory { get; }
        private void DrawTitlebar(string title, bool newLine = true)
        {
            Console.ForegroundColor = Settings.TabForeground;
            int width = Console.WindowWidth;
            int j = 0;
            for (int i = 0; i < width; i++)
            {
                if (i > width / 2 - title.Length / 2 - 2 && i < width / 2 + title.Length / 2)
                {
                    if (i == width / 2 - title.Length / 2 - 2 + 1)  Console.ForegroundColor = Settings.ExecutorMain_Foreground;
                    if (i == width / 2 + title.Length / 2 - 1)      Console.ForegroundColor = Settings.TabForeground;
                    if (j < title.Length)
                    {
                        Console.Write(title[j]);
                        j++;
                        continue;
                    }
                }
                Console.Write("=");
            }
            if (newLine) Console.Write("\n");
            Console.ForegroundColor = Settings.DefaultForeground;
        }
        public void Start()
        {
            Console.Clear();
            DrawTitlebar($" Executing {Filename} ", false);

            // Start the child process.
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(@"C:\Windows\System32\cmd.exe", "/c " + Shellcommand);

            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;

            // Start statistical meassures
            Stopwatch sw = new Stopwatch();
            sw.Start();
            p.Start();
            StreamWriter input = p.StandardInput;

            p.WaitForExit();
            sw.Stop();




            string output = p.StandardOutput.ReadToEnd();
            string error  = p.StandardError.ReadToEnd();

            Console.ForegroundColor = Settings.DefaultForeground;
            Console.WriteLine(output);

            if (!string.IsNullOrEmpty(error))
            {
                DrawTitlebar(" Errors ");
                System.Console.ForegroundColor = Settings.ExecutorError_Foreground;
                Console.WriteLine(error);
            }

            DrawTitlebar(" Statistics ", false);

            DrawStat("Time", sw.Elapsed.ToString() + $" ({sw.ElapsedMilliseconds} ms)");

            Console.ForegroundColor = Settings.TabForeground;
            Console.Write("\nPress any key to continue...");
            Console.ReadKey(); // Short pause
        }

        private void DrawStat(string key, string value)
        {
            Console.ForegroundColor = Settings.ExecutorStats_Foreground;
            Console.Write(key);
            Console.ForegroundColor = Settings.TabForeground;
            Console.Write(": ");
            Console.ForegroundColor = Settings.ExecutorStatsValue_Foreground;
            Console.WriteLine(value);
        }
    }
}
