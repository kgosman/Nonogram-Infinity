using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5TSPGA
{
    public class Member
    {
        public bool expert;
        public double fitness;
        public List<City> dna;
        public Member ()
        {
            this.expert = false;
            this.fitness = 0.0;
            this.dna = new List<City>();
        }
        //Calculate fitness of the dna
        public void FindFitness()
        {
            this.fitness = 0;
            for(int i = 0; i+1 < dna.Count; i++)
            {
                dna[i].DistanceToCity(dna[i + 1]);
                fitness += dna[i].distance;
            }
        }
        //Clone a members dna
        public void Clone(List<City> myCities)
        {
            foreach(City city in myCities)
            {
                dna.Add(city);
            }
        }
        //Shuffle the DNA
        public void RandomizeDNA()
        {
            int i = dna.Count;
            while (i > 1)
            {
                i--;
                int j = RandomHolder.Instance.Next(i + 1);
                City tmpCity = dna[j];
                dna[j] = dna[i];
                dna[i] = tmpCity;
            }
        }
    }
}