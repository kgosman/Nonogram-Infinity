using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nonogram_Infinity
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory());
            di = (di.Parent).Parent;
            string picross = "Crown2";
            ReadFile grid = new ReadFile
            {
                filepath = di.FullName + "\\Data\\" + picross + ".txt"
            };
            grid.MakeConstraints();

            WriteFile outfile = new WriteFile
            {
                outFilePath = di.FullName + "\\Data\\" + picross + ".csv"
            };
            outfile.WriteHeaders();

            double xSpace = myRowCanvas.Width / grid.numColumns;
            double ySpace = myRowCanvas.Height / grid.numRows;

            LabelColumns(xSpace, grid.colConstraints, grid.numColumns);
            LabelRows(ySpace, grid.rowConstraints, grid.numRows);

            Population population = new Population(grid.colConstraints, grid.rowConstraints, false);
            Population population2 = new Population(grid.colConstraints, grid.rowConstraints, true);
            
            RunGA(grid, population,population2, outfile);
            //DrawColBoard(grid, population2.members[0], xSpace, ySpace, 0, 0);
            //DrawRowBoard(grid, population2.members[1], xSpace, ySpace, 0);
            //DrawWoC(grid, population2.Breed2(population2.members[0], population2.members[1]), xSpace, ySpace);

        }

        public async void RunGA(ReadFile grid, Population population1, Population population2, WriteFile outfile)
        {
            int prevFitnessWoc = -1,genCount = 1;
            do
            //for(int i = 0; i < 5000; i++)
            {
                await Task.Delay(1);
                population1.ConsultExperts(population2);

                double xSpace = myRowCanvas.Width / grid.numColumns;
                double ySpace = myRowCanvas.Height / grid.numRows;

                double avg1 = 0;
                foreach (Member member in population1.members)
                {
                    avg1 += member.Fitness;
                }
                avg1 /= population1.members.Count;
                DrawColBoard(grid, population1.members[0], xSpace, ySpace, avg1, genCount,population1.members[population1.members.Count-1]);
                double avg2 = 0;
                foreach (Member member in population2.members)
                {
                    avg2 += member.Fitness;
                }
                avg2 /= population2.members.Count;
                DrawRowBoard(grid, population2.members[0], xSpace, ySpace, avg2, population2.members[population2.members.Count - 1]);

                if (population1.solution.Fitness < prevFitnessWoc || prevFitnessWoc == -1)
                {
                    xSpace = wocCanvas.Width / grid.numColumns;
                    ySpace = wocCanvas.Height / grid.numRows;
                    prevFitnessWoc = population1.solution.Fitness;
                    DrawWoC(grid, population1.solution, xSpace, ySpace);
                }
                outfile.WriteToFile(genCount, population1, population2, avg1, avg2);
                population1.BreedPopulaton(true);
                population2.BreedPopulaton(true);

                genCount++;
            } while (population1.solution.Fitness != 0);
        }
        public void DrawColBoard(ReadFile grid, Member member, double xSpace, double ySpace, double avg, int genCount, Member worst)
        {
            myColCanvas.Children.Clear();
            TextBlock textBlock = new TextBlock();
            textBlock.FontSize = 17;
            Canvas.SetLeft(textBlock, 400);
            Canvas.SetTop(textBlock, -20);
            textBlock.Text = "Currently on Generation " + genCount.ToString();
            myColCanvas.Children.Add(textBlock);
    
            for (int j = 0; j < grid.numRows; j++)
            {
                for (int i = 0; i < grid.numColumns; i++)
                {
                    Rectangle rectangle = new Rectangle
                    {
                        Stroke = Brushes.Black,
                        Width = xSpace,
                        Height = ySpace
                    };
                    if(member.DNA[i,j] == true)
                    {
                        rectangle.Fill = Brushes.Black;
                        if(grid.solution[i,j] == true)
                        {
                            rectangle.Fill = Brushes.Lime;
                        }
                    } 
                    Canvas.SetLeft(rectangle, xSpace * j);
                    Canvas.SetTop(rectangle, ySpace * i);
                    myColCanvas.Children.Add(rectangle);
                }
            }
            TextBlock text = new TextBlock();
            text.FontSize = 17;
            Canvas.SetLeft(text, 400);
            text.Text = "Best Column Fitness = " + member.Fitness.ToString();
            myColCanvas.Children.Add(text);
            TextBlock text2 = new TextBlock();
            text2.FontSize = 17;
            Canvas.SetLeft(text2, 400);
            Canvas.SetTop(text2, 20);
            text2.Text = "Worst Column Fitness = " + worst.Fitness.ToString();
            myColCanvas.Children.Add(text2);
            TextBlock text3 = new TextBlock();
            text3.FontSize = 17;
            Canvas.SetLeft(text3, 400);
            Canvas.SetTop(text3, 40);
            text3.Text = "Avg Column Fitness = " + avg.ToString();
            myColCanvas.Children.Add(text3);
        }
        public void DrawRowBoard(ReadFile grid, Member member, double xSpace, double ySpace, double avg, Member worst)
        {
            myRowCanvas.Children.Clear();
                                   
            for (int j = 0; j < grid.numRows; j++)
            {
                for (int i = 0; i < grid.numColumns; i++)
                {
                    Rectangle rectangle = new Rectangle
                    {
                        Stroke = Brushes.Black,
                        Width = xSpace,
                        Height = ySpace
                    };
                    if (member.DNA[i, j] == true)
                    {
                        rectangle.Fill = Brushes.Black;
                        if (grid.solution[i, j] == true)
                        {
                            rectangle.Fill = Brushes.Lime;
                        }
                    }
                    Canvas.SetLeft(rectangle, xSpace * j);
                    Canvas.SetTop(rectangle, ySpace * i);
                    myRowCanvas.Children.Add(rectangle);
                }
            }
            TextBlock text = new TextBlock();
            text.FontSize = 17;
            Canvas.SetLeft(text, 400);
            text.Text = "Best Row Fitness = " + member.Fitness.ToString();
            myRowCanvas.Children.Add(text);
            TextBlock text2 = new TextBlock();
            text2.FontSize = 17;
            Canvas.SetLeft(text2, 400);
            Canvas.SetTop(text2, 20);
            text2.Text = "Worst Row Fitness = " + worst.Fitness.ToString();
            myRowCanvas.Children.Add(text2);
            TextBlock text3 = new TextBlock();
            text3.FontSize = 17;
            Canvas.SetLeft(text3, 400);
            Canvas.SetTop(text3, 40);
            text3.Text = "Avg Row Fitness = " + avg.ToString();
            myRowCanvas.Children.Add(text3);
        }
        public void DrawWoC(ReadFile grid, Member member,double xSpace, double ySpace)
        {
            wocCanvas.Children.Clear();

            for (int j = 0; j < grid.numRows; j++)
            {
                for (int i = 0; i < grid.numColumns; i++)
                {
                    Rectangle rectangle = new Rectangle
                    {
                        Stroke = Brushes.Black,
                        Width = xSpace,
                        Height = ySpace
                    };
                    if (member.DNA[i, j] == true)
                    {
                        rectangle.Fill = Brushes.Black;
                        if (grid.solution[i, j] == true)
                        {
                            rectangle.Fill = Brushes.Lime;
                        }
                    }
                    Canvas.SetLeft(rectangle, xSpace * j);
                    Canvas.SetTop(rectangle, ySpace * i);
                    wocCanvas.Children.Add(rectangle);
                }
            }
            TextBlock text = new TextBlock();
            text.FontSize = 17;
            Canvas.SetTop(text, 600);
            text.Text = "Wisdom of Crowds Fitness = " + member.Fitness.ToString();
            wocCanvas.Children.Add(text);
        }
        public void LabelColumns(double xSpace,List<int>[] colConstraints, int numColumns)
        {
            double wocxSpace = wocCanvas.Width / numColumns;
            for (int i = 0; i < colConstraints.Length;i++)
            {
                if(colConstraints[i].Count == 1)
                {
                    TextBlock textBox = new TextBlock
                    {
                        FontSize = 17.5,
                        Text = colConstraints[i][0].ToString()
                    };
                    Canvas.SetLeft(textBox, xSpace * i + xSpace / 2 - 1);
                    Canvas.SetTop(textBox, 50);
                    colCanvas.Children.Add(textBox);

                    TextBlock textBoxWoC = new TextBlock
                    {
                        FontSize = 17.5,
                        Text = colConstraints[i][0].ToString()
                    };
                    Canvas.SetLeft(textBoxWoC, wocxSpace * i + wocxSpace / 2 - 1);
                    Canvas.SetTop(textBoxWoC, 50);
                    wocColCanvas.Children.Add(textBoxWoC);
                }
                else
                {
                    
                    int yOffset = 50;
                    foreach(int j in colConstraints[i])
                    {
                        TextBlock textBox = new TextBlock
                        {
                            FontSize = 17.5,
                            Text = j.ToString()
                        };
                        Canvas.SetLeft(textBox, xSpace * i + xSpace/2 - 1);
                        Canvas.SetTop(textBox, yOffset);
                        colCanvas.Children.Add(textBox);

                        TextBlock textBoxWoC = new TextBlock
                        {
                            FontSize = 17.5,
                            Text = j.ToString()
                        };
                        Canvas.SetLeft(textBoxWoC, wocxSpace * i + wocxSpace / 2 - 1);
                        Canvas.SetTop(textBoxWoC, yOffset);
                        wocColCanvas.Children.Add(textBoxWoC);
                        yOffset -= 15;                        
                    }
                }
                colConstraints[i].Reverse();
            }
        }
        public void LabelRows(double ySpace, List<int>[] rowConstraints, int numRows)
        {
            double wocySpace = wocCanvas.Height / numRows;

            for (int i = 0; i < rowConstraints.Length; i++)
            {
                if (rowConstraints[i].Count == 1)
                {
                    TextBlock textBox = new TextBlock
                    {
                        FontSize = 17.5,
                        Text = rowConstraints[i][0].ToString()
                    };
                    Canvas.SetTop(textBox, ySpace * i + ySpace / 8);
                    Canvas.SetLeft(textBox, 50);
                    rowCanvas.Children.Add(textBox);

                    TextBlock textBoxWoC = new TextBlock
                    {
                        FontSize = 17.5,
                        Text = rowConstraints[i][0].ToString()
                    };
                    Canvas.SetTop(textBoxWoC, wocySpace * i + wocySpace / 4);
                    Canvas.SetLeft(textBoxWoC, 50);
                    wocRowCanvas.Children.Add(textBoxWoC);
                }
                else
                {
                    int xOffset = 50;
                    foreach(int j in rowConstraints[i])
                    {
                        TextBlock textBox = new TextBlock
                        {
                            FontSize = 17.5,
                            Text = j.ToString()
                        };
                        Canvas.SetTop(textBox, ySpace * i + ySpace / 8);
                        Canvas.SetLeft(textBox, xOffset);
                        rowCanvas.Children.Add(textBox);

                        TextBlock textBoxWoC = new TextBlock
                        {
                            FontSize = 17.5,
                            Text = j.ToString()
                        };
                        Canvas.SetTop(textBoxWoC, wocySpace * i + wocySpace / 4);
                        Canvas.SetLeft(textBoxWoC, xOffset);
                        wocRowCanvas.Children.Add(textBoxWoC);
                        xOffset -= 15;
                    }
                }
                rowConstraints[i].Reverse();
            }
        }
    }
}