using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    public struct Move
    {
        public Columns Column { get; private set; }
        public Rows Row { get; private set; }
        public Values Value { get; private set; }

        public int Score { get; private set; }

        public Move(Columns column, Rows row, Values value, int score = 0)
            : this()
        {
            Row = row;
            Column = column;
            Value = value;
            Score = score;
        }

        public override string ToString()
        {
            return string.Format("(Row: {0} Column: {1} Value: {2})",
                this.Row,
                this.Column,
                this.Value
                );
        }
    }

    public enum Columns
    {
        A,B,C,D,
        E,F,G,H,
        I
    }

    public enum Rows
    {
        One, Two, Three, Four,
        Five, Six, Seven, Eight,
        Nine
    }

    [Flags]
    public enum Values
    {
        None = 0,
        One = 1,      // 1 << 1
        Two = 2,      // 1 << 2
        Three = 4,    // 1 << 3 
        Four = 8,     // ...
        Five = 16,
        Six = 32,
        Seven = 64,
        Eight = 128,
        Nine = 256,
        All = 1 | 2 | 4 | 8 | 16 | 32 | 64 | 128 | 256
    }



}
