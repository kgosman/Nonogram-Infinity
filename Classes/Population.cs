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
            members = members.OrderBy(member => member.fitness).ToList();
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

            foreach(Member member in members)
            {
                for(int i = 0; i < row; i++)
                {
                    for(int j = 0; j < col; j++)
                    {
                        if(member.dna[i,j] == true)
                        {
                            agreement[i, j] += 1;
                        }
                    }
                }
            }
            foreach (Member member in population.members)
            {
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        if (member.dna[i, j] == true)
                        {
                            agreement[i, j] += 1;
                        }
                    }
                }
            }
            List<Expert> experts = new List<Expert>();
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
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
                solution.dna[expert.i, expert.j] = true;
                blackCount++;
            }
        }
    }
}
