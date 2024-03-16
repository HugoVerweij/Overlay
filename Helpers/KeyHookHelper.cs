using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Overlay
{
    public class KeyHookHelper
    {
        #region Variables

        // Public.

        public delegate void KeyHookEventHandler(object sender, Keys code);
        public event KeyHookEventHandler OnKeyDown;

        public bool IsRunning { get => _hookID != IntPtr.Zero; }

        // DLL's.

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private readonly LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                OnKeyDown(this, (Keys)vkCode);
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        #endregion

        public KeyHookHelper(MainWindow window)
        {
            // Set the proc.
            _proc = HookCallback;

            // Subscribe to the closing event.
            window.Closing += Exit;
        }

        public void Start()
        {
            // Return if the logger is already running.
            if (IsRunning) return;

            // Set the hook.
            _hookID = SetHook(_proc);
        }

        public void Exit(object sender, CancelEventArgs e)
        {
            // Unhook.
            UnhookWindowsHookEx(_hookID);
        }
    }
}
