using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

public class DNA<T>
{
    public bool[,] Matrix { get; private set; }
    public int Fitness { get; private set; }

	private System.Random random;
	private Func<T> getRandomGene;
	private Func<int, double> fitnessFunction;
    private Func<T, T, int> evaluationFunction;
    private Func<DNA<T>, DNA<T>,DNA<T>> crossOverBFunction;
    private Func<List<IChromosome<T>>, double> IndividualFitnessFunction;
    private readonly bool useCrossOverB;
    private float IndividualMutationRate;
    private float ChromosomeMutationRate;

	public DNA(int col, int row, int filled)
	{
        Matrix = new bool[row, col];
        Random rng = new Random();
        int rng_tmp, i, j;
        for(int l = 0; l < row; l++)
        {
            for(int k = 0; k < col; k++)
            {
                Matrix[l,k] = false;
            }
        }
        for(int l = 0; l < filled; l++)
        {
            rng_tmp = rng.Next(0, row * col);
            i = rng_tmp / col;
            j = rng_tmp % col;
            Matrix[i,j] = true;
        }

    }

    public DNA<T> Crossover(DNA<T> otherParent)
    {
        return this;
    }









}