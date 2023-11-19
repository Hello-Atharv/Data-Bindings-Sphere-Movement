﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DataBindingsSphereMovement
{
    class Node
    {

        private Vector topLeft;
        private Vector bottomRight;

        private Node parent;
        private Node[] children = new Node[4];

        private Particle containedParticle;

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

        public Particle ContainedParticle
        {
            get { return containedParticle; }
            set { containedParticle = value; }
        }


    }
}
