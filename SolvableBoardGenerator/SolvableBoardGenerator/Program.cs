using Minesweeper;

int[,] field;
//bool[,] field;
/*
field = {
            { 0, 0, 0 },
            { 1, 1, 0 },
            { 9, 1, 0 } };
Ext.Assert(Test(field, 0, 0));
Ext.Assert(Test(field, 0, 1));
Ext.Assert(Test(field, 0, 2));
Ext.Assert(Test(field, 1, 2));
Ext.Assert(Test(field, 2, 2));
field = (new int[,] {
            { 0, 1, 1, 1 },
            { 0, 1, 9, 1 },
            { 0, 1, 1, 1 },
            { 0, 0, 0, 0 } });
Ext.Assert(Test(field, 0, 0));
Ext.Assert(Test(field, 1, 0));
Ext.Assert(Test(field, 2, 0));
Ext.Assert(Test(field, 3, 0));
Ext.Assert(Test(field, 3, 1));
Ext.Assert(Test(field, 3, 2));
Ext.Assert(Test(field, 3, 3));

field = (new int[,] {
            { 0, 1, 9, 2 },
            { 0, 1, 2, 9 },
            { 0, 0, 1, 1 },
            { 0, 0, 0, 0 } });
Ext.Assert(Test(field, 0, 0));
Ext.Assert(Test(field, 1, 0));
Ext.Assert(Test(field, 2, 0));
Ext.Assert(Test(field, 3, 0));
Ext.Assert(Test(field, 3, 1));
Ext.Assert(Test(field, 3, 2));
Ext.Assert(Test(field, 3, 3));
Ext.Assert(Test(field, 2, 1));
*/
/*
field = (new int[,] {
            { 0, 1, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 }});
*/

field = (new int[,] {
            { 0, 0, 1, 1 },
            { 0, 0, 0, 1 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 }});

var bfield = BoardConverter.BinaryIntsToBools(field);
Board board = new Board(bfield);
BoardConverter.PrintBoolArray(bfield);
Console.WriteLine();
var solvables = BoardChecker.CheckBoard(board);
BoardConverter.PrintBoolArray(solvables);