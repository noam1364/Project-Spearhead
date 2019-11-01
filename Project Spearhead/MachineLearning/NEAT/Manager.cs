using System.Collections.Generic;
using System;
using Project_Spearhead.MachineLearning.NEAT;

public class Manager 
{
    public NeatBird playerPrefab;
    public List<NeatBird> playerList;
    private List<Genome> genomes;
    private List<Network> nets;
    private Dictionary<Genome, Network> networkMap;
    private Dictionary<Genome, Species> speciesMap;
    private List<Species> speciesList;
    private bool training;
    private int generation;
    private static Random random = new Random(69);
    public const int population = 150;
    public const int inputNodes = 3;
    public const int outputNodes = 1;
    public const float C1 = 1f;
    public const float C2 = 1f;
    public const float C3 = 0.3f;
    public const float compatiblityThreshold = 3f;
    public const float survivalChance = 0.1f;
    public const float weightMutationChance = 0.2f; //originaly 0.8
    public const float randomWeightChance = 0.05f; //originaly 0.1
    public const float addNodeChance = 0.03f; //originaly 0.03
    public const float addConnectionChance = 0.05f; //originaly 0.05


    public void Start ()
    {
        training = false;
        generation = 1;
        genomes = new List<Genome>();
        speciesList = new List<Species>();
        playerList = new List<NeatBird>();
        System.Random r = new System.Random();
        for(int i = 0; i<population; i++)
        {
            Genome genome = new Genome(inputNodes, outputNodes, r);
            genomes.Add(genome);
        }
	}
	
	public void Update ()
    {
        if(!training)
        {
            AssignSpecies();
            MakePlayers();
            StartTraining();
        }

        if(TrainingComplete())
        {
            DestroyPlayers();
            SortNets();
            NextGen();
            training = false;
        }

    }

    private void AssignSpecies()
    {
        speciesMap = new Dictionary<Genome, Species>();
        foreach (Genome gen in genomes)
        {
            bool found = false;
            foreach (Species species in speciesList)
            {
                float distance = GenomeUtils.CompatiblityDistance(gen, species.GetMascot(), C1, C2, C3);
                if (distance < compatiblityThreshold)
                {
                    species.AddMember(gen);
                    speciesMap.Add(gen, species);
                    found = true;
                    break;
                }
            }

            if(!found)
            {
                Species species = new Species(gen);
                speciesList.Add(species);
                speciesMap.Add(gen, species);
            }
        }

        System.Random r = new System.Random();

        for(int i = speciesList.Count-1; i>=0; i--)
        {
            if(speciesList[i].GetCount()==0)
            {
                speciesList.RemoveAt(i);
            }
            else
            {
                speciesList[i].RandomizeMascot(r);
            }
        }

        //Debug.Log("Gen: " + generation + ", Population: " + population + ", Species: " + speciesList.Count);
    }

    private void MakePlayers()
    {
        nets = new List<Network>();
        networkMap = new Dictionary<Genome, Network>();

        foreach (Genome genome in genomes)
        {
            Network net = new Network(genome);
            nets.Add(net);
            networkMap.Add(genome, net);
        }

        foreach (Network net in nets)
        {
            ///new Player
            NeatBird player = new NeatBird();
            playerList.Add(player);
            player.brain = net;    ///player.setBrain()
        }
    }

    private void StartTraining()
    {
        training = true;
        foreach (NeatBird player in playerList)
        {
            player.init(); ///make 'update' active,not nessesary
        }
    }

    private bool TrainingComplete()
    {
        bool flag = true;
        foreach (NeatBird player in playerList)
        {
            if(player.isLive)    ///player.isLive()
            {
                flag = false;
                break;
            }
        }
        return flag;
    }

    private void DestroyPlayers()
    {
        foreach(NeatBird player in playerList)
        {
            player.Destroy();   
        }
        playerList.Clear();
    }

    private void SortNets()
    {
        foreach (Network net in nets)
        {
            net.SetFitness(net.GetFitness()/speciesMap[net.GetGenome()].GetCount());
            speciesMap[net.GetGenome()].AddFitness(net.GetFitness());
        }

        nets.Sort();
        speciesList.Sort();
    }

    private void NextGen()
    {
        Global.game.restart();
        generation++;
        float totalFitness = 0;
        float leftPopulation = population * (1 - survivalChance);
        List<Genome> nextGenomes = new List<Genome>();

        foreach (Species species in speciesList)
        {
            totalFitness += species.GetFitness();
        }

        for (int i=0; i<(int)(population*survivalChance); i++)
        {
            nextGenomes.Add(nets[i].GetGenome());
        }

        foreach (Species species in speciesList)
        {
            for (int i=0; i< (int)(species.GetFitness() / totalFitness * leftPopulation); i++)
            {
                Genome parent1 = species.GetRandomGenome(random);
                Genome parent2 = species.GetRandomGenome(random);
                Genome child = new Genome();

                if(networkMap[parent1].GetFitness()> networkMap[parent2].GetFitness())
                {
                    child = GenomeUtils.Crossover(parent1, parent2, random);
                }
                else
                {
                    child = GenomeUtils.Crossover(parent2, parent1, random);
                }
                nextGenomes.Add(child);
            }
        }

        while(nextGenomes.Count<population)
        {
            Genome parent1 = speciesList[0].GetRandomGenome(random);
            Genome parent2 = speciesList[0].GetRandomGenome(random);
            Genome child = new Genome();

            if (networkMap[parent1].GetFitness() > networkMap[parent2].GetFitness())
            {
                child = GenomeUtils.Crossover(parent1, parent2, random);
            }

            else
            {
                child = GenomeUtils.Crossover(parent2, parent1, random);
            }

            nextGenomes.Add(child);
        }

        foreach (Genome genome in nextGenomes)
        {
            double roll = random.NextDouble();

            if (roll < weightMutationChance)
            {
                genome.Mutate(randomWeightChance, random);
            }
            else if (roll < weightMutationChance + addNodeChance)
            {
                genome.AddNodeMutation(random);
            }
            else if (roll < weightMutationChance + addNodeChance + addConnectionChance)
            {
                genome.AddConnectionMutation(random);
            }
        }

        foreach (Species species in speciesList)
        {
            species.Reset();
        }
        genomes = nextGenomes;
    }
}
