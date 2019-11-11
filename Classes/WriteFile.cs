//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.IO;

//namespace Nonogram_Infinity
//{
//    public class WriteFile
//    {
//        public string outFilePath;
//        public WriteFile()
//        {
//            outFilePath = "";
//        }
//        //Write to set output file
//        public void WriteHeaders()
//        {
//            using (StreamWriter writer = new StreamWriter(outFilePath, true))
//            {
//                writer.WriteLine("Generation,Average Fitness,Best Fitness,Worst Fitness,WoC Fitness");
//            }
//        }
//        public void WriteToFile(Population population, int i)
//        {
//            double totalFitness = 0.00;
//            foreach (Member member in population.members)
//            {
//                totalFitness += member.fitness;
//            }
//            totalFitness /= population.members.Count;
//            using (StreamWriter writer = new StreamWriter(outFilePath, true))
//            {
//                //string line = ConvertMemberToString(population.members[0], true);
//                //string line2 = ConvertMemberToString(population.members[population.members.Count - 1], false);
//                writer.WriteLine("{0},{1},{2},{3},{4}",i.ToString(),totalFitness.ToString(),population.members[0].fitness.ToString(),population.members[population.members.Count-1].fitness.ToString(), population.solution.fitness.ToString());
//            }
//            //}
//            //    using (StreamWriter writer = new StreamWriter(outFilePath,true))
//            //    {
//            //        writer.WriteLine("Generation " + i.ToString() + " Had an Average Fitness of: " + totalFitness.ToString());
//            //        string line = ConvertMemberToString(population.members[0],true);
//            //        string line2 = ConvertMemberToString(population.members[population.members.Count-1], false);
//            //        writer.WriteLine(line);
//            //        writer.WriteLine(line2);
//            //    }
//        }
//        //Convert a member to a formated string 
//        public string ConvertMemberToString(Member member, bool best)
//        {
//            string memberAsString;
//            if( best == true){
//                memberAsString = "Best Fitness: " + member.fitness.ToString() + " DNA: { ";
//            }
//            else
//            {
//                memberAsString = "Worst Fitness: " + member.fitness.ToString() + " DNA: { ";
//            }
//            foreach (City city in member.dna)
//            {
//                memberAsString += (city.id).ToString() + " ";
//            }
//            memberAsString += "}";
//            return memberAsString;
//        }
//        public void WriteAlt(Population population)
//        {
//            using (StreamWriter writer = new StreamWriter(outFilePath, true))
//            {
//                //foreach (Member member in population.experts)
//                //{
//                //    string memberAsString = "{ ";
//                //    foreach (City city in member.dna)
//                //    {
//                //        memberAsString += (city.id).ToString() + " ";
//                //    }
//                //    memberAsString += "}";
//                //    writer.WriteLine(memberAsString);
//                //}
//                string solutionAsString = "Solutions Came Up With {";
//                foreach (City city in population.solution.dna)
//                {
//                    solutionAsString += (city.id).ToString() + " ";
//                }
//                solutionAsString += "} With Fitness of:";
//                solutionAsString += population.solution.fitness.ToString();
//                writer.WriteLine(solutionAsString);
//            }
//        }
//    }
//}
