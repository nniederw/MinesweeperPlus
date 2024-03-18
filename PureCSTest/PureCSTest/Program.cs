using Minesweeper;

int[,] field;
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

field = (new int[,] {
            { 0, 1, 1 },
            { 0, 1, 9 },
            { 0, 1, 1 }});
Ext.Assert(Test(field, 0, 0));

Console.WriteLine("All tests passed");
static bool Test(int[,] field, int x, int y)
=> new BoardSolver(new UnsolvedMineField(field)).IsSolvable(x, y);
