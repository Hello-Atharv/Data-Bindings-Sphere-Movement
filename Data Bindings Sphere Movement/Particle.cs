using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace DataBindingsSphereMovement
{
    public class Particle : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
    

        private Vector position;
        private Vector velocity;
        private Attributes properties;
        private Node quadtreeNode;

        private string underCollision = "no";

        private Random rand = new Random();

        public Particle(double xCoord, double yCoord, double xVel, double yVel, Attributes group)
        {

            position = new Vector(xCoord, yCoord);
            velocity = new Vector(xVel, yVel);

            properties = group;
        }

        public Vector Position
        {
            get { return position; }
            set { position = value;}
        }

        public Vector Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Attributes Properties
        {
            get { return properties; }
        }

        public Node QuadtreeNode
        {
            get { return quadtreeNode; }
            set { quadtreeNode = value; }
        }

        public string UnderCollision
        {
            get { return underCollision; }
            set { underCollision = value; OnPropertyChanged("UnderCollision"); }
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
