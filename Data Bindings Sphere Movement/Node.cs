using System;
using System.Collections.Generic;
using System.Text;

namespace DataBindingsSphereMovement
{
    public class Node
    {

        private Vector topLeft;
        private Vector bottomRight;

        private Node parent;
        private Node[] children = new Node[4];

        private LinkedList<Particle> containedParticle;

        private double nodeMass;
        private Vector nodeCOM;

        public Node(Vector topLeft, Vector bottomRight){
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
        }

        public Vector TopLeft
        {
            get { return topLeft; }
            set { topLeft = value; }
        }

        public Vector BottomRight
        {
            get { return bottomRight; }
            set { bottomRight = value; }
        }

        public Node Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public Node[] Children
        {
            get { return children; }
            set { children = value; }
        }

        public LinkedList<Particle> ContainedParticles
        {
            get { return containedParticle; }
            set { containedParticle = value; }
        }

        public double NodeMass
        {
            get { return nodeMass; }
            set { nodeMass = value; }
        }

        public Vector NodeCOM
        {
            get { return nodeCOM; }
            set { nodeCOM = value; }
        }


    }
}
