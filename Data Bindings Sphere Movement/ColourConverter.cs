﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DataBindingsSphereMovement
{
    class ColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            object pos;

            SolidColorBrush colour = new SolidColorBrush((Color)ColorConverter.ConvertFromString((string)value));
            pos = colour;

            return pos;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
