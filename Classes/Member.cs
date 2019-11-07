using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonogram_Infinity
{
    public class Member
    {
        public int fitness;
        public bool[,] dna;
        private int row;
        private int col;
        public Member (int row, int col, int black_squares)
        {
            this.fitness = 0;
            this.dna = new bool[row, col];
            this.row = row;
            this.col = col;
            int l, k, rng;
            int[] array = new int[row * col];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    dna[i, j] = false;
                    array[col * i + j] = col * i + j;
                }
            }
            int tmp;
            for (int i = 0; i < black_squares; i++)
            {
                rng = RandomHolder.Instance.Next(0, row * col-i);
                tmp = array[rng];
                array[rng] = array[row * col - i - 1];
                array[row * col - i - 1] = tmp;
                l = tmp / col;
                k = tmp % col;
                dna[l, k] = true;

            }
            FindFitness();
        }
        //Calculate fitness of the dna
        public void FindFitness() //Will todo
        {
           
        }
        //Clone a members dna
        public void Clone(bool[,] copyDNA)
        {
            dna = copyDNA;
        }
        
    }
}