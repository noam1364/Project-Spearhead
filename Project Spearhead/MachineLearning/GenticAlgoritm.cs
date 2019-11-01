using System;
using System.Collections.Generic;
using static Global;
using static NeuralNetwork;
using Project_Spearhead;

public static class GeneticAlgorithm
{
    public static readonly int[] structure = BirdBot.brainStructure;
    public static List<NeuralNetwork> population = null;
    public static List<BirdBot> deadPopulation = null;
    public static int currentGen=1,currentSpecies=1;
    public const int populationSize = 100;
    const double mutationRate = 0.01;
    const double mutationIntensity = 0.005;
    public static readonly Random random = new Random(6969);
    private static string fileUrl = "C:/Workspaces/C# workspace/Project Spearhead/Project Spearhead/AI";
    public static void Initiate()
    {
        population = new List<NeuralNetwork>();
        deadPopulation = new List<BirdBot>();
    }
    public static void CreateFirstGen()
    {
        NeuralNetwork loadedNet = DataHandler.ReadFromBinaryFile<NeuralNetwork>(fileUrl);
        population = new List<NeuralNetwork>(GeneticAlgorithm.populationSize);

        if(loadedNet == null || loadedNet.getDims().Equals(structure)||true)  ///if data doesnt fit the current config - initilize a random generation
        {
            for(int i = 0;i < populationSize;i++)
            {
                loadedNet = new NeuralNetwork(structure);
                loadedNet.initialize();
                population.Add(loadedNet);
            }
        }
        else
        {
            for(int i = 0;i < populationSize;i++)
            {
                loadedNet = new NeuralNetwork(loadedNet); ///create a copy
                loadedNet = mutate(loadedNet, mutationRate, mutationIntensity);
                population.Add(loadedNet);
            }
        }
    }
    public static void CreateNextGen()
    {
        currentSpecies = 0;
        currentGen++;
        ///find the best player in the last population,and save it
        BirdBot bestPlayer = deadPopulation[0];
        foreach(BirdBot currentPlayer in deadPopulation)
        {
            if(currentPlayer.getFitness() > bestPlayer.getFitness())
                bestPlayer = currentPlayer;
        }
        DataHandler.WriteToBinaryFile<NeuralNetwork>(fileUrl, bestPlayer.brain);
        ///create a new population based on the best sample of the last gen,mutated
        population = new List<NeuralNetwork>();
        deadPopulation = new List<BirdBot>();
        NeuralNetwork temp;
        for(int i=0;i<populationSize;i++)
        {
            temp = new NeuralNetwork(bestPlayer.brain);
            temp = mutate(temp,mutationRate, mutationIntensity);
            population.Add(temp);
        }
    }
    private static NeuralNetwork mutate(NeuralNetwork n,double mutationRate, double mutationIntencity)
    {
        Layer l = null;
        double probability = 0;
        for(int g = 0;g < n.layers.Count;g++)
        {
            l = n.layers[g];
            for(int i = 0;i < l.neurons.Count;i++)
            {
                probability = random.NextDouble(); ///determins if the current bias will be mutated or not
                if(g != 0 && probability <= mutationRate)    ///first layer has no biases
                    l.bias[i] *= 1 - mutationIntencity + 2 * random.NextDouble() * mutationIntencity;///between -mI to +mI

                if(l.synapse != null) ///The last layer has no synaptic connections
                {
                    for(int j = 0;j < l.synapse.ColumnCount;j++)
                    {
                        probability = random.NextDouble(); ///determins if the current synaptic connection will be mutated or not
                        if(probability <= mutationRate)
                            l.synapse[i, j] = -mutationIntencity + 2 * random.NextDouble() * mutationIntencity;///between -mI to +mI
                    }
                }
            }
        }
        return n;
    }
}
