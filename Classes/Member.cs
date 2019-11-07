using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonogram_Infinity
{
    public class Member
    {
        public int fitness;
        public bool[,] dna;
        private int row;
        private int col;

        public int RowFitness { get; private set; }
        public int ColumnFitness { get; private set; }
        public Member (int row, int col, int black_squares, List<int>[] rowRules, List<int>[] columnRules, bool[,] dna = null)
        {
            this.fitness = 0;
            this.row = row;
            this.col = col;

            if (dna == null)
            {
                this.dna = new bool[this.row, this.col];


                int l, k, rng;
                int[] array = new int[row * col];
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        dna[i, j] = false;
                        array[col * i + j] = col * i + j;
                    }
                }
                int tmp;
                for (int i = 0; i < black_squares; i++)
                {
                    rng = RandomHolder.Instance.Next(0, row * col - i);
                    tmp = array[rng];
                    array[rng] = array[row * col - i - 1];
                    array[row * col - i - 1] = tmp;
                    l = tmp / col;
                    k = tmp % col;
                    dna[l, k] = true;

                }
            }
            else
                this.dna = dna;

            FindFitness(rowRules, columnRules);
        }
        //Calculate fitness of the dna
        public void FindFitness(List<int>[] RowRules, List<int>[] ColumnRules) //Will todo
        {
            RowWiseFitness(RowRules);
            ColumnWiseFitness(ColumnRules);

            fitness = RowFitness + ColumnFitness;
        }

        public void RowWiseFitness(List<int>[] RowRules)
        {
            int fitness = 0;

            Stack<bool> currentStack = new Stack<bool>();

            for (int i = 0; i < row; i++)
            {
                State currentState = State.INITIAL_STATE;

                for (int j = RowRules[i].Count - 1; j >= 0; j--)
                {
                    currentStack.Push(false);

                    for (int k = 0; k < RowRules[i][j]; k++)
                        currentStack.Push(true);
                }

                for (int j = 0; j < col; j++)
                {
                    currentState = StateMachine.Delta(currentState, dna[i, j], ref currentStack);

                    if (currentState == State.RULE_BROKEN)
                        fitness++;
                }

                while (currentStack.Count != 0)
                {
                    if (currentStack.Peek())
                        fitness++;

                    currentStack.Pop();
                }
            }

            RowFitness = fitness;
        }

        public void ColumnWiseFitness(List<int>[] ColumnRules)
        {
            int fitness = 0;

            Stack<bool> currentStack = new Stack<bool>();

            for (int j = 0; j < col; j++)
            {
                State currentState = State.INITIAL_STATE;

                for (int i = ColumnRules[j].Count - 1; i >= 0; i--)
                {
                    currentStack.Push(false);

                    for (int k = 0; k < ColumnRules[j][i]; k++)
                        currentStack.Push(true);
                }

                for (int i = 0; i < row; i++)
                {
                    currentState = StateMachine.Delta(currentState, dna[i, j], ref currentStack);

                    if (currentState == State.RULE_BROKEN)
                        fitness++;
                }

                while (currentStack.Count != 0)
                {
                    if (currentStack.Peek())
                        fitness++;

                    currentStack.Pop();
                }
            }

            ColumnFitness = fitness;
        }

        //Clone a members dna
        public void Clone(bool[,] copyDNA)
        {
            dna = copyDNA;
        }
    }

    public class StateMachine
    {
        public static State Delta(bool value, ref int current_runs, int max_runs)
        {
            if (value)
            {
                if (current_runs < max_runs)
                {
                    current_runs++;

                    return State.COUNTING_RUNS;
                }

                if (current_runs == max_runs)
                {
                    return State.RULE_BROKEN;
                }
            }

            if (!value)
            {
                if (current_runs == max_runs)
                {
                    current_runs = 0;

                    return State.INITIAL_STATE;
                }
            }

            current_runs = 0;

            return State.INITIAL_STATE;
        }

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

    public enum State
    {
        INITIAL_STATE,
        COUNTING_RUNS,
        FINDING_NEW_RUN,
        END_OF_CURRENT_RUNS_REACHED,
        BREAK_IN_CURRENT_RUNS_ENCOUNTERED,
        RULE_BROKEN
    }
}