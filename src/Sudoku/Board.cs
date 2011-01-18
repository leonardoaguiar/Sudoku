using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    public class Board
    {
        Values[,] _BoardValues = new Values[9, 9];
        Values[] _RowValues = new Values[9];
        Values[] _ColumnValues = new Values[9];
        Values[,] _RegionValues = new Values[3, 3];
        int _EmptySquaresCount = 81;

        public Board()
        {

        }

        public Board(int[] board) : this()
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    Columns c = (Columns)j;
                    Rows r = (Rows)i;
                    if (board[(i * 9) + j] != 0)
                    {
                        Values v = (Values)(1 << (board[(i * 9) + j] - 1));
                        if (v != Values.None)
                            this.MakeMove(new Move(c, r, v));
                    }
                }
        }

        public Board MakeMoves(params Move[] moves)
        {
            foreach (var move in moves)
                this.MakeMove(move);

            return this;
        }

        public Board MakeMove(Move move)
        {
            int row = (int)move.Row;
            int column = (int)move.Column;

            _BoardValues[column, row] = move.Value;
            //int bit = (int) move.Value;

            if ((_RowValues[row] & move.Value) == move.Value)
                throw new ArgumentException(
                    string.Format("Invalid move {2}.\nNumber {0}. \nAlready present in row {1}\n{3}",
                    move.Value,
                    move.Row,
                    move.ToString(),
                    this.ToString()
                    ));

            if ((_ColumnValues[column] & move.Value) == move.Value)
                throw new ArgumentException(
                    string.Format("Invalid move {2}.\nNumber {0} already present in column {1}\n{3}",
                    move.Value,
                    move.Column,
                    move.ToString(),
                    this.ToString()
                    ));

            if ((_RegionValues[column / 3, row / 3] & move.Value) == move.Value)
                throw new ArgumentException(
                    string.Format("Invalid move {1}.\nNumber {0} already present in this region\n{2}",
                    move.Value,
                    move.ToString(),
                    this.ToString()
                    ));

            _RowValues[row] |= move.Value;
            _ColumnValues[column] |= move.Value;
            _RegionValues[column / 3, row / 3] |= move.Value;
            _EmptySquaresCount--;

            return this;
        }

        public Board UnmakeMove(Move move)
        {
            int row = (int)move.Row;
            int column = (int)move.Column;
            if (_BoardValues[column, row] == Values.None)
                throw new ArgumentException();

            _BoardValues[column, row] = Values.None;
            int bit = ~((int)move.Value);
            _RowValues[row] = (Values) ((int)_RowValues[row] & bit);
            _ColumnValues[column] = (Values)((int)_ColumnValues[column] & bit);
            _RegionValues[column / 3, row / 3] = (Values) ((int) _RegionValues[column / 3, row / 3] & bit);
            _EmptySquaresCount++;

            return this;
        }

        public Values GetCellValue(Columns column, Rows row)
        {
            return this._BoardValues[(int)column, (int)row];
        }

        public Values GetRowValues(Rows row)
        {
            return _RowValues[(int)row];
        }

        public Values GetRegionValues(Columns oneCellColumn, Rows oneCellRow)
        {
            int column = (int)oneCellColumn;
            int row = (int)oneCellRow;
            return _RegionValues[column / 3, row / 3];
        }


        public Values GetColumnValues(Columns column)
        {
            return _ColumnValues[(int)column];
        }

        public Values GetDeniedValues(Columns column, Rows row)
        {
            return GetRowValues(row) |
                GetColumnValues(column) |
                GetRegionValues(column, row);
        }

        public IEnumerable<Values> GetValueOptions(Columns column, Rows row)
        {
            var usedValues = GetDeniedValues(column, row);

            for (int i = 0; i < 9; i++)
            {
                Values candidate = (Values)(1 << i);
                if ((usedValues & candidate) == Values.None)
                    yield return candidate;
            }
        }


        public bool IsSolved
        { get { return _EmptySquaresCount == 0; } }

        public bool HasDeadSquares
        {
            get
            {
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 9; j++)
                    {
                        Rows row = (Rows)i;
                        Columns column = (Columns)j;

                        if (GetCellValue(column, row) != Values.None) continue;

                        if (GetDeniedValues(column, row) == Values.All)
                            return true;
                    }

                return false;
            }
        }

        public bool Solve()
        {
            if (this.IsSolved) return true;
            if (this.HasDeadSquares) 
                return false;

            var orderedMoves = this.GenerateMoves().OrderBy(move => move.Score);

            foreach (Move move in orderedMoves)
            {
                this.MakeMove(move);
                if (this.Solve()) return true;
                this.UnmakeMove(move);
            }

            return false;
        }

        
        private IEnumerable<Move> GenerateMoves()
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    Rows row = (Rows)i;
                    Columns column = (Columns)j;

                    if (GetCellValue(column, row) != Values.None) continue;

                    var options = GetValueOptions(column, row).ToArray();
                    
                    foreach (Values option in options)
                    {
                        var score = options.Count() * 100
                            + NumberOfSquaresInColumnCouldUseValue(column, option)
                            + NumberOfSquaresInRowCouldUseValue(row, option)
                            + NumberOfSquaresInRegionCouldUseValue(column, row, option);

                        yield return new Move(
                            column,
                            row,
                            option,
                            score
                            );
                    }
                }
        }

        private int NumberOfSquaresInRowCouldUseValue(Rows row, Values value)
        {
            var result = 0;
            for (int i = 0; i < 9; i++)
            {
                Columns c = (Columns)i;
                if (CouldPutValueInSquare(c, row, value))
                    result ++;
            }
            return result;
        }

        private int NumberOfSquaresInColumnCouldUseValue(Columns column, Values value)
        {
            var result = 0;
            for (int i = 0; i < 9; i++)
            {
                Rows r = (Rows)i;
                if (CouldPutValueInSquare(column, r, value))
                    result++;
            }
            return result;
        }

        private int NumberOfSquaresInRegionCouldUseValue(Columns column, Rows row, Values value)
        {
            int r = (int)row / 3;
            int c = (int)column / 3;
            var result = 0;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j ++ )
                {
                    Rows ro = (Rows)(r*3+i);
                    Columns co = (Columns)(c * 3 + i);
                    if (CouldPutValueInSquare(co, ro, value)) result++;
                }
            
            return result;
        }

        private bool CouldPutValueInSquare(Columns column, Rows row, Values value)
        {
            int r = (int)row;
            int c = (int)column;
            return (
                ((_RowValues[r] & value) == 0) &&
                ((_ColumnValues[c] & value) > 0) &&
                ((_RegionValues[c / 3, r / 3] & value) == 0)
                );
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 8; i >= 0; i--)
            {
                sb.Append("+-+-+-+-+-+-+-+-+-+");
                sb.Append("\n");
                sb.Append("|");
                for (int j = 0; j < 9; j++)
                {
                    var v = GetCellValue((Columns)j, (Rows)i);
                    string s = " ";
                    if (v == Values.One) s = "1";
                    if (v == Values.Two) s = "2";
                    if (v == Values.Three) s = "3";
                    if (v == Values.Four) s = "4";
                    if (v == Values.Five) s = "5";
                    if (v == Values.Six) s = "6";
                    if (v == Values.Seven) s = "7";
                    if (v == Values.Eight) s = "8";
                    if (v == Values.Nine) s = "9";
                    if (v == Values.None) s = "0";

                    sb.Append(s);
                    sb.Append("|");
                  
                }
                sb.Append("\n");
            }
            sb.Append("+-+-+-+-+-+-+-+-+-+");
            return sb.ToString();
        }
    }
}
