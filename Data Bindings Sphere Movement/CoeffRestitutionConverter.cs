using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace DataBindingsSphereMovement 
{
    class CoeffRestitutionConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string pos = value.ToString();

                if(pos.Length == 1) //A single digit
                {
                    pos = pos + ".0";
                }
                else
                {
                    pos = pos.Substring(0, 3);
                }

                return pos;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
