using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Nonogram_Infinity
{
    public class WriteFile
    {
        public string outFilePath;
        public WriteFile()
        {
            outFilePath = "";
        }
        //Write to set output file
        public void WriteHeaders()
        {
            using (StreamWriter writer = new StreamWriter(outFilePath, false))
            {
                writer.WriteLine("Generation, Best # of Correct Squares (Column),Average Column Fitness,Best Column Fitness,Worst Column Fitness,Best # of Correct Squares (Row),Average Row Fitness,Best Row Fitness,Worst Row Fitness,Best # of Correct Squares (WoC),WoC Fitness");
            }
        }
        public void WriteToFile(int genCount, Population population, Population population2, double avg1, double avg2, int correctCol, int correctRow, int correctWoC)
        {
            using (StreamWriter writer = new StreamWriter(outFilePath, true))
            {
                writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", genCount.ToString(), correctCol.ToString(),avg1.ToString(), population.members[0].Fitness.ToString(), population.members[population.members.Count - 1].Fitness.ToString(), correctRow.ToString(), avg2.ToString(), population2.members[0].Fitness.ToString(), population2.members[population2.members.Count - 1].Fitness.ToString(), correctWoC.ToString(), population.solution.Fitness.ToString());
            }
        }
        //Convert a member to a formated string 
        public string ConvertMemberToString(Member member, bool best)
        {
            string memberAsString;
            if (best == true)
            {
                memberAsString = "Best Fitness: " + member.Fitness.ToString();
            }
            else
            {
                memberAsString = "Worst Fitness: " + member.Fitness.ToString();
            }
            return memberAsString;
        }
    }
}
