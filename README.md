# Project-Spearhead
A machine learning experiment, based on the code of 'Desert Eagle'. The project implements 2 different algorithms of building and training a neural network (using neuro evolution) to play the game.

One algorithm is the vanilla algorithm, which is based on a fixed topology neural network,
trained in a procces of mutation. The implementation of this algorithm in this project was
created by me.

The second algorithm is called NEAT (Neuro Evolution of Augmenting Topologies), and is
based on an augmenting topology neural networks, which train via mutation, but also by
breeding. More info on NEAT here https://towardsdatascience.com/neat-an-awesome-approach-to-neuroevolution-3eca5cc7930f).
The implemntation of the NEAT algorithm as it appears in the game, is a heavily modified version of this implementation: https://github.com/Atharv24/NEAT .

For simplicity, I seperated the 2 algorithms into 2 different main classes of the game, so in order to
switch between methods, you simply replace the 'Game' object being created in 'Program' class.

