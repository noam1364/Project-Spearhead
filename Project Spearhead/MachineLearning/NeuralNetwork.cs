﻿using System;
using System.Linq;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System.Reflection;

[Serializable]
public class NeuralNetwork
{
    private static Random random = new Random(420);
    public List<Layer> layers;
    public NeuralNetwork()
    {   ///for serializing perpuses

    }
    public NeuralNetwork(int[] dims)
    {
        layers = new List<Layer>();
        for(int i = 0;i < dims.Length-1 ;i++)
            layers.Add(new Layer(new int[] { dims[i], dims[i + 1] }));
        layers.Add(new Layer(new int[] { dims[dims.Length-1],0}));///last layer has no synaptic connections,only neurons and biases
        layers[0].bias = null;///first layer has no biases
    }
    public NeuralNetwork(NeuralNetwork n):this(n.getDims())
    {
        Layer l = null;
        for(int g = 0;g < layers.Count - 1;g++)   ///not in including the last layer,which is a dummy layer
        {
            l = layers[g];
            for(int i = 0;i < l.synapse.RowCount;i++)
            {
                if(g != 0)    ///first layer has no biases
                    l.bias[i] = n.layers[g].bias[i];
                if(l.synapse != null) ///if its a last layer,there are no synaptic connections
                {
                    for(int j = 0;j < l.synapse.ColumnCount;j++)
                    {
                        l.synapse[i, j] = n.layers[g].synapse[i, j];
                    }
                }
            }
        }
    }
    public NeuralNetwork(List<NeuralNetwork>parentsLst):this(parentsLst[0].getDims())
    {
        Layer l = null;
        Random random = new Random();
        NeuralNetwork[] parents = parentsLst.ToArray();
        NeuralNetwork curParent;
        for(int g = 0;g < layers.Count - 1;g++)   ///not in including the last layer,which is a dummy layer
        {
            curParent = selectParent(random, parents);
            l = layers[g];
            for(int i = 0;i < l.synapse.RowCount;i++)
            {
                if(g != 0)    ///first layer has no biases
                    l.bias[i] = curParent.layers[g].bias[i];
                if(l.synapse!=null) ///if its a last layer,there are no synaptic connections
                {
                    for(int j = 0;j < l.synapse.ColumnCount;j++)
                    {
                        l.synapse[i, j] = curParent.layers[g].synapse[i,j];
                    }
                }
            }
        }
    }
    public Vector<double> feedNet(Vector<double> input)
    {
        if(input.Count!=layers[0].neurons.Count)return null;///input must be same size as the input layer
        layers[0].neurons = input; ///initiate first layer with the input;
        Layer layer, nextLayer = null;
        for(int i=0;i<layers.Count-1;i++)
        {
            layer = layers[i];nextLayer = layers[i + 1];
            nextLayer.neurons = layer.neurons * layer.synapse;
            nextLayer.neurons = sigmoid(nextLayer.neurons+nextLayer.bias); ///activation function
        }
        return nextLayer.neurons;   
    }
    public void mutate(double mutationRate,double mutationIntencity,Random random)
    {
        Layer l = null;
        double probability = 0;
        for(int g = 0;g < layers.Count ;g++)  
        {
            l = layers[g];
            for(int i = 0;i < l.neurons.Count;i++)
            {
                probability = random.NextDouble(); ///determins if the current bias will be mutated or not
                if(g != 0 && probability <= mutationRate)    ///first layer has no biases
                    l.bias[i] *= 1 -mutationIntencity + 2 * random.NextDouble() * mutationIntencity;///between -mI to +mI

                if(l.synapse!=null) ///The last layer has no synaptic connections
                { 
                    for(int j = 0;j < l.synapse.ColumnCount;j++)
                    {
                        probability = random.NextDouble(); ///determins if the current synaptic connection will be mutated or not
                        if(probability<=mutationRate)
                            l.synapse[i, j] = -mutationIntencity + 2 * random.NextDouble() * mutationIntencity;///between -mI to +mI
                    }
                }
            }
        }
    }
    private Vector<double> sigmoid(Vector<double> v)
    {
        Vector<double> ret = Vector<double>.Build.Dense(v.Count);
        for(int i = 0;i < v.Count;i++)
            ret[i] = 1 / (1 + Math.Pow(Math.E, -v[i]));
        return ret;
    }
    public void initialize()
    {
        Layer l = null;
        for(int g=0;g<layers.Count;g++)   ///not including the last layer,which is a dummy layer
        {
            l = layers[g];
            for(int i=0;i<l.neurons.Count;i++)
            {
                if(g!=0)    ///first layer has no biases
                    l.bias[i] = -1+2*random.NextDouble();
                if(l.synapse !=null) ///if its a last layer,there are no synaptic connections
                {
                    for(int j = 0;j < l.synapse.ColumnCount;j++)
                    {
                        l.synapse[i, j] = -1 + 2 * random.NextDouble();
                    }
                }
            }
        }
    }
    public int[] getDims()
    {
        int[] arr = new int[this.layers.Count];
        for(int i = 0;i < arr.Length;i++)
        {
            arr[i] = layers[i].neurons.Count;
        }
        return arr;
    }
    private NeuralNetwork selectParent(Random rand,NeuralNetwork[] parents)
    {   ///random is recived as a param because if a new Random will be created each call,it will be the same every time
        double prob = rand.NextDouble();
        int parentIdx = (int)Math.Floor(prob * parents.Length);
        return parents[parentIdx];
    }

    [Serializable]
    public class Layer
    {
        public Matrix<double> synapse; ///connections synapses to the next layer   |   1st index represents the weights of all 
        ///connections of a specific neuron,and the 2nd index is the weight of the connections of this layer to each neurons in the next layer
        ///example : synape[0,1] is the weight of the connection between the 0 neuron of this layer to the 1 neuron of the next
        ///dims = synape[this.length,next.length]
        ///each row in a neuron
        public Vector<double> neurons;
        public Vector<double> bias;
        public MethodInfo activation;

        public Layer(int[] dims)
        {
            if(dims[1] == 0)
            {
                synapse = null;///if this is a last layer in a network
            }
            else
            {
                synapse = Matrix<double>.Build.Dense(dims[0], dims[1]);
            }
            activation = null;
            neurons = Vector<double>.Build.Dense(dims[0]);
            bias = Vector<double>.Build.Dense(dims[0]);
        }
    }
}