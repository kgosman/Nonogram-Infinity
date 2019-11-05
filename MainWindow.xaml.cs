using System;
using System.Collections.Generic;
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
            int numColumns = 20;
            int numRows = 20;

            List<int>[] colConstraints = MakeColConstraints(numColumns);
            List<int>[] rowConstraints = MakeRowConstraints(numRows);

            bool[,] matrix = new bool[numColumns, numRows];
            matrix[1, 2] = true;
            DrawGrid(numColumns, numRows, matrix, colConstraints,rowConstraints);        
        }

        public void DrawGrid(int width, int height, bool[,] matrix, List<int>[] colConstraints, List<int>[] rowConstraints)
        {
            myCanvas.Children.Clear();
            double xSpace = myCanvas.Width / width;
            double ySpace = myCanvas.Height / height;

            LabelColumns(xSpace, colConstraints);
            LabelRows(ySpace, rowConstraints);
            for(int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
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
                    } 
                    Canvas.SetLeft(rectangle, xSpace * j);
                    Canvas.SetTop(rectangle, ySpace * i);
                    myCanvas.Children.Add(rectangle);
                }
            }
        }
        public void LabelColumns(double xSpace,List<int>[] colConstraints)
        {
            for(int i = 0; i < colConstraints.Length;i++)
            {
                if(colConstraints[i].Count == 1)
                {
                    TextBlock textBox = new TextBlock
                    {
                        FontSize = 17.5,
                        Text = colConstraints[i][0].ToString()
                    };
                    Canvas.SetLeft(textBox, xSpace * i + xSpace / 2 - 1);
                    Canvas.SetTop(textBox, 75);
                    colCanvas.Children.Add(textBox);
                }
                else
                {
                    int yOffset = 75;
                    for(int j = 0; j < colConstraints[i].Count; j++)
                    {
                        TextBlock textBox = new TextBlock
                        {
                            FontSize = 17.5,
                            Text = colConstraints[i][j].ToString()
                        };
                        Canvas.SetLeft(textBox, xSpace * i + xSpace/2 - 1);
                        Canvas.SetTop(textBox, yOffset);
                        colCanvas.Children.Add(textBox);
                        yOffset -= 15;                        
                    }
                }
            }
        }
        public void LabelRows(double ySpace, List<int>[] rowConstraints)
        {
            for (int i = 0; i < rowConstraints.Length; i++)
            {
                if (rowConstraints[i].Count == 1)
                {
                    TextBlock textBox = new TextBlock
                    {
                        FontSize = 17.5,
                        Text = rowConstraints[i][0].ToString()
                    };
                    Canvas.SetTop(textBox, ySpace * i + ySpace / 4);
                    Canvas.SetLeft(textBox, 75);
                    rowCanvas.Children.Add(textBox);
                }
                else
                {
                    int xOffset = 75;
                    for (int j = 0; j < rowConstraints[i].Count; j++)
                    {
                        TextBlock textBox = new TextBlock
                        {
                            FontSize = 17.5,
                            Text = rowConstraints[i][j].ToString()
                        };
                        Canvas.SetTop(textBox, ySpace * i + ySpace / 4 );
                        Canvas.SetLeft(textBox, xOffset);
                        rowCanvas.Children.Add(textBox);
                        xOffset -= 15;
                    }
                }
            }
        }

        public List<int>[] MakeColConstraints(int numColumns)
        {
            List<int>[] colConstraints = new List<int>[numColumns];

            for(int i = 0; i < numColumns; i++)
            {
                colConstraints[i] = new List<int>();
            }
            colConstraints[0].Add(0);

            colConstraints[1].Add(2);

            colConstraints[2].Add(3);

            colConstraints[3].Add(2);
            colConstraints[3].Add(2);

            colConstraints[4].Add(1);
            colConstraints[4].Add(5);

            colConstraints[5].Add(2);
            colConstraints[5].Add(5);

            colConstraints[6].Add(1);
            colConstraints[6].Add(4);

            colConstraints[7].Add(1);
            colConstraints[7].Add(1);
            colConstraints[7].Add(5);

            colConstraints[8].Add(1);
            colConstraints[8].Add(2);
            colConstraints[8].Add(4);

            colConstraints[9].Add(1);
            colConstraints[9].Add(8);

            colConstraints[10].Add(2);
            colConstraints[10].Add(1);
            colConstraints[10].Add(5);
            colConstraints[10].Add(2);

            colConstraints[11].Add(2);
            colConstraints[11].Add(4);

            colConstraints[12].Add(3);
            colConstraints[12].Add(5);

            colConstraints[13].Add(2);
            colConstraints[13].Add(4);
            colConstraints[13].Add(1);

            colConstraints[14].Add(2);
            colConstraints[14].Add(4);
            colConstraints[14].Add(2);

            colConstraints[15].Add(7);

            colConstraints[16].Add(5);

            colConstraints[17].Add(2);

            colConstraints[18].Add(0);

            colConstraints[19].Add(0);
            return colConstraints;
        }
        public List<int>[] MakeRowConstraints(int numRows)
        {

            List<int>[] rowConstraints = new List<int>[numRows];
            for (int i = 0; i < numRows; i++)
            {
                rowConstraints[i] = new List<int>();
            }
            rowConstraints[0].Add(0);

            rowConstraints[1].Add(0);

            rowConstraints[2].Add(0);

            rowConstraints[3].Add(0);

            rowConstraints[4].Add(1);
            rowConstraints[4].Add(3);

            rowConstraints[5].Add(2);
            rowConstraints[5].Add(2);
            rowConstraints[5].Add(6);

            rowConstraints[6].Add(1);
            rowConstraints[6].Add(1);
            rowConstraints[6].Add(1);
            rowConstraints[6].Add(4);
            rowConstraints[6].Add(2);

            rowConstraints[7].Add(11);

            rowConstraints[8].Add(11);

            rowConstraints[9].Add(9);

            rowConstraints[10].Add(12);

            rowConstraints[11].Add(5);
            rowConstraints[11].Add(2);
            rowConstraints[11].Add(2);

            rowConstraints[12].Add(4);
            rowConstraints[12].Add(2);

            rowConstraints[13].Add(2);
            rowConstraints[13].Add(1);
            rowConstraints[13].Add(2);

            rowConstraints[14].Add(3);

            rowConstraints[15].Add(4);

            rowConstraints[16].Add(4);

            rowConstraints[17].Add(6);

            rowConstraints[18].Add(0);

            rowConstraints[19].Add(0);

            return rowConstraints;
        }
    }
}

