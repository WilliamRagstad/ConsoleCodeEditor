using System.Diagnostics;
using System;
using Console = Colorful.Console;

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
        private void DrawTitlebar(string title)
        {
            Console.ForegroundColor = Settings.TabForeground;
            int width = Console.WindowWidth;
            string bar = "";
            for (int i = 0; i < width / 2 - title.Length / 2; i++)
            {
                bar += "=";
            }
            Console.ForegroundColor = Settings.TabForeground;
            Console.Write(bar);
            Console.ForegroundColor = Settings.ExecutorMain_Foreground;
            Console.Write(title);
            Console.ForegroundColor = Settings.TabForeground;
            Console.WriteLine(bar);

            Console.ForegroundColor = Settings.DefaultForeground;
        }
        public void Start()
        {
            Console.Clear();
            DrawTitlebar($" Executing {Filename} ");
            // Start the child process.
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(@"C:\Windows\System32\cmd.exe", "/c " + Shellcommand);

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.WorkingDirectory = Filedirectory;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            p.Start();
            p.WaitForExit();

            sw.Stop();

            Console.Write(Environment.NewLine);
            DrawTitlebar(" Statistics ");

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
