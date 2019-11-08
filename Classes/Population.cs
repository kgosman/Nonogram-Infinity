using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nonogram_Infinity
{
    public class Population
    {

        public List<Member> members;
        public double[,] matrix;
        public Member solution;

        private int row;
        private int col;
        private int black_squares;
        private List<int>[] colConstraints;
        private List<int>[] rowConstraints;

        public Population(List<int>[] cConst, List<int>[] rConst)
        {
            this.black_squares = 0;
            this.rowConstraints = rConst;
            this.colConstraints = cConst;
            this.row = rowConstraints.Length;
            this.col = colConstraints.Length;
            for(int i = 0; i < row; i++)
            {
                foreach(int a in rowConstraints[i])
                {
                    black_squares += a;
                }
            }
            this.members = new List<Member>(100);
            for(int i = 0; i < 100; i++)
            {
                members.Add(new Member(row, col, black_squares, rowConstraints, colConstraints));
            }
            this.ConsultExperts();
        }

        //Clone all members in a population
        public void Clone(Population population)
        {
            foreach (Member member in population.members)
            {
                members.Add(member);
            }
        }
        //Breeding with 2 splice points chosen at random
        public Member Breed(Member mother, Member father)//Austin todo
        {
            return new Member(row, col, black_squares, rowConstraints, colConstraints);
        }
        //Breed top 50% discard bottom 25% and replace with 25% from the resultant breeding
        public void BreedPopulaton(bool elitePreservation)//***********done************
        {
            List<Member> offspring = new List<Member>();

            int j = 0;
            for(int i = 0; i < members.Count / 4; i++)
            {
                offspring.Add(Breed(members[j], members[j + 1]));
                j += 2;
            }

            for (int i = 0; i < offspring.Count; i++)
            {
                members.RemoveAt(members.Count - 1);
            }
            foreach(Member child in offspring)
            {
                child.FindFitness(rowConstraints, colConstraints);
                members.Add(child);
            }
            MutatePopulation(elitePreservation);
            members = members.OrderBy(member => member.Fitness).ToList();
        }
        //Function to mutate a member with 2% chance
        public void Mutate(Member member) //Will todo
        {
            int row1 = 0, row2 = 1;

            int col1 = 0, col2 = 1;

            bool row1Okay = false, row2Okay = false, col1Okay = false, col2Okay = false;

            while (true)
            {
                if (!row1Okay)
                    row1 = RandomHolder.Instance.Next(0, row);

                if (!row2Okay)
                    row2 = RandomHolder.Instance.Next(0, row);

                if (!col1Okay)
                    col1 = RandomHolder.Instance.Next(0, col);

                if (!col2Okay)
                    col2 = RandomHolder.Instance.Next(0, col);

                if (member.RowFitness[row1] > 0) row1Okay = true;

                if (member.RowFitness[row2] > 0) row2Okay = true;

                if (member.ColumnFitness[col1] > 0) col1Okay = true;

                if (member.ColumnFitness[col2] > 0) col2Okay = true;

                if (row1 == row2) row1Okay = row2Okay = false;

                if (col1 == col2) col1Okay = col2Okay = false;

                if (row1Okay && row2Okay && col1Okay && col2Okay) break;
            }
        }
        //Mutate bottom 90% of population if elitePreservation is true
        public void MutatePopulation(bool elitePreservation)
        {
            int i = 0;
            foreach(Member member in members)
            {
                if(i > members.Count / 10 && elitePreservation == true)
                {
                    Mutate(member);
                }
                i++;
            }
        }
        //WoC function
        public void ConsultExperts() //Kaden todo
        {
            int[,] agreement = new int[row,col];

            foreach(Member member in members)
            {
                for(int i = 0; i < row; i++)
                {
                    for(int j = 0; j < col; j++)
                    {
                        if(member.DNA[i,j] == true)
                        {
                            agreement[i, j] += 1;
                        }
                    }
                }
            }
            solution = new Member(row, col, 0, rowConstraints, colConstraints);
            int x = 0;
            while(x != black_squares)
            {
                int highestI = 0, highestJ = 0, previousMax = -1;
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        if (agreement[i, j] > previousMax || previousMax == -1)
                        {
                            previousMax = agreement[i, j];
                            highestI = i;
                            highestJ = j;
                        }
                    }
                }
                agreement[highestI, highestJ] = 0;
                solution.DNA[highestI, highestJ] = true;
                x++;
            }
        }
    }
}
