using System;
using System.Collections.Generic;

namespace Nonogram_Infinity.GA
{
    class Picross
    {
        bool[,] GameBoard;
        int MaxRows;
        int MaxColumns;
        List<int>[] ColumnRules;
        List<int>[] RowRules;

        public Picross()
        {

        }

        public double EvaluationFunction()
        {
            double fitness = 0;
            for (int i = 0; i < MaxRows; i++)
            {
                int k = 0;
                int current_runs = 0;
                for (int j = 0; j < MaxColumns; j++)
                {
                    if (k == RowRules[i].Count)
                        break;

                    int rule = RowRules[i][k];

                    State currentState = StateMachine.Delta(GameBoard[i, j], ref current_runs, rule);

                    if (currentState == State.RULE_BROKEN)
                        fitness += 1;

                    if (currentState == State.INITIAL_STATE)
                        k++;
                }
            }

            for (int i = 0; i < MaxColumns; i++)
            {
                int k = 0;
                int current_runs = 0;
                for (int j = 0; j < MaxRows; j++)
                {
                    if (k == ColumnRules[i].Count)
                        break;
                    int rule = ColumnRules[i][k];

                    State currentState = StateMachine.Delta(GameBoard[i, j], ref current_runs, rule);

                    if (currentState == State.RULE_BROKEN)
                        fitness += 1;

                    if (currentState == State.INITIAL_STATE)
                        k++;
                }
            }

            return fitness;
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

            if (current_runs == max_runs)
                return State.END_OF_CURRENT_RUNS_REACHED;

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
    }

    public enum State
    {
        INITIAL_STATE,
        COUNTING_RUNS,
        END_OF_CURRENT_RUNS_REACHED,
        RULE_BROKEN
    }
}
