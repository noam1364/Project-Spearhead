using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Spearhead.MachineLearning.NEAT
{
    public class NeatBird :Bird
    {
        public Network brain;
        public bool isLive;
        public NeatBird()
        {
            isLive = false;
        }
        public NeatBird(Network brain)
        {
            isLive = false;
            this.brain = brain;
        }
        public void init()
        {
            isLive = true;
        }
        public override void update()
        {
            if(!isLive)
                return;

            Obsticle closest = closestObst();
            ///for some reason the training is extremely fast WITHOUT input normalization
            float dx = (float)(closest.XRight() - X()) / Global.winWidth,
                dy = (float)(closest.hatchYCenter() - YCenter()) / Global.winHeight,
                vy = this.vy / Global.winHeight;
            ///feed values to the network
            float[] input = new float[] { dx, dy, vy };
            ///procces output and play accourdingly
            double prob = brain.GetOutput(input)[0];
            if(prob >= 0.5) jump();
            ///update bird
            movmentManager();
            CheckCollitions();
        }
        public override void draw()
        {
            if(isLive)
                base.draw();
        }
        protected override void killBird()
        {
            brain.SetFitness(getFitness());
            isLive = false;
        }
        public void Destroy()
        {

        }
        public float getFitness()
        {
            return score;
        }
    }
}
