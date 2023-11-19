using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace DataBindingsSphereMovement
{
    class Attributes : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
    
        private double radius;
        private double mass;

        public Attributes(double radius, double mass)
        {
            this.radius = radius;
            this.mass = mass;
        }

        public double Radius
        {
            get { return radius; }
            set { radius = value; OnPropertyChanged("Radius"); }
        }
        public double Mass
        {
            get { return mass; }
            set { mass = value; OnPropertyChanged("Mass"); }
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
