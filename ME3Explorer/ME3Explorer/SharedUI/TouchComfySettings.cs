﻿using System.ComponentModel;

namespace ME3Explorer.SharedUI
{
    public class TouchComfySettings
    {
        public static int TreeViewMargin => Properties.Settings.Default.TouchComfyMode ? 5 : 2;
        public static int InterpreterWPFNodeMargin => Properties.Settings.Default.TouchComfyMode ? 3 : 1;

        private static readonly PropertyChangedEventArgs TreeViewMarginEventArgs = new PropertyChangedEventArgs(nameof(TreeViewMargin));
        private static readonly PropertyChangedEventArgs InterpreterWPFNodeMarginEventArgs = new PropertyChangedEventArgs(nameof(InterpreterWPFNodeMargin));
        public static event PropertyChangedEventHandler StaticPropertyChanged;

        static TouchComfySettings()
        {
            // Set up an empty event handler
            StaticPropertyChanged += (sender, e) => { return; };
        }

        internal static void ModeSwitched()
        {
            StaticPropertyChanged?.Invoke(null, TreeViewMarginEventArgs);
            StaticPropertyChanged?.Invoke(null, InterpreterWPFNodeMarginEventArgs);
        }
    }
}
