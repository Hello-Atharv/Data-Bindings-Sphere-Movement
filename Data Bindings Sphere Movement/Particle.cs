using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace DataBindingsSphereMovement
{
    class Particle
    {

        private Vector position;
        private Vector velocity;

        private double radius = 8;

        private double mass = 1;

        private Random rand = new Random();

        public Particle(double xCoord, double yCoord, double xVel, double yVel)
        {

            position = new Vector(xCoord, yCoord);
            velocity = new Vector(xVel, yVel);

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

        public double Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public double Mass
        {
            get { return mass; }
            set { mass = value; }
        }


    }
}
