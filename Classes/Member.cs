using System.Collections.Generic;
using System.Linq;

namespace Nonogram_Infinity
{
    public class Member
    {
        public int Fitness { get; private set; }
        public bool[,] DNA { get; private set; }
        private readonly int Row;
        private readonly int Column;

        public int[] RowFitness { get; private set; }
        public int[] ColumnFitness { get; private set; }
        public Member(int row, int col)
        {
            Fitness = 0;
            Row = row;
            Column = col;
            DNA = new bool[Row, Column];
            RowFitness = new int[Row];
            ColumnFitness = new int[Column];
        }
        public Member (int Row, int Column, int BlackSquares, List<int>[] RowRules, List<int>[] ColumnRules)
        {
            Fitness = 0;
            this.Row = Row;
            this.Column = Column;
            RowFitness = new int[Row];
            ColumnFitness = new int[Column];

            DNA = new bool[this.Row, this.Column];

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

        public Member(int Row, int Column, List<int>[] RowRules, List<int>[] ColumnRules, bool[,] DNA)
        {
            Fitness = 0;
            this.Row = Row;
            this.Column = Column;
            RowFitness = new int[Row];
            ColumnFitness = new int[Column];

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
            Stack<bool> currentStack = new Stack<bool>();

            for (int i = 0; i < Row; i++)
            {
                int fitness = 0;
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
                    currentState = StateMachine.OtherDelta(currentState, DNA[i, j], ref currentStack);

                    if (currentState == State.RULE_BROKEN)
                    {
                        fitness++;
                        currentState = State.COUNTING_BLACK_RUNS;
                    }

                    if (currentState == State.COUNTING_BLACK_RUNS)
                        totalBlackSquares--;
                }

                while (currentStack.Count != 0)
                    currentStack.Pop();

                if (totalBlackSquares < 0)
                    totalBlackSquares *= -1;

                fitness += totalBlackSquares;
                RowFitness[i] = fitness;
            }
        }

        private void ColumnWiseFitness(List<int>[] ColumnRules)
        {
            int fitness;

            Stack<bool> currentStack = new Stack<bool>();

            for (int j = 0; j < Column; j++)
            {
                fitness = 0;

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
                    currentState = StateMachine.OtherDelta(currentState, DNA[i, j], ref currentStack);

                    if (currentState == State.RULE_BROKEN)
                    {
                        fitness++;
                        currentState = State.COUNTING_BLACK_RUNS;
                    }

                    if (currentState == State.COUNTING_BLACK_RUNS)
                        totalBlackSquares--;
                }

                while (currentStack.Count != 0)
                {
                    currentStack.Pop();
                }

                if (totalBlackSquares < 0)
                    totalBlackSquares *= -1;

                fitness += totalBlackSquares;
                
                ColumnFitness[j] = fitness;
            }
        }

        /// <summary>
        /// Copies <paramref name="copyDNA"/> into DNA
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
            if ((currentState == State.INITIAL_STATE || currentState == State.COUNTING_RUNS || currentState == State.NO_BLACK_SQAURES_ENCOUNTERED)
                    && value == currentStack.Peek() && !currentStack.Peek())
                currentState = State.INITIAL_STATE;

            else if (currentState == State.COUNTING_RUNS && currentStack.Peek() != value && 
                    ((currentStack.Peek() && !value) || (!currentStack.Peek() && value)))
            {
                currentState = State.RULE_BROKEN;

                currentStack.Pop();
            }

            else if (currentState == State.COUNTING_RUNS
                    && currentStack.Peek() == value && currentStack.Peek())
            {
                currentState = State.COUNTING_RUNS;

                currentStack.Pop();
            }

            else if (currentState == State.INITIAL_STATE && value != currentStack.Peek() && !currentStack.Peek())
            {
                currentState = State.COUNTING_RUNS;
                currentStack.Pop();

                if (currentStack.Count() > 0)
                    currentStack.Pop();
                else
                    currentState = State.RULE_BROKEN;
            }

            else if (currentState == State.INITIAL_STATE && value == currentStack.Peek() && currentStack.Peek())
            {
                currentState = State.COUNTING_RUNS;

                currentStack.Pop();
            }

            else if (currentState == State.RULE_BROKEN && currentStack.Count > 0 ? value == currentStack.Peek() : false && value)
                currentState = State.COUNTING_RUNS;

            else if (currentState == State.RULE_BROKEN && currentStack.Count > 0 ? value == currentStack.Peek() : false && !value)
                currentState = State.INITIAL_STATE;

            return currentState;
        }

        public static State OtherDelta(State currentState, bool value, ref Stack<bool> currentStack)
        {
            if (currentState == State.COUNTING_BLACK_RUNS && currentStack.Count == 0 && value)
            {
                currentState = State.RULE_BROKEN;
            }
            else if (currentState == State.COUNTING_WHITE_RUNS && currentStack.Count == 0 && !value)
            {
                currentState = State.COUNTING_WHITE_RUNS;
            }
            else if (currentState == State.COUNTING_WHITE_RUNS && currentStack.Count == 0 && value)
            {
                currentState = State.RULE_BROKEN;
            }
            else if (currentState == State.INITIAL_STATE && value == currentStack.Peek() && value)
            {
                currentState = State.COUNTING_BLACK_RUNS;

                currentStack.Pop();
            }
            else if (currentState == State.INITIAL_STATE && value == currentStack.Peek() && !value)
            {
                currentState = State.COUNTING_WHITE_RUNS;

                currentStack.Pop();
            }
            else if (currentState == State.INITIAL_STATE && value != currentStack.Peek() && value)
            {
                currentState = State.RULE_BROKEN;
            }
            else if (currentState == State.COUNTING_BLACK_RUNS && value == currentStack.Peek() && value)
            {
                currentState = State.COUNTING_BLACK_RUNS;

                currentStack.Pop();
            }
            else if (currentState == State.COUNTING_WHITE_RUNS && value == currentStack.Peek() && !value)
            {
                currentState = State.COUNTING_WHITE_RUNS;
            }
            else if (currentState == State.COUNTING_BLACK_RUNS && value == currentStack.Peek() && !value)
            {
                currentState = State.COUNTING_WHITE_RUNS;

                currentStack.Pop();
            }
            else if (currentState == State.COUNTING_BLACK_RUNS && value != currentStack.Peek() && value)
            {
                currentState = State.RULE_BROKEN;

                currentStack.Pop();
            }
            else if (currentState == State.COUNTING_BLACK_RUNS && value != currentStack.Peek() && !value)
            {
                currentState = State.COUNTING_WHITE_RUNS;

                currentStack.Pop();
            }
            else if (currentState == State.COUNTING_WHITE_RUNS && value == currentStack.Peek() && value)
            {
                currentState = State.COUNTING_BLACK_RUNS;

                currentStack.Pop();
            }

            return currentState;
        }
    }

    enum State
    {
        NO_BLACK_SQAURES_ENCOUNTERED,
        INITIAL_STATE,
        COUNTING_RUNS,
        RULE_BROKEN,
        COUNTING_BLACK_RUNS,
        COUNTING_WHITE_RUNS
    }
}