using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleCodeEditor
{
    /// <summary>
    /// This class is no longer used due to usage of different solution.
    /// </summary>

    [Obsolete]
    class KeyHook
    {
        public event KeyEventHandler KeyPress;
        protected virtual void OnKeyPress(KeyEventArgs e) => KeyPress?.Invoke(this, e);
        private Thread _runtime;
        private bool _keepAlive;
        private List<int> _trackKeys;

        public KeyHook()
        {
            _runtime = new Thread(Runtime);
            _runtime.ApartmentState = ApartmentState.STA;
            _trackKeys = new List<int>();

            // Setup console
            
        }
        public void Track(Keys key)
        {
            _trackKeys.Add((int)key);
        }
        public void Track(int key)
        {
            _trackKeys.Add(key);
        }
        public void Hook()
        {
            _keepAlive = true;
            _runtime.Start();
        }
        public void Stop()
        {
            _keepAlive = false;
            _runtime.Join();
        }
        private void Runtime()
        {
            while(_keepAlive)
            {
                // Capture keystrokes
                for (int i = 0; i < _trackKeys.Count; i++)
                {
                    if (GetAsyncKeyState(_trackKeys[i]) != 0)
                    {
                        OnKeyPress(new KeyEventArgs((Keys)_trackKeys[i]));
                    }
                }
            }
        }


        #region Low-Level Functions
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
        // Takes in a constant int that determines which key scan code to return
        // The constants are available here https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
        // You can also use Keys enums i.e. Keys.Left, but they may not match exactly.
        // If the key is currently down, the value is non zero
        // If the key has just been pressed, the first bit will be set to 1 so you can "AND" the rest of the bits away and check if the value equals
        // A more detailed explanation is available here https://www.unknowncheats.me/forum/c-c-c/62440-some-food-brain-all-getasynckeystate.html

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        [Flags]
        private enum ConsoleInputModes : uint
        {
            ENABLE_PROCESSED_INPUT = 0x0001,
            ENABLE_LINE_INPUT = 0x0002,
            ENABLE_ECHO_INPUT = 0x0004,
            ENABLE_WINDOW_INPUT = 0x0008,
            ENABLE_MOUSE_INPUT = 0x0010,
            ENABLE_INSERT_MODE = 0x0020,
            ENABLE_QUICK_EDIT_MODE = 0x0040,
            ENABLE_EXTENDED_FLAGS = 0x0080,
            ENABLE_AUTO_POSITION = 0x0100
        }

        [Flags]
        private enum ConsoleOutputModes : uint
        {
            ENABLE_PROCESSED_OUTPUT = 0x0001,
            ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
            ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
            DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
            ENABLE_LVB_GRID_WORLDWIDE = 0x0010
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);
        static int STD_INPUT_HANDLE = -10;
        #endregion
    }
}
