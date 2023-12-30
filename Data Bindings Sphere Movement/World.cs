using System;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Collections.Generic;

namespace DataBindingsSphereMovement
{
    public class World : INotifyPropertyChanged
    {
        private Random rand = new Random();

        Vector iHat = new Vector(1, 0);
        Vector jHat = new Vector(0, 1);

        private const double perimeterWidth = 1475.2;
        private const double perimeterHeight = 803.2;
        private const double offset = 30;

        Quadtree quadtree = new Quadtree(new Vector(offset, offset), new Vector(offset + perimeterWidth, offset + perimeterHeight));

        private double uniformGravStrength = 2;
        private bool uniformGravEnabled = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Particle> particles;
        private Dictionary<int, Attributes> attributesDictionary;
        private int particleGroupCount = 0;

        private int groupSelected;

        private double tempCoeff = 5.12; //Value chosen because it gives the inital temperature to be 20 degrees C (room temperature)
        private const double tempMultiplier = 0.333;

        private double coeffRestitution = 1;

        private const double gravConstant = 10;

        public World()
        {
            particles = new ObservableCollection<Particle>();
            attributesDictionary = new Dictionary<int, Attributes>();

            Vector topLeft = new Vector(offset, offset);
            Vector bottomRight = new Vector(offset + perimeterWidth, offset + perimeterHeight);

            groupSelected = 1;
        }

        public ObservableCollection<Particle> AllParticles
        {
            get
            {
                return particles;
            }
        }

        public int ParticleCount
        {
            get { return particles.Count; }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void AddParticle(double xPos, double yPos)
        {
            double xVel = rand.NextDouble() * 100;
            double yVel = rand.NextDouble() * 100;

            if (rand.Next(0, 2) == 1)
            {
                xVel = -1 * xVel;
            }
            if (rand.Next(0, 2) == 1)
            {
                yVel = -1 * yVel;
            }

            Particle newParticle = new Particle(xPos, yPos, xVel, yVel, attributesDictionary[groupSelected]);
            particles.Add(newParticle);
            attributesDictionary[groupSelected].ParticleAdded();
            OnPropertyChanged("ParticleCount");
        }

        public void RebuildQuadtree()
        {
            quadtree.BuildQuadtree(particles.ToList());
        }


        public void UpdatePos(double deltaT)
        {
            Vector correction;
            double perimeterSafetyFactor;

            Vector accel = new Vector(0, 0);
            double sepDistance;
            Vector sepVector;
            double scaler;

            foreach (Particle p in particles)
            {
                perimeterSafetyFactor = -15;

                p.Position.AddVectors(true,p.Velocity.ScalarMultiply(false,deltaT*(tempCoeff*tempMultiplier)));

                //To implemenent unifrom grav field
                if (uniformGravEnabled)
                {
                    UniformGravField(p);
                }


                GlobalGravField(p);

                /*
                List<Node> gravContenders = quadtree.GlobalGravField(p);


                
                    foreach (Node grav in gravContenders)
                    {
                        if (quadtree.CheckLeafNode(grav) && grav.ContainedParticles!=null)
                        {
                            
                            foreach (Particle target in grav.ContainedParticles)
                            {
                            if (target != p)
                            {
                                sepDistance = p.Position.SepDistance(target.Position);
                                sepVector = p.Position.SubtractVectors(false, target.Position);
                                scaler =  -1*(target.Properties.Mass * gravConstant) / (Math.Pow(sepDistance, 2)); //this number should be 3 but changed to two to make effects of gravity more noticeable
                                accel.AddVectors(true, sepVector.ScalarMultiply(false, scaler));
                            }
                            }
                        }
                        else
                        {
                            sepDistance = p.Position.SepDistance(grav.NodeCOM);
                            sepVector = p.Position.SubtractVectors(false, grav.NodeCOM);
                            scaler = -1*(grav.NodeMass * gravConstant) / (Math.Pow(sepDistance, 2));
                            accel.AddVectors(true, sepVector.ScalarMultiply(false, scaler));
                        }
                    }
                
                p.Velocity.AddVectors(true, accel);
            */

            /*
            if(p.Position.XValue < (offset - perimeterSafetyFactor-p.Radius))
            {
                correction = new Vector(p.Radius, 0);
                vectors.AddVectors(true, p.Position, correction);
            }
            if (p.Position.XValue > (offset + perimeterWidth + perimeterSafetyFactor + p.Radius))
            {
                correction = new Vector(-1*p.Radius, 0);
                vectors.AddVectors(true, p.Position, correction);
            }
            if (p.Position.YValue < (offset - perimeterSafetyFactor - p.Radius))
            {
                correction = new Vector(0, p.Radius);
                vectors.AddVectors(true, p.Position, correction);
            }
            if (p.Position.YValue > (offset + perimeterHeight + perimeterSafetyFactor + p.Radius))
            {
                correction = new Vector(0, -1*p.Radius);
                vectors.AddVectors(true, p.Position, correction);
            }
            */
        }

        }

        public void CollisionsAgainstPerimeter()
        {
            foreach (Particle p in particles)
            {
                if(p.Position.XValue < offset)
                {
                    p.Velocity.XValue = Math.Abs(p.Velocity.XValue);
                }
                if(p.Position.XValue > offset + perimeterWidth - p.Properties.Diameter)
                {
                    p.Velocity.XValue = -1*Math.Abs(p.Velocity.XValue);
                }
                if (p.Position.YValue < offset)
                {
                    p.Velocity.YValue = Math.Abs(p.Velocity.YValue);
                }
                if(p.Position.YValue > offset + perimeterHeight - p.Properties.Diameter)
                {
                    p.Velocity.YValue = -1 * Math.Abs(p.Velocity.YValue);
                }
            }
        }

        public void CollisionsBetweenParticles()
        {
            Node[] collisionContenders;

            for(int i = 0; i < particles.Count; i++)
            {
                
                collisionContenders = quadtree.FindAdjacentNodes(particles[i].QuadtreeNode).ToArray<Node>();
                foreach (Node n in collisionContenders)
                {
                    if (n != null)
                    {
                        foreach (Particle p in n.ContainedParticles)
                        {
                            if (particles[i].Position.SepDistance(p.Position) <= particles[i].Properties.Diameter/2 + p.Properties.Diameter/2 && particles.IndexOf(p)>i)
                            {
                                SimulateParticleCollision45(particles[i], p);
                            }
                        }
                    }

                }
                
                /*
                for(int j = i+1; j<particles.Count; j++)
                {
                    if(particles[i].Position.SepDistance(particles[j].Position)<=(particles[i].Properties.Diameter/2 + particles[j].Properties.Diameter/2)&& particles[i] != particles[j])
                    {
                        particles[i].UnderCollision = "yes";
                        particles[j].UnderCollision = "yes";
                        SimulateParticleCollision45(particles[i], particles[j]);
                    }
                    else
                    {
                        particles[i].UnderCollision = "no";
                        particles[j].UnderCollision = "no";
                    }
                }
                */
                
            }

        }

        public void SimulateParticleCollision(int particle1, int particle2)
        {
            Particle par1 = particles[particle1];
            Particle par2 = particles[particle2];

            double commonFactor = (2*par1.Velocity.SubtractVectors(false, par2.Velocity).DotProduct(par1.Position.SubtractVectors(false, par2.Position)))/((par1.Properties.Mass+par2.Properties.Mass)*par1.Position.SubtractVectors(false,par2.Position).Magnitude());

            par1.Velocity = par1.Velocity.SubtractVectors(true, par1.Position.SubtractVectors(false, par2.Position).ScalarMultiply(false,commonFactor));
            par2.Velocity = par2.Velocity.AddVectors(true, par1.Position.SubtractVectors(false, par2.Position).ScalarMultiply(false, commonFactor));
        }


            
        private void SimulateParticleCollision23(Particle par1, Particle par2)
        {
            Vector sepVector = par1.Position.SubtractVectors(false, par2.Position);
            Vector orthoSepVector = sepVector.Orthogonal();

            Vector velocity1BasisChanged = par1.Velocity.ChangeBasis(sepVector, orthoSepVector);
            Vector velocity2BasisChanged = par2.Velocity.ChangeBasis(sepVector, orthoSepVector);

            Vector finalVelocity1BasisChanged = new Vector(velocity1BasisChanged.XValue, velocity1BasisChanged.YValue);
            Vector finalVelocity2BasisChanged = new Vector(velocity2BasisChanged.XValue, velocity2BasisChanged.YValue);

            double alpha = (par1.Properties.Mass) / (par2.Properties.Mass);

            double a = alpha + 1;
            double b = -2 * (alpha * velocity1BasisChanged.XValue + velocity2BasisChanged.XValue);
            double c = (Math.Pow(velocity2BasisChanged.XValue, 2) * (1 - alpha)) + (2 * alpha * velocity1BasisChanged.XValue * velocity2BasisChanged.XValue);

            double[] Velocities = QuadraticSolver(a, b, c);

            if(Velocities[0] == velocity2BasisChanged.XValue)
            {
                finalVelocity2BasisChanged.XValue = Velocities[1];
                finalVelocity1BasisChanged.XValue = ((alpha * velocity1BasisChanged.XValue) + velocity2BasisChanged.XValue - Velocities[1])/alpha;
            }
            else
            {
                finalVelocity2BasisChanged.XValue = Velocities[0];
                finalVelocity1BasisChanged.XValue = ((alpha * velocity1BasisChanged.XValue) + velocity2BasisChanged.XValue - Velocities[0]) / alpha;
            }

            /*
            if(vectors.DotProduct(particles[particle1].Velocity, particles[particle2].Velocity) < 0)
            {
                finalVelocity1BasisChanged.YValue = -1 * finalVelocity1BasisChanged.YValue;
                finalVelocity1BasisChanged.YValue = -1 * finalVelocity1BasisChanged.YValue;
            }
            */

            par1.Velocity = finalVelocity1BasisChanged.InvertFromBasis(sepVector, orthoSepVector);
            par2.Velocity = finalVelocity2BasisChanged.InvertFromBasis(sepVector, orthoSepVector);

        }
            
        private void SimulateParticleCollision45(Particle part1, Particle part2)
        {
            Vector SepVector = part1.Position.SubtractVectors(false, part2.Position);

            Vector SepHat = SepVector.ConvertToUnitVector();
            Vector PerpHat = SepHat.Orthogonal().ScalarMultiply(false, -1);

            double uParallel1 = part1.Velocity.DotProduct(SepHat);
            double uParallel2 = part2.Velocity.DotProduct(SepHat);
            double uPerp1 = part1.Velocity.DotProduct(PerpHat);
            double uPerp2 = part2.Velocity.DotProduct(PerpHat);

            double mass1 = part1.Properties.Mass;
            double mass2 = part2.Properties.Mass;

            double vParallel1 = (mass1 * uParallel1 + mass2 * uParallel2 - mass2 * coeffRestitution * (uParallel1 - uParallel2)) / (mass1 + mass2);
            double vParallel2 = vParallel1 + (uParallel1 - uParallel2) * coeffRestitution;

            double v1IHat = vParallel1 * SepHat.DotProduct(iHat) + uPerp1 * PerpHat.DotProduct(iHat);
            double v2IHat = vParallel2 * SepHat.DotProduct(iHat) + uPerp2 * PerpHat.DotProduct(iHat);

            double v1JHat = vParallel1 * SepHat.DotProduct(jHat) + uPerp1 * PerpHat.DotProduct(jHat);
            double v2JHat = vParallel2 * SepHat.DotProduct(jHat) + uPerp2 * PerpHat.DotProduct(jHat);

            part1.Velocity = new Vector(v1IHat, v1JHat);
            part2.Velocity = new Vector(v2IHat,v2JHat);

        }

        private double[] QuadraticSolver(double aCoeff, double bCoeff, double cCoeff)
        {
            double[] result = new double[2];

            result[0] = (-bCoeff + Math.Sqrt(Math.Pow(bCoeff, 2) - 4 * aCoeff * cCoeff)) / (2 * aCoeff);
            result[1] = (-bCoeff - Math.Sqrt(Math.Pow(bCoeff, 2) - 4 * aCoeff * cCoeff)) / (2 * aCoeff);

            return result;
        }

        public void MakeNewParticleGroup()
        {
            Attributes group = new Attributes(30, 1);
            particleGroupCount++;
            attributesDictionary.Add(particleGroupCount, group);
        }


        public double PerimeterWidth
        {
            get { return perimeterWidth; }
        }

        public double PerimeterHeight
        {
            get { return perimeterHeight; }
        }

        public double TempCoeff
        {
            get { return tempCoeff; }
            set { tempCoeff = value; OnPropertyChanged("TempCoeff"); }
        }

        public Dictionary<int, Attributes> AttributesDictionary
        {
            get { return attributesDictionary; }
        }

        public Quadtree GetQuadtree
        {
            get { return quadtree; }
        }

        public int GroupSelected
        {
            get { return groupSelected; }
            set { groupSelected = value; }
        }

        public double CoeffRestitution
        {
            get { return coeffRestitution; }
            set { coeffRestitution = value; OnPropertyChanged("CoeffRestitution"); }
        }

        public void UniformGravField(Particle p)
        {
           
                p.Velocity.AddVectors(true, new Vector(0, uniformGravStrength));
            
        }

        public void GlobalGravField(Particle p)
        {
            Vector accel = new Vector(0,0);
            double sepDistance;
            Vector sepVector;
            double scaler;

            foreach(Particle target in particles)
            {
                if(target != p)
                {
                    sepDistance = p.Position.SepDistance(target.Position);
                    sepVector = p.Position.SubtractVectors(false, target.Position);
                    scaler = -1*(target.Properties.Mass * gravConstant) / (Math.Pow(sepDistance, 2)); //this number should be 3 but changed to two to make effects of gravity more noticeable
                    accel.AddVectors(true,sepVector.ScalarMultiply(false, scaler));
                }
            }

            p.Velocity.AddVectors(true, accel);
        }

        public bool IsUniformGravEnabled
        {
            get { return uniformGravEnabled; }
            set { uniformGravEnabled = value;}
        }

        public double UniformGravStrength
        {
            get { return uniformGravStrength; }
            set { uniformGravStrength = value; OnPropertyChanged("UniformGravStrength"); }
        }
    }
}
