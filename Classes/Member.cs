using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonogram_Infinity
{
    public class Member
    {
        public int Fitness { get; private set; }
        public bool[,] DNA { get; private set; }
        private readonly int Row;
        private readonly int Column;

        public List<int> RowFitness { get; private set; }
        public List<int> ColumnFitness { get; private set; }
        public Member(int row, int col)
        {
            Fitness = 0;
            this.Row = row;
            this.Column = col;
            this.DNA = new bool[this.Row, this.Column];
            RowFitness = new List<int>(Row);
            for(int i = 0; i < Row; i++)
            {
                RowFitness.Add(0);
            }
            ColumnFitness = new List<int>(Column);
            for (int i = 0; i < Column; i++)
            {
                ColumnFitness.Add(0);
            }
        }
        public Member (int Row, int Column, int BlackSquares, List<int>[] RowRules, List<int>[] ColumnRules)
        {
            Fitness = 0;
            this.Row = Row;
            this.Column = Column;

            RowFitness = new List<int>(Row);

            ColumnFitness = new List<int>(Column);

            this.DNA = new bool[this.Row, this.Column];

            int l, k, rng;
            int[] array = new int[Row * Column];
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    DNA[i, j] = false;
                    array[Column * i + j] = Column * i + j;
                }
            }
            int tmp;
            for (int i = 0; i < BlackSquares; i++)
            {
                rng = RandomHolder.Instance.Next(0, Row * Column - i);
                tmp = array[rng];
                array[rng] = array[Row * Column - i - 1];
                array[Row * Column - i - 1] = tmp;
                l = tmp / Column;
                k = tmp % Column;
                DNA[l, k] = true;

            }

            FindFitness(RowRules, ColumnRules);
        }

        public Member(int Row, int Column, int BlackSquares, List<int>[] RowRules, List<int>[] ColumnRules, bool[,] DNA)
        {
            Fitness = 0;
            this.Row = Row;
            this.Column = Column;

            RowFitness = new List<int>(Row);

            ColumnFitness = new List<int>(Column);

            this.DNA = DNA;

            FindFitness(RowRules, ColumnRules);
        }

        /// <summary>
        /// Calculates the fitness of a given member,
        /// this calculation is done by determining
        /// if the runs of black squares adhere to
        /// the <paramref name="ColumnRules"/> and
        /// <paramref name="RowRules"/>. It also
        /// will take into account whether or not the
        /// total number of black squares adhere
        /// to the rules provided.
        /// </summary>
        /// <param name="RowRules"></param>
        /// <param name="ColumnRules"></param>
        public void FindFitness(List<int>[] RowRules, List<int>[] ColumnRules) //Will todo
        {
            RowWiseFitness(RowRules);
            ColumnWiseFitness(ColumnRules);

            foreach (int fitness in RowFitness)
                Fitness += fitness;

            foreach (int fitness in ColumnFitness)
                Fitness += fitness;
        }

        private void RowWiseFitness(List<int>[] RowRules)
        {
            int fitness = 0;

            Stack<bool> currentStack = new Stack<bool>();

            for (int i = 0; i < Row; i++)
            {
                State currentState = State.INITIAL_STATE;

                int totalBlackSquares = 0;

                for (int j = RowRules[i].Count - 1; j >= 0; j--)
                {
                    currentStack.Push(false);

                    for (int k = 0; k < RowRules[i][j]; k++)
                    {
                        currentStack.Push(true);
                        totalBlackSquares++;
                    }
                }

                for (int j = 0; j < Column; j++)
                {
                    currentState = StateMachine.Delta(currentState, DNA[i, j], ref currentStack);

                    if (currentState == State.RULE_BROKEN)
                        fitness++;

                    if (currentState == State.COUNTING_RUNS)
                        totalBlackSquares--;
                }

                while (currentStack.Count != 0)
                {
                    if (currentStack.Peek())
                        fitness++;

                    currentStack.Pop();
                }

                if (totalBlackSquares < 0)
                    totalBlackSquares *= -1;

                fitness += totalBlackSquares;
                RowFitness.Add(fitness);
            }
        }

        private void ColumnWiseFitness(List<int>[] ColumnRules)
        {
            int fitness = 0;

            Stack<bool> currentStack = new Stack<bool>();

            for (int j = 0; j < Column; j++)
            {
                State currentState = State.INITIAL_STATE;

                int totalBlackSquares = 0;

                for (int i = ColumnRules[j].Count - 1; i >= 0; i--)
                {
                    currentStack.Push(false);

                    for (int k = 0; k < ColumnRules[j][i]; k++)
                    {
                        currentStack.Push(true);
                        totalBlackSquares++;
                    }
                }

                for (int i = 0; i < Row; i++)
                {
                    currentState = StateMachine.Delta(currentState, DNA[i, j], ref currentStack);

                    if (currentState == State.RULE_BROKEN)
                        fitness++;

                    if (currentState == State.COUNTING_RUNS)
                        totalBlackSquares--;
                }

                while (currentStack.Count != 0)
                {
                    if (currentStack.Peek())
                        fitness++;

                    currentStack.Pop();
                }

                if (totalBlackSquares < 0)
                    totalBlackSquares *= -1;

                fitness += totalBlackSquares;

                ColumnFitness.Add(fitness);
            }
        }

        /// <summary>
        /// Copies input into DNA
        /// </summary>
        /// <param name="copyDNA"></param>
        public void Clone(bool[,] copyDNA)
        {
            DNA = copyDNA;
        }

         ///<summary>
         ///Mutates a given member by columns or rows
         ///depending on the population and if a given
         ///row or column is at a fitness = 0;
         ///</summary>
        internal void Mutate(bool rowWise, bool columnWise)
        {
            if (rowWise)
            {
                int row1Index = 0, row2Index = 1;

                bool row1Okay = false, row2Okay = false;

                while (true)
                {
                    if (!row1Okay) row1Index = RandomHolder.Instance.Next(0, Row);

                    if (!row2Okay) row2Index = RandomHolder.Instance.Next(0, Row);

                    if (RowFitness[row1Index] > 0) row1Okay = true;

                    if (RowFitness[row2Index] > 0) row2Okay = true;

                    if (row1Index == row2Index) row1Okay = row2Okay = false;

                    if (row1Okay && row2Okay) break;
                }

                bool[] row1;
                bool[] row2 = row1 = new bool[Column];

                for (int i = 0; i < Column; i++)
                {
                    row1[i] = DNA[row1Index, i];
                    row2[i] = DNA[row2Index, i];
                }

                for (int i = 0; i < Column; i++)
                {
                    DNA[row1Index, i] = row2[i];
                    DNA[row2Index, i] = row1[i];
                }
            }

            if (columnWise)
            {
                int col1Index = 0, col2Index = 1;

                bool col1Okay = false, col2Okay = false;

                while (true)
                {
                    if (!col1Okay) col1Index = RandomHolder.Instance.Next(0, Column);

                    if (!col2Okay) col2Index = RandomHolder.Instance.Next(0, Column);

                    if (ColumnFitness[col1Index] > 0) col1Okay = true;

                    if (ColumnFitness[col2Index] > 0) col2Okay = true;

                    if (col1Index == col2Index) col1Okay = col2Okay = false;

                    if (col1Okay && col2Okay) break;
                }

                bool[] col1;
                bool[] col2 = col1 = new bool[Row];

                for (int i = 0; i < Row; i++)
                {
                    col1[i] = DNA[i, col1Index];
                    col2[i] = DNA[i, col2Index];
                }

                for (int i = 0; i < Row; i++)
                {
                    DNA[i, col1Index] = col2[i];
                    DNA[i, col2Index] = col1[i];
                }
            }
        }
    }

    class StateMachine
    {
        public static State Delta(State currentState, bool value, ref Stack<bool> currentStack)
        {
            if ((currentState == State.INITIAL_STATE || currentState == State.COUNTING_RUNS) 
                    && value == currentStack.Peek() && !currentStack.Peek())
                currentState = State.INITIAL_STATE;

            if (currentState == State.COUNTING_RUNS && currentStack.Peek() != value)
            {
                currentState = State.RULE_BROKEN;

                currentStack.Pop();
            }

            if (currentState == State.COUNTING_RUNS
                    && currentStack.Peek() == value && currentStack.Peek())
            {
                currentState = State.COUNTING_RUNS;

                currentStack.Pop();
            }

            if (currentState == State.INITIAL_STATE && value != currentStack.Peek() && !currentStack.Peek())
            {
                currentState = State.COUNTING_RUNS;
                currentStack.Pop();

                if (currentStack.Count() > 0)
                    currentStack.Pop();
                else
                    currentState = State.RULE_BROKEN;
            }

            if (currentState == State.INITIAL_STATE && value == currentStack.Peek() && currentStack.Peek())
            {
                currentState = State.COUNTING_RUNS;

                currentStack.Pop();
            }

            return currentState;
        }
    }

    enum State
    {
        INITIAL_STATE,
        COUNTING_RUNS,
        RULE_BROKEN
    }
}