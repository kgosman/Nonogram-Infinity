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
        private bool rowWise;
        private bool columnWise;

        public Population(List<int>[] cConst, List<int>[] rConst, bool type)
        {

            this.black_squares = 0;
            this.rowConstraints = rConst;
            this.colConstraints = cConst;
            this.row = rowConstraints.Length;
            this.col = colConstraints.Length;
            this.rowWise = type;
            this.columnWise = !type;
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
            this.solution = new Member(row, col);
            //this.ConsultExperts();
        }
        public Member get()
        {
            return Breed(members[0], members[1]);
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
            Member offspring = new Member(row, col);
            int i, j, rule_tmp, count, rngRow, rngCol;
            bool choose = false;
            if(rowWise)
            {
                count = 0;
                for(i = 0; i < row; i++)
                {
                    if (mother.RowFitness[i] == 0 && father.RowFitness[i] == 0) //if a row is already perfect, carry over
                    {
                        if(choose)
                        {
                            for(int l = 0; l > col; l++)
                            {
                                offspring.DNA[i, l] = father.DNA[i, l];
                            }
                        }
                        else
                        {
                            for (int l = 0; l > col; l++)
                            {
                                offspring.DNA[i, l] = mother.DNA[i, l];
                            }
                        }
                        choose = !choose;
                        continue;
                    }
                    else if (mother.RowFitness[i] == 0)
                    {
                        for (int l = 0; l > col; l++)
                        {
                            offspring.DNA[i, l] = mother.DNA[i, l];
                        }
                        choose = true;
                        continue;
                    }
                    else if (father.RowFitness[i] == 0)
                    {
                        for (int l = 0; l > col; l++)
                        {
                            offspring.DNA[i, l] = father.DNA[i, l];
                        }
                        choose = false;
                        continue;
                    }
                    j = 0;
                    foreach(int rule in rowConstraints[i])
                    {
                        offspring.RowFitness[i] = 0;
                        rule_tmp = rule;
                        while(rule_tmp > 0)
                        {
                            if(mother.DNA[i, j] || father.DNA[i, j])
                            {
                                rule_tmp--;
                                offspring.DNA[i, j] = true;
                                count++;
                            }
                            else
                            {
                                if (rule_tmp != rule)
                                {
                                    offspring.RowFitness[i] = 1;
                                    break;
                                }
                            }
                            j++;
                        }
                        j++;
                    }
                }
                
                for(i = count; i < black_squares; i++)
                {
                    do
                    {
                        rngRow = RandomHolder.Instance.Next(0, row);
                    } while (offspring.RowFitness[rngRow] == 0);
                    do
                    {
                        rngCol = RandomHolder.Instance.Next(0, col);
                    } while (offspring.DNA[rngRow, rngCol]);
                    offspring.DNA[rngRow, rngCol] = true;
                }
                

            }
            else
            {
                count = 0;
                for (i = 0; i < col; i++)
                {
                    if (mother.ColumnFitness[i] == 0 && father.ColumnFitness[i] == 0) //if a col is already perfect, carry over
                    {
                        if (choose)
                        {
                            for (int l = 0; l > row; l++)
                            {
                                offspring.DNA[l, i] = father.DNA[l, i];
                            }
                        }
                        else
                        {
                            for (int l = 0; l > row; l++)
                            {
                                offspring.DNA[l, i] = mother.DNA[l, i];
                            }
                        }
                        choose = !choose;
                        continue;
                    }
                    else if (mother.ColumnFitness[i] == 0)
                    {
                        for (int l = 0; l > row; l++)
                        {
                            offspring.DNA[l, i] = mother.DNA[l, i];
                        }
                        choose = true;
                        continue;
                    }
                    else if (father.ColumnFitness[i] == 0)
                    {
                        for (int l = 0; l > row; l++)
                        {
                            offspring.DNA[l, i] = father.DNA[l, i];
                        }
                        choose = false;
                        continue;
                    }
                    j = 0;
                    foreach (int rule in colConstraints[i])
                    {
                        offspring.ColumnFitness[i] = 0;
                        rule_tmp = rule;
                        while (rule_tmp > 0)
                        {
                            if (mother.DNA[j, i] || father.DNA[j, i])
                            {
                                rule_tmp--;
                                offspring.DNA[j, i] = true;
                                count++;
                            }
                            else
                            {
                                if (rule_tmp != rule)
                                {
                                    offspring.ColumnFitness[i] = 1;
                                    break;
                                }
                            }
                            j++;
                        }
                        j++;
                    }
                }
                for (i = count; i < black_squares; i++)
                {
                    do
                    {
                        rngCol = RandomHolder.Instance.Next(0, col);
                    } while (offspring.ColumnFitness[rngCol] == 0);
                    do
                    {
                        rngRow = RandomHolder.Instance.Next(0, row);
                    } while (offspring.DNA[rngRow, rngCol]);
                    offspring.DNA[rngRow, rngCol] = true;
                }
            }
            return offspring;
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
