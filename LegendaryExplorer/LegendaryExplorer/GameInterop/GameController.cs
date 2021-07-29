﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using LegendaryExplorerCore.GameFilesystem;
using LegendaryExplorerCore.Packages;
using Keys = System.Windows.Forms.Keys;

namespace LegendaryExplorer.GameInterop
{
    public static class GameController
    {
        public static string InteropAsiName(MEGame game) => game switch
        {
            MEGame.ME2 => "ZZZ_LEXInteropME2.asi",
            MEGame.ME3 => "LEXInteropME3.asi",
            _ => throw new ArgumentOutOfRangeException(nameof(game), game, null)
        };

        //asis used by ME3Explorer. here so that they can be deleted if found in game's asi dir.
        public static string OldInteropAsiName(MEGame game) => game switch
        {
            MEGame.ME2 => "ME3ExplorerInteropME2.asi",
            MEGame.ME3 => "ME3ExplorerInterop.asi",
            _ => null
        };

        public const string TempMapName = "AAAME3EXPDEBUGLOAD";
        public static bool TryGetMEProcess(MEGame game, out Process meProcess)
        {
            if (game is MEGame.ME2)
            {
                meProcess = Process.GetProcessesByName("ME2Game").FirstOrDefault() ?? Process.GetProcessesByName("MassEffect2").FirstOrDefault();
            }
            else
            {
                meProcess = Process.GetProcessesByName(game switch
                {
                    MEGame.ME1 => "MassEffect",
                    MEGame.ME3 => "MassEffect3",
                    _ => throw new ArgumentOutOfRangeException(nameof(game), game, null)
                }).FirstOrDefault();
            }
            return meProcess != null;
        }

        private static bool hasRegisteredForMessages; 
        public static void InitializeMessageHook(Window window)
        {
            if (hasRegisteredForMessages) return;
            hasRegisteredForMessages = true;
            if (PresentationSource.FromVisual(window) is HwndSource hwndSource)
            {
                hwndSource.AddHook(WndProc);
            }
        }

        public static event Action<string> RecieveME3Message;

        public static event Action<string> RecieveME2Message;

        public static event Action<string> RecieveME1Message;
        public static void SendKey(IntPtr hWnd, Keys key) => SendKey(hWnd, (int)key);

        public static void ExecuteConsoleCommands(MEGame game, params string[] commands)
        {
            switch (game)
            {
                case MEGame.ME2:
                    ExecuteME2ConsoleCommands(commands.AsEnumerable());
                    break;
                case MEGame.ME3:
                    ExecuteME3ConsoleCommands(commands.AsEnumerable());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(game), game, null);
            }
        }

        public static void ExecuteME3ConsoleCommands(params string[] commands) => ExecuteME3ConsoleCommands(commands.AsEnumerable());
        public static void ExecuteME3ConsoleCommands(IEnumerable<string> commands)
        {
            if (TryGetMEProcess(MEGame.ME3, out Process me3Process))
            {
                ExecuteConsoleCommands(me3Process.MainWindowHandle, MEGame.ME3, commands);
            }
        }


        public static void ExecuteME2ConsoleCommands(params string[] commands) => ExecuteME2ConsoleCommands(commands.AsEnumerable());
        public static void ExecuteME2ConsoleCommands(IEnumerable<string> commands)
        {
            if (TryGetMEProcess(MEGame.ME2, out Process me2Process))
            {
                ExecuteConsoleCommands(me2Process.MainWindowHandle, MEGame.ME2, commands);
            }
        }

        public static bool SendME3TOCUpdateMessage()
        {
            if (TryGetMEProcess(MEGame.ME3, out Process me3Process))
            {
                const uint ME3_TOCUPDATE = 0x8000 + 'T' + 'O' + 'C';
                return SendMessage(me3Process.MainWindowHandle, ME3_TOCUPDATE, 0, 0);
            }

            return false;
        }

        public static void ExecuteConsoleCommands(IntPtr hWnd, MEGame game, params string[] commands) => ExecuteConsoleCommands(hWnd, game, commands.AsEnumerable());
        public static void ExecuteConsoleCommands(IntPtr hWnd, MEGame game, IEnumerable<string> commands)
        {
            const string execFileName = "me3expinterop";
            string execFilePath = Path.Combine(MEDirectories.GetDefaultGamePath(game), "Binaries", execFileName);
            File.WriteAllText(execFilePath, string.Join(Environment.NewLine, commands));
            DirectExecuteConsoleCommand(hWnd, $"exec {execFileName}");
        }

        //private

        #region Internal support functions
        [StructLayout(LayoutKind.Sequential)]
        struct COPYDATASTRUCT
        {
            public ulong dwData;
            public uint cbData;
            public IntPtr lpData;
        }
        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_COPYDATA = 0x004a;
            const uint SENT_FROM_ME3 = 0x02AC00C2;
            const uint SENT_FROM_ME2 = 0x02AC00C3;
            const uint SENT_FROM_ME1 = 0x02AC00C4;
            if (msg == WM_COPYDATA)
            {
                COPYDATASTRUCT cds = Marshal.PtrToStructure<COPYDATASTRUCT>(lParam);
                switch (cds.dwData)
                {
                    case SENT_FROM_ME3:
                    {
                        string value = Marshal.PtrToStringUni(cds.lpData);
                        handled = true;
                        RecieveME3Message?.Invoke(value);
                        return (IntPtr)1;
                    }
                    case SENT_FROM_ME2:
                    {
                        string value = Marshal.PtrToStringUni(cds.lpData);
                        handled = true;
                        RecieveME2Message?.Invoke(value);
                        return (IntPtr)1;
                    }
                    case SENT_FROM_ME1:
                    {
                        string value = Marshal.PtrToStringUni(cds.lpData);
                        handled = true;
                        RecieveME1Message?.Invoke(value);
                        return (IntPtr)1;
                    }
                }
            }

            return IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        const int WM_SYSKEYDOWN = 0x0104;

        private static void SendKey(IntPtr hWnd, int key) => PostMessage(hWnd, WM_SYSKEYDOWN, key, 0);

        /// <summary>
        /// Executes a console command on the game whose window handle is passed.
        /// <param name="command"/> can ONLY contain [a-z0-9 ] 
        /// </summary>
        /// <param name="gameWindowHandle"></param>
        /// <param name="command"></param>
        private static void DirectExecuteConsoleCommand(IntPtr gameWindowHandle, string command)
        {
            SendKey(gameWindowHandle, Keys.Tab);
            foreach (char c in command)
            {
                if (characterMapping.TryGetValue(c, out Keys key))
                {
                    SendKey(gameWindowHandle, key);
                }
                else
                {
                    throw new ArgumentException("Invalid characters!", nameof(command));
                }
            }
            SendKey(gameWindowHandle, Keys.Enter);
        }

        static readonly Dictionary<char, Keys> characterMapping = new()
        {
            ['a'] = Keys.A,
            ['b'] = Keys.B,
            ['c'] = Keys.C,
            ['d'] = Keys.D,
            ['e'] = Keys.E,
            ['f'] = Keys.F,
            ['g'] = Keys.G,
            ['h'] = Keys.H,
            ['i'] = Keys.I,
            ['j'] = Keys.J,
            ['k'] = Keys.K,
            ['l'] = Keys.L,
            ['m'] = Keys.M,
            ['n'] = Keys.N,
            ['o'] = Keys.O,
            ['p'] = Keys.P,
            ['q'] = Keys.Q,
            ['r'] = Keys.R,
            ['s'] = Keys.S,
            ['t'] = Keys.T,
            ['u'] = Keys.U,
            ['v'] = Keys.V,
            ['w'] = Keys.W,
            ['x'] = Keys.X,
            ['y'] = Keys.Y,
            ['z'] = Keys.Z,
            ['0'] = Keys.D0,
            ['1'] = Keys.D1,
            ['2'] = Keys.D2,
            ['3'] = Keys.D3,
            ['4'] = Keys.D4,
            ['5'] = Keys.D5,
            ['6'] = Keys.D6,
            ['7'] = Keys.D7,
            ['8'] = Keys.D8,
            ['9'] = Keys.D9,
            [' '] = Keys.Space,
        };

        #endregion
    }
}
