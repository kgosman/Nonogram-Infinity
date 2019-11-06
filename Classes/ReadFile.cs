using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Nonogram_Infinity
{
    public class ReadFile
    {
        public string filepath;
        public int numColumns;
        public int numRows;
        public bool[,] solution;
        public List<int>[] colConstraints;
        public List<int>[] rowConstraints;

        public ReadFile()
        {
            this.filepath = "";
            this.numColumns = 0;
            this.numRows = 0;
        }

        public void MakeConstraints()
        {
            string line;
            StreamReader file = new StreamReader(filepath);
            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains("Dimension Row: ") == true)
                {
                    numRows = int.Parse(line.Substring(("Dimension Row: ").Length));
                    break;
                }
            }

            rowConstraints = new List<int>[numRows];

            line = file.ReadLine();
            int i = 0;
            while (!(line = file.ReadLine()).Contains("Dimension Col:"))
            {
                line = line.TrimEnd('}');
                line = line.TrimStart('{');
                rowConstraints[i] = new List<int>();

                if (line.Contains(",") == true)
                {
                    string[] numbers = line.Split(',');
                    foreach (string number in numbers)
                    {
                        rowConstraints[i].Add(Convert.ToInt32(number));
                    }
                }
                else
                {
                    rowConstraints[i].Add(int.Parse(line));
                }
                i++;
            }

            numColumns = int.Parse(line.Substring(("Dimension Col: ").Length));

            colConstraints = new List<int>[numColumns];

            line = file.ReadLine();
            int j = 0;
            while (!(line = file.ReadLine()).Contains("Solution:"))
            {
                line = line.TrimEnd('}');
                line = line.TrimStart('{');
                colConstraints[j] = new List<int>();

                if (line.Contains(",") == true)
                {
                    string[] numbers = line.Split(',');
                    foreach (string number in numbers)
                    {
                        colConstraints[j].Add(Convert.ToInt32(number));
                    }
                }
                else
                {
                    colConstraints[j].Add(int.Parse(line));
                }
                j++;
            }
            this.solution = new bool[numColumns,numRows];
            int m = 0;
            while((line = file.ReadLine()) != "")
            {
                int n = 0;
                string[] lineAsStrings = line.Split(',');
                foreach (string strings in lineAsStrings)
                {
                    if(strings == "0")
                    {
                        solution[m, n] = false;
                    }
                    else
                    {
                        solution[m, n] = true;
                    }
                    n++;
                }
                m++;
            }
            file.Close();
        }
    }
}
