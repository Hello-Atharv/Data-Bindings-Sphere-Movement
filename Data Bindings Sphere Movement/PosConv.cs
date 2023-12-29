using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DataBindingsSphereMovement
{
    public class PosConv : IValueConverter
    {
        private double radius;

        public PosConv(double diameter)
        {
            radius = diameter/2;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            object pos;
            
            pos = (object)((double) value+radius);   
            
            return pos;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}