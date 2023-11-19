using System;
using System.Collections.Generic;
using System.Text;

namespace DataBindingsSphereMovement
{
    class Quadtree
    {
        private Node root;

        private Vector iHat = new Vector(1, 0);
        private Vector jHat = new Vector(0, 1);

        private const int northWest = 0;
        private const int southWest = 1;
        private const int southEast = 2;
        private const int northEast = 3;

        private List<Node> nodes;

        public Quadtree(Vector rootTopLeft, Vector rootBottomRight)
        {
            root = new Node(rootTopLeft, rootBottomRight);
            
        }

        public void BuildQuadtree(List<Particle> particles){
            nodes = new List<Node>();
            nodes.Add(root);
            BuildingProcess(root, ref particles);
        }

        private void BuildingProcess(Node parentNode, ref List<Particle> particles)
        {
            Node[] nodesplit = splitQuadrant(parentNode);
            Tuple<Particle, bool> particleCheck;

            for(int i = 0; i < nodesplit.Length; i++)
            {
                particleCheck = CheckParticlesInRectangle(nodesplit[i], ref particles);
                if (particleCheck.Item2)
                {
                    BuildingProcess(nodesplit[i], ref particles);
                    nodes.Add(nodesplit[i]);
                }
                else if(particleCheck.Item1 != null)
                {
                    nodesplit[i].ContainedParticle = particleCheck.Item1;
                    parentNode.Children[i] = nodesplit[i];
                    nodesplit[i].Parent = parentNode;
                    nodes.Add(nodesplit[i]);
                }
            }
        }

        private Tuple<Particle,bool> CheckParticlesInRectangle(Node node, ref List<Particle> particles)
        {
            int no_particles = 0;
            int index = 0;
            bool splitQuadrant = false;

            Particle particle = null;

            while(index < particles.Count && no_particles < 2) { 
                if (particles[index].Position.XValue > node.TopLeft.XValue && particles[index].Position.XValue < node.BottomRight.XValue && particles[index].Position.YValue > node.TopLeft.YValue && particles[index].Position.YValue < node.BottomRight.YValue)
                {
                    no_particles++;
                }
                index++;
            }

            if (no_particles == 1)
            {
                particle = particles[index - 1];
            }
            else if(no_particles >= 2){
                splitQuadrant = true;
            }

            return new Tuple<Particle, bool>(particle,splitQuadrant);
            
        }

        private Node[] splitQuadrant(Node parentNode)
        {
            Node[] nodes = new Node[4];
            Vector diagonal = parentNode.BottomRight.SubtractVectors(false, parentNode.TopLeft);

            Vector deltaIHat = new Vector(Math.Abs(diagonal.DotProduct(iHat))/2, 0);
            Vector deltaJHat = new Vector(0, Math.Abs(diagonal.DotProduct(jHat)) / 2);

            nodes[northWest] = new Node(parentNode.TopLeft, parentNode.BottomRight.SubtractVectors(false, deltaIHat.AddVectors(false, deltaJHat)));
            nodes[southWest] = new Node(parentNode.TopLeft.AddVectors(false, deltaJHat), parentNode.BottomRight.SubtractVectors(false, deltaIHat));
            nodes[southEast] = new Node(parentNode.TopLeft.AddVectors(false, deltaIHat.AddVectors(false, deltaJHat)), parentNode.BottomRight);
            nodes[northEast] = new Node(parentNode.TopLeft.AddVectors(false,deltaIHat), parentNode.BottomRight.SubtractVectors(false,deltaJHat));

            return nodes;
        }

        public List<Node> Nodes
        {
            get { return nodes; }
        }

    }
}
