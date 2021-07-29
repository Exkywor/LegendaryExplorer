﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ME3Explorer.Properties;
using ME3ExplorerCore.Unreal;

namespace ME3Explorer.SharedUI.Converters
{
    [ValueConversion(typeof(Property), typeof(Visibility))]
    public class UPropertyColorPickerVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Debug.WriteLine("booltocollapsed: " + ((bool)value == true).ToString());
            if (value is StructProperty sp)
            {
                return (sp.StructType == "Color" || sp.StructType == "LinearColor" && Settings.Default.InterpreterWPF_ShowLinearColorWheel) ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {    // Don't need any convert back
            return null;
        }
    }
}
