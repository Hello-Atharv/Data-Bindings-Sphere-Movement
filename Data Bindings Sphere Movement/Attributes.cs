using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace DataBindingsSphereMovement
{
    public class Attributes : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private double diameter;
        private double mass;

        private int groupCount;

        public Attributes(double diameter, double mass)
        {
            this.diameter = diameter;
            this.mass = mass;

            groupCount = 0;
        }

        public void ParticleAdded()
        {
            groupCount++;
            OnPropertyChanged("GroupCount");
        }

        public double Diameter
        {
            get { return diameter; }
            set { diameter = value; OnPropertyChanged("Diameter"); }
        }
        public double Mass
        {
            get { return mass; }
            set { mass = value; OnPropertyChanged("Mass"); }
        }


        public int GroupCount
        {
            get { return groupCount;}
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
