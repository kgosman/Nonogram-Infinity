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
            int fitness = 0;
            for (int i = 0; i < row; i++)
            {
                int k = 0;
                int current_runs = 0;
                State currentState = State.INITIAL_STATE;
                int rule = 0;
                for (int j = 0; j < col; j++)
                {
                    if (k == RowRules[i].Count)
                        break;

                    rule = RowRules[i][k];

                    currentState = StateMachine.Delta(dna[i, j], ref current_runs, rule);

                    if (currentState == State.RULE_BROKEN)
                        fitness += 1;

                    if (currentState == State.INITIAL_STATE)
                        k++;
                }

                if (currentState == State.RULE_BROKEN)
                    fitness += 1;

                if (currentState == State.COUNTING_RUNS && current_runs != rule)
                    fitness += 1;

            }

            for (int j = 0; j < col; j++)
            {
                int k = 0;
                int current_runs = 0;
                State currentState = State.INITIAL_STATE;
                int rule = 0;
                for (int i = 0; i < row; i++)
                {
                    if (k == ColumnRules[j].Count)
                        break;

                    rule = ColumnRules[j][k];

                    currentState = StateMachine.Delta(dna[i, j], ref current_runs, rule);

                    if (currentState == State.RULE_BROKEN)
                        fitness += 1;

                    if (currentState == State.INITIAL_STATE)
                        k++;
                }

                if (currentState == State.RULE_BROKEN)
                    fitness += 1;

                if (currentState == State.COUNTING_RUNS && current_runs != rule)
                    fitness += 1;
            }

            this.fitness = fitness;
        }

        public void RowWiseFitness(List<int>[] RowRules)
        {
            int fitness = 0;

            Stack<bool> currentStack = new Stack<bool>();

            for (int i = 0; i < RowRules.Count(); i++)
            {
                foreach (int rule in RowRules[i])
                {
                    for (int j = 0; j < rule; j++)
                        currentStack.Push(true);
                }
            }

            for (int i = 0; i < row; i++)
            {
                int k = 0;
                int current_runs = 0;
                State currentState = State.INITIAL_STATE;
                int rule = 0;

                for (int j = 0; j < col; j++)
                {
                    if (k == RowRules[i].Count)
                        break;

                    rule = RowRules[i][k];

                    currentState = StateMachine.Delta(currentState, dna[i, j], ref currentStack);

                    if (currentState == State.RULE_BROKEN)
                        fitness += 1;

                    if (currentState == State.INITIAL_STATE)
                        k++;
                }

                if (currentState == State.RULE_BROKEN)
                    fitness += 1;

                if (currentState == State.COUNTING_RUNS && current_runs != rule)
                    fitness += 1;

            }

            this.RowFitness = fitness;
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

            if ((currentState == State.COUNTING_RUNS || currentState == State.INITIAL_STATE)
                    && currentStack.Peek() == value && currentStack.Peek())
            {
                currentState = State.COUNTING_RUNS;

                currentStack.Pop();
            }

            if (currentState == State.COUNTING_RUNS && currentStack.Peek() != value)
                currentState = State.RULE_BROKEN;

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