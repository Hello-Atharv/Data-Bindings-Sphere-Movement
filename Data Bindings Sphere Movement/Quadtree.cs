using System;
using System.Collections.Generic;
using System.Text;

namespace DataBindingsSphereMovement
{
    public class Quadtree
    {
        private Node root;

        private Vector iHat = new Vector(1, 0);
        private Vector jHat = new Vector(0, 1);

        private const int northWest = 0;
        private const int southWest = 1;
        private const int southEast = 2;
        private const int northEast = 3;

        private const int maxParticles = 2;

        private const double splittingConstGrav = 1;

        private LinkedList<Node> nodes;

        private LinkedList<Node> adjNodes;
        private LinkedList<Node> gravNodes;

        public Quadtree(Vector rootTopLeft, Vector rootBottomRight)
        {
            root = new Node(rootTopLeft, rootBottomRight);
            adjNodes = new LinkedList<Node>();
            gravNodes = new LinkedList<Node>();
        }

        public void BuildQuadtree(List<Particle> particles){
            nodes = new LinkedList<Node>();
            nodes.Add(root);
            BuildingProcess(root, particles);
            AddGravityInfo();
        }

        private void BuildingProcess(Node parentNode, List<Particle> particles)
        {
            Node[] nodesplit = SplitQuadrant(parentNode);
            Tuple<LinkedList<Particle>, bool> particleCheck;

            for(int i = 0; i < nodesplit.Length; i++)
            {
                particleCheck = CheckParticlesInRectangle(nodesplit[i], particles);
                if (particleCheck.Item2)
                {
                    Node node = nodesplit[i];
                    parentNode.Children[i] = node;
                    node.Parent = parentNode;

                    BuildingProcess(node, particles);
                    nodes.Add(node);
                }
                else
                {
                    if (particleCheck.Item1.Length != 0)
                    {
                        nodesplit[i].ContainedParticles = particleCheck.Item1;
                        Particle[] partsInNode = nodesplit[i].ContainedParticles.AllData();
                        foreach(Particle p in partsInNode)
                        {
                            p.QuadtreeNode = nodesplit[i];
                        }
                        parentNode.Children[i] = nodesplit[i];
                        nodesplit[i].Parent = parentNode;
                        nodes.Add(nodesplit[i]);
                    }
                }
            }
        }

        private Tuple<LinkedList<Particle>,bool> CheckParticlesInRectangle(Node node, List<Particle> particles)
        {
            int index = 0;
            bool splitQuadrant = false;

            LinkedList<Particle> partInNode = new LinkedList<Particle>();

            while(index < particles.Count &&  partInNode.Length < maxParticles+1) { 
                if (particles[index].Position.XValue > node.TopLeft.XValue && particles[index].Position.XValue < node.BottomRight.XValue && particles[index].Position.YValue > node.TopLeft.YValue && particles[index].Position.YValue < node.BottomRight.YValue)
                {
                    partInNode.Add(particles[index]);
                }
                index++;
            }

            if(partInNode.Length > maxParticles)
            {
                splitQuadrant = true;
            }
                     
            return new Tuple<LinkedList<Particle>, bool>(partInNode,splitQuadrant);
            
        }

        private Node[] SplitQuadrant(Node parentNode)
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




        public LinkedList<Particle> FindAdjacentParticles(Node node)
        {

            adjNodes = new LinkedList<Node>();
            CollisionTraverse(root, node);

            LinkedList<Particle> adjParticles = new LinkedList<Particle>();
            for(int i = 0; i < adjNodes.Length; i++)
            {
                if (adjNodes.FindDataAtIndex(i) != null)
                {
                    Particle[] partsInNode = adjNodes.FindDataAtIndex(i).ContainedParticles.AllData();
                    foreach (Particle p in adjNodes.FindDataAtIndex(i).ContainedParticles.AllData())
                    {
                        adjParticles.Add(p);
                    }
                }
            }


            return adjParticles;
        }

        private void CollisionTraverse(Node node, Node toCompare)
        {
            foreach(Node n in node.Children)
            {
                if (n != null && toCompare!= null)
                {
                    if (AreIntersecting(n, toCompare))
                    {
                        if (n.ContainedParticles == null)
                        {
                            CollisionTraverse(n, toCompare);
                        }
                        else
                        {
                            adjNodes.Add(n);
                        }
                    }
                }
            }           
        }

        private bool AreIntersecting(Node node1, Node node2)
        {
            bool intersect = false;

            double x1TopL = node1.TopLeft.DotProduct(iHat);
            double y1TopL = node1.TopLeft.DotProduct(jHat);
            double x1BotR = node1.BottomRight.DotProduct(iHat);
            double y1BotR = node1.BottomRight.DotProduct(jHat);
            double x2TopL = node2.TopLeft.DotProduct(iHat);
            double y2TopL = node2.TopLeft.DotProduct(jHat);
            double x2BotR = node2.BottomRight.DotProduct(iHat);
            double y2BotR = node2.BottomRight.DotProduct(jHat);

            if ((y2TopL >= y1TopL && y2TopL <= y1BotR) || (y2BotR >= y1TopL && y2BotR <= y1BotR))
            {
                if ((x2TopL <= x1BotR && x2TopL >= x1TopL) || (x2BotR <= x1BotR && x2BotR >= x1TopL))
                {
                    intersect = true;
                }
            }
            return intersect;
        }

        public double FindNodeMass(Node node)
        {
            double totalMass = 0;
            Particle[] partsInNode = node.ContainedParticles.AllData();
            foreach(Particle p in partsInNode)
            {
                totalMass = totalMass + p.Properties.Mass;
            }

            return totalMass;
        }

        public Vector FindNodeWeightedSum(Node node)
        {
            Vector weightedSum = new Vector(0, 0);
            Particle[] partsInNode = node.ContainedParticles.AllData();
            foreach (Particle p in partsInNode)
            {
                weightedSum.AddVectors(true, p.Position.ScalarMultiply(false, p.Properties.Mass));
            }

            return weightedSum;
        }

        public void AddGravityInfo()
        {
            COMTraverse(root);
        }

        private void COMTraverse(Node node)
        {
            Vector weightedSum = new Vector(0, 0);
            node.NodeMass = 0;
            foreach(Node n in node.Children)
            {
                if (n != null)
                {
                    if (CheckLeafNode(n))
                    {
                        node.NodeMass = node.NodeMass + FindNodeMass(n);
                        weightedSum.AddVectors(true, FindNodeWeightedSum(n));
                        
                    }
                    else
                    {
                        COMTraverse(n);
                        node.NodeMass = node.NodeMass + n.NodeMass;
                        weightedSum.AddVectors(true, n.NodeCOM.ScalarMultiply(false, n.NodeMass));
                    }
                    
                }
            }
            node.NodeCOM = weightedSum.ScalarMultiply(false, 1/node.NodeMass);
        }

        public LinkedList<Node> GlobalGravField(Particle particle)
        {
            gravNodes = new LinkedList<Node>();
            GravTraverse(root, particle);

            return gravNodes;
        }

        private void GravTraverse(Node node, Particle particle)
        {
            if (node != null)
            {
                if (CheckLeafNode(node))
                {
                    gravNodes.Add(node);
                }
                else
                {
                    if (GravTraverseCondition(node, particle)){
                        foreach (Node n in node.Children)
                        {
                            GravTraverse(n, particle);
                        }
                    }
                    else
                    {
                        gravNodes.Add(node);
                    }
                }
            }
        }

        private bool GravTraverseCondition(Node node, Particle particle)
        {
            bool traverseNode = false;
            double sepDistance = particle.Position.SepDistance(node.NodeCOM);

            if(FindIntermediarySize(node)/sepDistance > splittingConstGrav)
            {
                traverseNode = true;
            }

            return traverseNode;
        }

        private double FindIntermediarySize(Node node)
        {
            Vector diagonal = node.BottomRight.SubtractVectors(false, node.TopLeft);
            double size = Math.Abs(diagonal.DotProduct(iHat)) + Math.Abs(diagonal.DotProduct(jHat));
            double intermediarySize = size / 2;

            return intermediarySize;
        }

        public bool CheckLeafNode(Node node)
        {
            bool isLeafNode = false;
            int nullCount = 0;
            foreach (Node n in node.Children)
            {
                if (n == null)
                {
                    nullCount++;
                }
            }
            if (nullCount == node.Children.Length)
            {
                isLeafNode = true;
            }

            return isLeafNode;
        }

        public LinkedList<Node> Nodes
        {
            get { return nodes; }
        }

    }
}
