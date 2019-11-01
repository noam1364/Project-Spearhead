using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace Project_Spearhead
{
    public class BirdBot : Bird
    {
        public static int[] brainStructure = new int[] { 3, 6, 6, 1 };
        public NeuralNetwork brain;
        public BirdBot()
        {

        }
        public BirdBot(NeuralNetwork brain)
        {
            this.brain = brain;
        }
        public override void update()
        {
            Obsticle closest = closestObst();
            double dx = (float)(closest.XRight() - X()) / Global.winWidth,
                dy = (float)(closest.hatchYCenter() - YCenter()) / Global.winHeight,
                vy = this.vy / Global.winHeight;
            ///feed values to the network
            Vector<double> input = Vector<double>.Build.DenseOfArray(new double[] {dx,dy,vy });
            ///procces output and play accourdingly
            double prob = brain.feedNet(input)[0];
            if(prob >= 0.5) jump();
            ///update bird
            movmentManager();
            CheckCollitions();
        }
        protected override void killBird()
        {
            GeneticAlgorithm.deadPopulation.Add(this);
        }
        public float getFitness()
        {
            return score;
        }
    }
}
