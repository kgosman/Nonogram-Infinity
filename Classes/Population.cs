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
            rowWise = type;
            columnWise = !type;
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
                members.Add(new Member(row, col, black_squares));
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
            if(rowWise)
            {

            }
            else if(columnWise)
            {

            }
            return new Member(row, col, black_squares);
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
                child.FindFitness();
                members.Add(child);
            }
            MutatePopulation(elitePreservation);
            members = members.OrderBy(member => member.fitness).ToList();
        }
        //Breed top 50% discard bottom 50% add the 25% from the resultant breeding and introduce 25% new
     
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



        }
    }
}
