using System;
using System.Windows.Threading;

namespace DataBindingsSphereMovement
{

    public delegate void NotifyClosing();
    public class SimBuild
    {
        int ticks;
        private DispatcherTimer timer;

        private const double deltaT = 0.02;

        public World SimWorld = new World();

        public event EventHandler TickNotify;

        public SimBuild()
        {
            timer = new DispatcherTimer();
            timer.Tick += GameTimerEvent;
            timer.Interval = TimeSpan.FromMilliseconds(deltaT * 1000);
            timer.Start();
        }

        private void GameTimerEvent(object sender, EventArgs e)
        { 
            UpdateSim();
            OnTickNotify(EventArgs.Empty);
            ticks++;
        }


        private void UpdateSim()
        {
            SimWorld.RebuildQuadtree();
            SimWorld.UpdatePos(deltaT);
            SimWorld.CollisionsAgainstPerimeter();
            SimWorld.CollisionsBetweenParticles();
            
        }

        public void ParticleAdd(double xPos, double yPos)
        {
            SimWorld.AddParticle(xPos, yPos);
        }

        public void ParticleGroupAdd()
        {
            SimWorld.MakeNewParticleGroup();
        }

        public int Ticks
        {
            get { return ticks; }
        }

        protected virtual void OnTickNotify(EventArgs e)
        {
            TickNotify?.Invoke(this, e);
        }

        public double DeltaT
        {
            get { return deltaT; }
        }

     }
}
