using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5TSPGA
{
    public class Population
    {
        public List<City> cities = new List<City>();
        public List<Member> members;
        public List<Member> experts;
        public int dimension;
        public double[,] matrix;
        public Member solution = new Member();
        public Population()
        {
            this.members = new List<Member>(100);
            this.experts = new List<Member>(50);
        }
        public void CreatePopulation()
        {
            int i = 0;
            while (i < 100)
            {
                Member member = new Member();
                member.Clone(cities);
                member.RandomizeDNA();
                member.dna.Add(member.dna[0]);
                member.FindFitness();
                members.Add(member);
                i++;
            }
            members = members.OrderBy(member => member.fitness).ToList();
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
        public Member Breed(Member mother, Member father)
        {
            Member child = new Member();

            Member childMaternal = new Member();
            childMaternal.Clone(mother.dna);
            childMaternal.dna.RemoveAt(childMaternal.dna.Count - 1);

            Member childPaternal = new Member();
            childPaternal.Clone(father.dna);
            childPaternal.dna.RemoveAt(childPaternal.dna.Count - 1);

            int splicePointA = RandomHolder.Instance.Next(1, childMaternal.dna.Count);
            int splicePointB = RandomHolder.Instance.Next(1, childMaternal.dna.Count);

            if(splicePointA == splicePointB)
            {
                if(splicePointB == 0)
                {
                    splicePointB += 1;
                }
                else
                {
                    splicePointB -= 1;
                }
            }
            int start = Math.Min(splicePointA, splicePointB);
            int stop = Math.Max(splicePointA, splicePointB);

            for (int i = start; i < stop; i++)
            {
                child.dna.Add(childMaternal.dna[i]);
            }

            foreach (City city in child.dna)
            {
                int j = 0;
                while (j < childPaternal.dna.Count)
                {
                    if (city.id == childPaternal.dna[j].id)
                    {
                        childPaternal.dna.RemoveAt(j);
                        break;
                    }
                    j++;
                }
            }
            for (int i = 0; i < childPaternal.dna.Count; i++)
            {
                child.dna.Add(childPaternal.dna[i]);
            }
            child.dna.Add(child.dna[0]);
            return child;
        }
        //Breed top 50% discard bottom 25% and replace with 25% from the resultant breeding
        public void BreedPopulaton(bool elitePreservation)
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
        public void BreedPopulatonAlt(bool elitePreservation)
        {
            List<Member> offspring = new List<Member>();

            int j = 0;
            for (int i = 0; i < members.Count / 4; i++)
            {
                offspring.Add(Breed(members[j], members[j + 1]));
                j += 2;
            }
            for (int i = 0; i < offspring.Count*2; i++)
            {
                members.RemoveAt(members.Count - 1);
            }
            int k = 0;
            while (k < members.Count/2)
            {
                Member member = new Member();
                member.Clone(cities);
                member.RandomizeDNA();
                member.dna.Add(member.dna[0]);
                offspring.Add(member);
                k++;
            }
            foreach (Member child in offspring)
            {
                child.FindFitness();
                members.Add(child);
            }
            MutatePopulation(elitePreservation);
            members = members.OrderBy(member => member.fitness).ToList();
        }
        //Function to mutate a member with 2% chance
        public void Mutate(Member member)
        {
            if(RandomHolder.Instance.Next(1,100) < 3)
            {
                int pos1 = RandomHolder.Instance.Next(1, member.dna.Count - 2);
                City tmp = member.dna[pos1];
                //int pos2 = System.Convert.ToInt32(tmp.id);
                int pos2 = RandomHolder.Instance.Next(1, member.dna.Count - 2);
                if(pos1 == pos2)
                {
                    pos2 += 1;
                }
                member.dna[pos1] = member.dna[pos2];
                member.dna[pos2] = tmp;
                member.FindFitness();
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
        public void ConsultExperts()
        {
            //Create experts
            matrix = new double[dimension + 1, dimension + 1];
            //for(int i = 0; i < dimension + 1; i++)
            //{
            //    for(int j = 0; j < dimension + 1; j++)
            //    {
            //        matrix[i, j] = 0;
            //    }
            //}
            experts = new List<Member>();
            for (int i = 0; i < members.Count / 2; i++)
            {
                Member expert = new Member();
                expert.Clone(members[i].dna);
                experts.Add(expert);
            }
            //Make matrix
            foreach (Member expert in experts)
            {
                //expert.dna.RemoveAt(expert.dna.Count - 1);
                int i = 0, j = 1;
                while (j < expert.dna.Count)
                {
                    matrix[System.Convert.ToInt32(expert.dna[i].id) - 1, System.Convert.ToInt32(expert.dna[j].id) - 1] += 1;
                    i++;
                    j++;
                }
            }

            //Find globaly most common agreement and also build cost matrix with beta func
            int highestI = 0, highestJ = 0;
            double previousMax = 0.0;

            for (int i = 0; i < dimension + 1; i++)          //Col
            {
                for (int j = 0; j < dimension + 1; j++)      //Row
                {
                    if (matrix[i, j] != 0)
                    {
                       // matrix[i, j] = (SpecialFunctions.BetaRegularized(3, 3, (matrix[i, j] / 50)));
                    }
                    if (matrix[i, j] > previousMax)
                    {
                        highestI = i;
                        highestJ = j;
                        previousMax = matrix[i, j];
                    }
                }
            }

            solution.dna.Clear();
            //Build Solution
            solution.dna.Add(cities[highestI]);
            //solution.dna.Add(members[0].dna[0]);
            //int x = (System.Convert.ToInt32(solution.dna[0].id) - 1);
            //for (int j = 0; j < dimension; j++)
            //{
            //    if (matrix[x, j] > previousMax)
            //    {
            //        highestJ = j;
            //        previousMax = matrix[x, j];
            //    }
            //}
            int m = highestJ, n = 0;
            //int m = (System.Convert.ToInt32(solution.dna[0].id)-1), n = 0;
            int nextN = n;
            while (solution.dna.Count < dimension)
            {
                solution.dna.Add(cities[m]);
                previousMax = 0.0;
                for (n = 0; n < dimension + 1; n++)         //Row
                {
                    //if (m != n && ((matrix[m, n] > previousMax && matrix[m, n] > 0) || previousMax == 0.0))

                    if ((matrix[m, n] > previousMax))
                    {
                        nextN = n;
                        previousMax = matrix[m, n];
                        continue;
                    }
                    if ((matrix[m, n] == previousMax))
                    {
                        if (solution.dna.Contains(cities[nextN]))
                        {
                            nextN = n;
                        }
                    }
                }
                if (!solution.dna.Contains(cities[m]))      //Col
                {
                    m = nextN;
                }
                else
                {
                    int closestCity = m;
                    double closestDistance = 0.0;
                    City cityAtM = new City();
                    cityAtM.Clone(cities[m]);
                    foreach (City city in cities)
                    {
                        if (!solution.dna.Contains(city))
                        {
                            cityAtM.DistanceToCity(city);
                            if (cityAtM.distance < closestDistance || closestDistance == 0.0)
                            {
                                closestCity = System.Convert.ToInt32(city.id) - 1;
                                closestDistance = cityAtM.distance;
                            }
                        }
                    }
                    m = closestCity;
                    if (m == dimension + 1)
                    {
                        m = 0;
                    }
                }
            }
            solution.dna.Add(solution.dna[0]);
            solution.FindFitness();
        }
    }
}
