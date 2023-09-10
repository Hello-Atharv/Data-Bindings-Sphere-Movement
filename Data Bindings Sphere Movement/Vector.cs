using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace DataBindingsSphereMovement
{
    class Vector : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private double xValue;
        private double yValue;

        public Vector(double xValue, double yValue)
        {
            this.xValue = xValue;
            this.yValue = yValue;
        }

        public double XValue
        {
            get { return xValue; }
            set { xValue = value; OnPropertyChanged("XValue"); }
        }

        public double YValue
        {
            get { return yValue; }
            set { yValue = value; OnPropertyChanged("YValue"); }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
