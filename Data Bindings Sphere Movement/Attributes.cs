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

        private List<Particle> particles;

        public Attributes(double diameter, double mass)
        {
            this.diameter = diameter;
            this.mass = mass;

            particles = new List<Particle>();
        }

        public void ParticleAdded(Particle particle)
        {
            particles.Add(particle);
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

        public List<Particle> Particles
        {
            get { return particles; }
        }

        public int GroupCount
        {
            get { return particles.Count;}
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
