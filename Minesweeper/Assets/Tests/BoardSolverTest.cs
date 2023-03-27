using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Minesweeper;
public class BoardSolverTest
{
    [Test]
    public void BoardSolverTestSimplePasses()
    {
        int[,] field = { { 0, 0, 0 }, { 1, 1, 0 }, { 9, 1, 0 } };
        BoardSolver solver = new BoardSolver(new UnsolvedMineField(field));
        Assert.IsTrue(solver.IsSolvable(0, 0));
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator BoardSolverTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
