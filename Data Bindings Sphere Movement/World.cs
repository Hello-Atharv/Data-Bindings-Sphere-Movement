using System;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Collections.Generic;

namespace DataBindingsSphereMovement
{
    class World : INotifyPropertyChanged
    {
        private Random rand = new Random();

        private const double perimeterWidth = 1475.2;
        private const double perimeterHeight = 803.2;
        private const double offset = 30;

        Quadtree quadtree = new Quadtree(new Vector(offset, offset), new Vector(offset + perimeterWidth, offset + perimeterHeight));

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Particle> particles;
        private List<Attributes> attributes;

        private double tempCoeff = 0;
        private const double tempMultiplier = 0.333;

        public World()
        {
            particles = new ObservableCollection<Particle>();
            attributes = new List<Attributes>();

            Vector topLeft = new Vector(offset, offset);
            Vector bottomRight = new Vector(offset + perimeterWidth, offset + perimeterHeight);
            
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

            Particle newParticle = new Particle(xPos, yPos, xVel, yVel, attributes[0]);
            particles.Add(newParticle);
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

            foreach (Particle p in particles)
            {
                perimeterSafetyFactor = -15;

                p.Position.AddVectors(true,p.Velocity.ScalarMultiply(false,deltaT*(1+tempCoeff*tempMultiplier)));

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
                if((p.Position.XValue < offset)||(p.Position.XValue > offset + perimeterWidth-2*p.Properties.Radius))
                {
                    p.Velocity.XValue = -1 * p.Velocity.XValue;
                }
                if ((p.Position.YValue < offset) || (p.Position.YValue > offset + perimeterHeight - 2 * p.Properties.Radius))
                {
                    p.Velocity.YValue = -1 * p.Velocity.YValue;
                }
            }
        }

        public void CollisionsBetweenParticles()
        {

            for(int x = 0; x< particles.Count; x++)
            {
                for(int y = 0; y< particles.Count-1; y++)
                {

                    if( (particles[x].Position.SepDistance(particles[y].Position) <= particles[x].Properties.Radius + particles[y].Properties.Radius) && x != y)
                    {
                        SimulateParticleCollision23(x, y);
                       
                    }
                }
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


            
        private void SimulateParticleCollision23(int particle1, int particle2)
        {
            Vector sepVector = particles[particle1].Position.SubtractVectors(false, particles[particle2].Position);
            Vector orthoSepVector = sepVector.Orthogonal();

            Vector velocity1BasisChanged = particles[particle1].Velocity.ChangeBasis(sepVector, orthoSepVector);
            Vector velocity2BasisChanged = particles[particle2].Velocity.ChangeBasis(sepVector, orthoSepVector);

            Vector finalVelocity1BasisChanged = new Vector(velocity1BasisChanged.XValue, velocity1BasisChanged.YValue);
            Vector finalVelocity2BasisChanged = new Vector(velocity2BasisChanged.XValue, velocity2BasisChanged.YValue);

            double alpha = (particles[particle1].Properties.Mass) / (particles[particle2].Properties.Mass);

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

            particles[particle1].Velocity = finalVelocity1BasisChanged.InvertFromBasis(sepVector, orthoSepVector);
            particles[particle2].Velocity = finalVelocity2BasisChanged.InvertFromBasis(sepVector, orthoSepVector);

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
            Attributes group = new Attributes(10, 1);
            attributes.Add(group);
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

        public List<Attributes> Attributes
        {
            get { return attributes; }
        }



        public ref Quadtree GetQuadtree
        {
            get { return ref quadtree; }
        }
    }
}
