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

            ReadFile grid = new ReadFile
            {
                filepath = di.FullName + "\\Data\\Lizard.txt"
            };
            grid.MakeConstraints();

            Population population = new Population(grid.colConstraints, grid.rowConstraints);

            double xSpace = myRowCanvas.Width / grid.numColumns;
            double ySpace = myRowCanvas.Height / grid.numRows;

            LabelColumns(xSpace, grid.colConstraints, grid.numColumns);
            LabelRows(ySpace, grid.rowConstraints, grid.numRows);

            DrawColBoard(grid, population.members[0].DNA,xSpace,ySpace);
            DrawRowBoard(grid, population.members[1].DNA, xSpace, ySpace);

            xSpace = wocCanvas.Width / grid.numColumns;
            ySpace = wocCanvas.Height / grid.numRows;

            DrawWoC(grid,grid.solution, xSpace, ySpace);
        }

        public void DrawColBoard(ReadFile grid, bool[,] matrix, double xSpace, double ySpace)
        {
            myColCanvas.Children.Clear();

            for(int j = 0; j < grid.numRows; j++)
            {
                for (int i = 0; i < grid.numColumns; i++)
                {
                    Rectangle rectangle = new Rectangle
                    {
                        Stroke = Brushes.Black,
                        Width = xSpace,
                        Height = ySpace
                    };
                    if(matrix[i,j] == true)
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
        }
        public void DrawRowBoard(ReadFile grid, bool[,] matrix, double xSpace, double ySpace)
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
                    if (matrix[i, j] == true)
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
        }
        public void DrawWoC(ReadFile grid, bool[,] matrix,double xSpace, double ySpace)
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
                    if (matrix[i, j] == true)
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