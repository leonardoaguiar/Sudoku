using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Sudoku.Tests
{
    [TestFixture]
    public class BoardTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MakeMove_NumberRepetitionInRegion_ThrowsInvalidOperationException()
        {
            // arrange
            Board b = new Board();
            
            // act
            b.MakeMove(new Move(Columns.A, Rows.One, Values.Nine));
            b.MakeMove(new Move(Columns.B, Rows.Two, Values.Nine));

            // assert
        }

        [Test]
        public void UnmakeMove_MakeAndUnmake9InA1_EmptyA1()
        {
            Board b = new Board();
            
            b.MakeMove(new Move(Columns.A, Rows.One, Values.Nine));
            b.UnmakeMove(new Move(Columns.A, Rows.One, Values.Nine));

            Assert.AreEqual(Values.None, b.GetCellValue(Columns.A, Rows.One));
            Assert.AreEqual(Values.None, b.GetColumnValues(Columns.A));
            Assert.AreEqual(Values.None, b.GetRowValues(Rows.One));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MakeMove_NumberRepetitionInLine_ThrowsInvalidOperationException()
        {
            // arrange
            Board b = new Board();

            // act
            b.MakeMove(new Move(Columns.A, Rows.One, Values.Nine));
            b.MakeMove(new Move(Columns.G, Rows.One, Values.Nine));

            // assert
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MakeMove_NumberRepetitionInColumn_ThrowsInvalidOperationException()
        {
            // arrange
            Board b = new Board();

            // act
            b.MakeMove(new Move(Columns.A, Rows.One, Values.Nine));
            b.MakeMove(new Move(Columns.A, Rows.Seven, Values.Nine));

            // assert
        }

        [Test]
        public void GetCellValue_SettingCellTo9_Returns9()
        {
            // arrange
            var b = new Board();
            b.MakeMove(new Move(Columns.A, Rows.Seven, Values.Nine));
            // act
            var result = b.GetCellValue(Columns.A, Rows.Seven);
            // assert
            Assert.AreEqual(Values.Nine, result);
        }

        [Test]
        public void GetRowValues_Setting7And9_Returns7And9()
        {
            // arrange
            var v = new Board();
            v.MakeMove(new Move(Columns.A, Rows.One, Values.One));
            v.MakeMove(new Move(Columns.G, Rows.One, Values.Nine));

            // act
            var result = v.GetRowValues(Rows.One);

            // assert
            Assert.AreEqual(Values.One | Values.Nine, result);
        }

        [Test]
        public void GetColumnValues_Setting7And9_Returns7And9()
        {
            // arrange
            var v = new Board();
            v.MakeMove(new Move(Columns.A, Rows.One, Values.One));
            v.MakeMove(new Move(Columns.A, Rows.Seven, Values.Nine));

            // act
            var result = v.GetColumnValues(Columns.A);

            // assert
            Assert.AreEqual(Values.One | Values.Nine, result);
        }

        [Test]
        public void GetRegionValues_Setting7And9_Returns7And9()
        {
            // arrange
            var v = new Board();
            v.MakeMove(new Move(Columns.A, Rows.One, Values.One));
            v.MakeMove(new Move(Columns.C, Rows.Three, Values.Nine));
            
            // act
            var result = v.GetRegionValues(Columns.B, Rows.Two);

            // assert
            Assert.AreEqual(Values.One | Values.Nine, result);
        }

        [Test]
        public void GetValueOptions_A1InEmptyBoard_ReturnsAllValues()
        {
            // arrange
            var v = new Board();
            var expected = Values.One | Values.Two | Values.Three
                | Values.Four | Values.Five | Values.Six
                | Values.Seven | Values.Eight | Values.Nine;
            
            // act
            var result = v.GetValueOptions(Columns.A, Rows.One)
                .Aggregate((v1, v2) => v1 | v2)
                ;

            // assert
            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void Solve_Board1()
        {
            // arrange
            var b = new Board();
            b
                .MakeMoves(
                    new Move(Columns.A, Rows.One, Values.Six),
                    new Move(Columns.B, Rows.One, Values.Four),
                    new Move(Columns.C, Rows.One, Values.One),
                    new Move(Columns.F, Rows.One, Values.Seven)
                )
                .MakeMoves(
                    new Move(Columns.C, Rows.Two, Values.Two),
                    new Move(Columns.D, Rows.Two, Values.Six),
                    new Move(Columns.E, Rows.Two, Values.Eight),
                    new Move(Columns.F, Rows.Two, Values.Three),
                    new Move(Columns.G, Rows.Two, Values.Five)
                )
                .MakeMoves(
                    new Move(Columns.A, Rows.Three, Values.Three),
                    new Move(Columns.H, Rows.Three, Values.Six),
                    new Move(Columns.I, Rows.Three, Values.Two)
                )
                .MakeMoves(
                    new Move(Columns.B, Rows.Four, Values.Five),
                    new Move(Columns.D, Rows.Four, Values.Seven),
                    new Move(Columns.H, Rows.Four, Values.Nine),
                    new Move(Columns.I, Rows.Four, Values.Three)
                )
                .MakeMoves(
                    new Move(Columns.A, Rows.Five, Values.One),
                    new Move(Columns.C, Rows.Five, Values.Seven),
                    new Move(Columns.G, Rows.Five, Values.Eight),
                    new Move(Columns.I, Rows.Five, Values.Six)
                )
                .MakeMoves(
                    new Move(Columns.A, Rows.Six, Values.Two),
                    new Move(Columns.B, Rows.Six, Values.Three),
                    new Move(Columns.F, Rows.Six, Values.Five),
                    new Move(Columns.H, Rows.Six, Values.Seven)
                )
                .MakeMoves(
                    new Move(Columns.A, Rows.Seven, Values.Eight),
                    new Move(Columns.B, Rows.Seven, Values.One),
                    new Move(Columns.I, Rows.Seven, Values.Five)
                )
                .MakeMoves(
                    new Move(Columns.C, Rows.Eight, Values.Four),
                    new Move(Columns.D, Rows.Eight, Values.Nine),
                    new Move(Columns.E, Rows.Eight, Values.Six),
                    new Move(Columns.F, Rows.Eight, Values.One),
                    new Move(Columns.G, Rows.Eight, Values.Three)
                )
                .MakeMoves(
                    new Move(Columns.D, Rows.Nine, Values.Four),
                    new Move(Columns.G, Rows.Nine, Values.One),
                    new Move(Columns.H, Rows.Nine, Values.Two),
                    new Move(Columns.I, Rows.Nine, Values.Nine)
                );
            
            // act
            var result = b.Solve();
            
            // assert
            Assert.IsTrue(b.IsSolved);
            Assert.IsTrue(result);
        }

        [Test]
        public void Solve_Board2()
        {
            int[] board = new int[] 
            {
                0,0,1,0,0,0,0,9,0,
                3,0,0,0,0,2,0,1,0,
                0,0,9,1,0,0,4,0,6,
                0,0,0,4,0,8,0,0,0,
                9,3,0,0,0,0,0,2,7,
                0,0,0,2,0,7,0,0,0,
                5,0,2,0,0,1,9,0,0,
                0,9,0,7,0,0,0,0,5,
                0,6,0,0,0,0,3,0,0
            };

            var b = new Board(board);

            // act
            var result = b.Solve();

            // assert
            Assert.IsTrue(b.IsSolved);
            Assert.IsTrue(result);
        }
    }
}
