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
        public int[,] matrix;
        public int[,] helper; //0: nothing; 1: must be filled; 2: cannot be filled
        public Member solution;

        private int row;
        private int col;
        private int black_squares;
        private List<int>[] colConstraints;
        private List<int>[] rowConstraints;
        private List<int>[] colConstraintsR;
        private List<int>[] rowConstraintsR;
        private bool rowWise;
        private bool columnWise;

        public Population(List<int>[] cConst, List<int>[] rConst, bool type)
        {

            this.black_squares = 0;
            this.rowConstraints = rConst;
            this.colConstraints = cConst;
            this.colConstraintsR = cConst;
            this.rowConstraintsR = rConst;
            this.colConstraintsR.Reverse();
            this.rowConstraintsR.Reverse();
            this.row = rowConstraints.Length;
            this.col = colConstraints.Length;
            this.helper = new int[row, col];
            this.rowWise = type;
            this.columnWise = !type;
            int i, j, rule_tmp;
            for (i = 0; i < row; i++)
            {
                for(j = 0; j < col; j++)
                {
                    if (rowConstraints[i][0] == 0 || colConstraints[j][0] == 0)
                        helper[i, j] = 2;
                    else
                        helper[i, j] = 0;
                }
                foreach(int a in rowConstraints[i])
                {
                    black_squares += a;
                }
            }
            this.members = new List<Member>(100);

            for(i = 0; i < row; i++)
            {
                j = 0;
                foreach(int rule in rowConstraints[i])
                {
                    rule_tmp = rule;
                    while(rule_tmp > 0)
                    {
                        helper[i, j] = 3;
                        j++;
                        rule_tmp--;
                    }
                }
            }
            for (i = 0; i < row; i++)
            {
                j = col-1;
                foreach (int rule in rowConstraintsR[i])
                {
                    rule_tmp = rule;
                    while (rule_tmp > 0)
                    {
                        if(helper[i, j] == 3)
                        {
                            helper[i, j] = 1;
                        }
                        j--;
                        rule_tmp--;
                    }
                }
            }


            for (i = 0; i < col; i++)
            {
                j = row-1;
                foreach (int rule in colConstraintsR[i])
                {
                    rule_tmp = rule;
                    while (rule_tmp > 0)
                    {
                        if (helper[j, i] != 1)
                            helper[j, i] = 4;
                        j--;
                        rule_tmp--;
                    }
                }
            }
            for (i = 0; i < col; i++)
            {
                j = 0;
                foreach (int rule in colConstraints[i])
                {
                    rule_tmp = rule;
                    while (rule_tmp > 0)
                    {
                        if (helper[j, i] == 4)
                            helper[j, i] = 1;
                        j++;
                        rule_tmp--;
                    }
                }
            }

            for (i = 0; i < 100; i++)
            {
                //members.Add(new Member(row, col, black_squares, rowConstraints, colConstraints, helper));
                members.Add(new Member(row, col, rowConstraints, colConstraints, helper, type));
            }
            this.solution = new Member(row, col, rowWise);
            //this.ConsultExperts();
        }


        //Clone all members in a population
        public void Clone(Population population)
        {
            foreach (Member member in population.members)
            {
                members.Add(member);
            }
        }
        public Member Breed2(Member mother, Member father)
        {
            int i, j, k, start;
            Member offspring = new Member(row, col, rowWise);
            if(rowWise)
            {
                for(i = 0; i < row; i++)
                {
                    k = 0;
                    foreach(int rule in rowConstraints[i])
                    {
                        start = (mother.starting[i][k] + father.starting[i][k])/2;
                        offspring.starting[i].Add(start);
                        for(j = 0; j < rule; j++)
                        {
                            offspring.DNA[i, j + start] = true;
                        }
                       k++;
                    }
                }
            }
            else
            {
                for (i = 0; i < col; i++)
                {
                    k = 0;
                    foreach (int rule in colConstraints[i])
                    {
                        start = (mother.starting[i][k] + father.starting[i][k]) / 2;
                        offspring.starting[i].Add(start);
                        for (j = 0; j < rule; j++)
                        {
                            //offspring.DNA[i, j + start] = true;
                            offspring.DNA[j + start, i] = true;
                        }
                        k++;
                    }
                }
            }
            return offspring;
        }

        //Breeding with 2 splice points chosen at random
        public Member Breed(Member mother, Member father)//Austin todo
        {
            Member offspring = new Member(row, col, rowWise);
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
                            if (j >= col)
                                break;
                            if (mother.DNA[i, j] || father.DNA[i, j])
                            {
                                rule_tmp--;
                                offspring.DNA[i, j] = true;
                                count++;
                            }
                            else
                            {
                                if (rule_tmp != rule)
                                {
                                    j += rule_tmp;
                                    offspring.RowFitness[i] = 1;
                                    break;
                                }
                            }
                            j++;
                        }
                        j++;
                    }
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
                    offspring.ColumnFitness[i] = 0;
                    foreach (int rule in colConstraints[i])
                    {
                        
                        
                        rule_tmp = rule;
                        while (rule_tmp > 0)
                        {
                            if (j >= row)
                                break;
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
                                    j += rule_tmp;
                                    offspring.ColumnFitness[i] = 1;
                                    break;
                                }
                            }
                            j++;
                        }
                        j++;
                    }
                }
            }
            for (i = count; i < black_squares; i++)
            {
                do
                {
                    rngRow = RandomHolder.Instance.Next(0, row);
                    rngCol = RandomHolder.Instance.Next(0, col);
                } while (offspring.DNA[rngRow, rngCol] || helper[rngRow, rngCol] == 2);
                offspring.DNA[rngRow, rngCol] = true;
            }


            return offspring;
        }
        public Member BreedReverse(Member mother, Member father)//Austin todo
        {
            Member offspring = new Member(row, col, rowWise);
            int i, j, rule_tmp, count, rngRow, rngCol;
            bool choose = false;
            if (rowWise)
            {
                count = 0;
                for (i = 0; i < row; i++)
                {
                    if (mother.RowFitness[i] == 0 && father.RowFitness[i] == 0) //if a row is already perfect, carry over
                    {
                        if (choose)
                        {
                            for (int l = 0; l > col; l++)
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
                    j = col - 1;
                    foreach (int rule in rowConstraintsR[i])
                    {

                        offspring.RowFitness[i] = 0;
                        rule_tmp = rule;
                        while (rule_tmp > 0)
                        {
                            if (j <= 0)
                                break;
                            if (mother.DNA[i, j] || father.DNA[i, j])
                            {
                                rule_tmp--;
                                offspring.DNA[i, j] = true;
                                count++;
                            }
                            else
                            {
                                if (rule_tmp != rule)
                                {
                                    j -= rule_tmp;
                                    offspring.RowFitness[i] = 1;
                                    break;
                                }
                            }
                            j--;
                        }
                        j--;
                    }
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
                    j = row - 1;
                    offspring.ColumnFitness[i] = 0;
                    foreach (int rule in colConstraintsR[i])
                    {
                        rule_tmp = rule;
                        while (rule_tmp > 0)
                        {
                            if (j <= row)
                                break;
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
                                    j -= rule_tmp;
                                    offspring.ColumnFitness[i] = 1;
                                    break;
                                }
                            }
                            j--;
                        }
                        j--;
                    }
                }
            }
            for (i = count; i < black_squares; i++)
            {
                do
                {
                    rngRow = RandomHolder.Instance.Next(0, row);
                    rngCol = RandomHolder.Instance.Next(0, col);
                } while (offspring.DNA[rngRow, rngCol] || helper[rngRow, rngCol] == 2);
                offspring.DNA[rngRow, rngCol] = true;
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
                offspring.Add(Breed2(members[j], members[j + 1]));
                //offspring.Add(Breed(members[j], members[j + 1]));
                //offspring.Add(BreedReverse(members[j], members[j + 1]));
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
        //Mutate bottom 90% of population if elitePreservation is true
        public void MutatePopulation(bool elitePreservation)
        {
            int j = 0;
            for (int i = 0; i < members.Count; i++)
            {
                int rng = RandomHolder.Instance.Next(0, 10);
                if(elitePreservation == true)
                {
                    if (j > members.Count / 50 && rng < 5)
                    {
                        members[i].MutateStartingPositions(rowWise, rowConstraints, colConstraints);
                        members[i].FindFitness(rowConstraints, colConstraints);
                    }
                    j++;
                }
                else
                {
                    if (rng < 5)
                    {
                        members[i].MutateStartingPositions(rowWise, rowConstraints, colConstraints);
                        members[i].FindFitness(rowConstraints, colConstraints);
                    }                
                }
                
            }
        }

        public class Expert
        {
            public int i;
            public int j;
            public int weight;
            public Expert(int i, int j, int weight)
            {
                this.i = i;
                this.j = j;
                this.weight = weight;
            }
        }
        //WoC function
        public void ConsultExperts(Population population) //Kaden todo
        {
            int[,] agreement = new int[row,col];
            int count = 0;
            foreach (Member member in members)
            {
                if (count == members.Count * .10)
                    break;
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        if (member.DNA[i, j] == true)
                        {
                            agreement[i, j] += 1;
                        }
                    }
                }
                count++;
            }
            count = 0;
            foreach (Member member in population.members)
            {
                if (count == population.members.Count*.10)
                    break;
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        if (member.DNA[i, j] == true)
                        {
                            agreement[i, j] += 1;
                        }
                    }
                }
                count++;
            }
            List<Expert> experts = new List<Expert>();
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    solution.DNA[i, j] = false;
                    experts.Add(new Expert(i, j, agreement[i, j]));
                }
            }
            
            experts = experts.OrderByDescending(expert => expert.weight).ToList();
            int blackCount = 0;
            foreach(Expert expert in experts)
            {
                if(blackCount == black_squares)
                {
                    break;
                }
                solution.DNA[expert.i, expert.j] = true;
                blackCount++;
            }
            solution.FindFitness(rowConstraints,colConstraints);
        }
    }
}
