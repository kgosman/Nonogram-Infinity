﻿using System;
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

        public int RowFitness { get; private set; }
        public int ColumnFitness { get; private set; }
        public Member (int Row, int Column, int BlackSquares, List<int>[] RowRules, List<int>[] ColumnRules, bool[,] DNA = null)
        {
            Fitness = 0;
            this.Row = Row;
            this.Column = Column;

            if (DNA == null)
            {
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
            }
            else
                this.DNA = DNA;

            FindFitness(RowRules, ColumnRules);
        }
        /*
         * Calculates the distance of the DNA
         */
        public void FindFitness(List<int>[] RowRules, List<int>[] ColumnRules) //Will todo
        {
            RowWiseFitness(RowRules);
            ColumnWiseFitness(ColumnRules);

            Fitness = RowFitness + ColumnFitness;
        }

        public void RowWiseFitness(List<int>[] RowRules)
        {
            int fitness = 0;

            Stack<bool> currentStack = new Stack<bool>();

            for (int i = 0; i < Row; i++)
            {
                State currentState = State.INITIAL_STATE;

                for (int j = RowRules[i].Count - 1; j >= 0; j--)
                {
                    currentStack.Push(false);

                    for (int k = 0; k < RowRules[i][j]; k++)
                        currentStack.Push(true);
                }

                for (int j = 0; j < Column; j++)
                {
                    currentState = StateMachine.Delta(currentState, DNA[i, j], ref currentStack);

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

            for (int j = 0; j < Column; j++)
            {
                State currentState = State.INITIAL_STATE;

                for (int i = ColumnRules[j].Count - 1; i >= 0; i--)
                {
                    currentStack.Push(false);

                    for (int k = 0; k < ColumnRules[j][i]; k++)
                        currentStack.Push(true);
                }

                for (int i = 0; i < Row; i++)
                {
                    currentState = StateMachine.Delta(currentState, DNA[i, j], ref currentStack);

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
            DNA = copyDNA;
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