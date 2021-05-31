﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ME3ExplorerCore.Packages;

namespace ME3Explorer.SharedUI.Converters
{
    [Localizable(false)]
    public class GameToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string gameStr)
            {
                bool inverted = false;
                if (gameStr.IndexOf('_') > 0)
                {
                    var splitparms = gameStr.Split('_');
                    inverted = splitparms.Any(x => x == "Not");
                    gameStr = splitparms.Last();
                }
                if (Enum.TryParse(gameStr, out MEGame parameterGame))
                {
                    if (inverted ^ parameterGame == (MEGame)value) return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false; //don't need this
        }
    }
}