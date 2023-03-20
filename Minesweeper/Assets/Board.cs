using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(RectTransform))]
public class Board : MonoBehaviour
{
    [SerializeField] private int SizeX = 10;
    [SerializeField] private int SizeY = 10;
    [SerializeField] private int Mines = 30;
    [SerializeField] private int MinesLeft = 30;
    private RectTransform rectTransform;
    [SerializeField] private GameObject Cell;
    private int[,] MineField; //0-8 numbers, 9 meaning mine
    private Square[,] SquareField;
    void Start()
    {
        MineField = new int[SizeX, SizeY];
        SquareField = new Square[SizeX, SizeY];
        GenerateField();
        CalcNumbers();
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(SizeX, SizeY);
        for (int y = 0; y < SizeY; y++)
        {
            for (int x = 0; x < SizeX; x++)
            {
                Transform squareTrans = Instantiate(Cell, transform).transform;
                Square square = squareTrans.GetComponent<Square>();
                SquareField[x, y] = square;
                square.SetPos(x, y);
                square.OnOpen = OnSquareOpen;
                square.OnClick = TryChord;
                square.OnFlagg = () => MinesLeft--;
                square.OnUnFlagg = () => MinesLeft++;
                square.SetNumber(MineField[x, y]);
                squareTrans.localPosition = new Vector2(x, y) - new Vector2(SizeX / 2, SizeY / 2) + new Vector2(0.5f, 0.5f);
            }
        }
    }
    private void OnSquareOpen(int x, int y)
    {
        if (MineField[x, y] == 0)
        {
            AdjSquares(x, y).ForEach(s => s.Reveal());
        }
    }
    private List<Square> AdjSquares(int x, int y)
    {
        List<Square> adjsqu = new List<Square>();
        try { adjsqu.Add(SquareField[x - 1, y + 1]); } catch { }
        try { adjsqu.Add(SquareField[x + 0, y + 1]); } catch { }
        try { adjsqu.Add(SquareField[x + 1, y + 1]); } catch { }
        try { adjsqu.Add(SquareField[x + 1, y + 0]); } catch { }
        try { adjsqu.Add(SquareField[x + 1, y - 1]); } catch { }
        try { adjsqu.Add(SquareField[x + 0, y - 1]); } catch { }
        try { adjsqu.Add(SquareField[x - 1, y - 1]); } catch { }
        try { adjsqu.Add(SquareField[x - 1, y + 0]); } catch { }
        return adjsqu;
    }
    private void TryChord(int x, int y)
    {
        int mines = MineField[x, y];
        var adjSqu = AdjSquares(x, y);
        int flagged = adjSqu.Where(s => s.Flagged).Count();
        if (flagged == mines)
        {
            adjSqu.Where(s => !s.Flagged).Foreach(s => s.Reveal());
        }
    }
    private void GenerateField(int seed = 1)
    {
        System.Random rand = new System.Random(seed);
        int mines = Mines;
        MinesLeft = mines;
        int leftSquares = SizeX * SizeY;
        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                if (Mines != leftSquares)
                {
                    MineField[x, y] = rand.NextDouble() <= ((double)mines) / leftSquares ? 9 : 0;
                    if (MineField[x, y] == 9) { mines--; }
                    leftSquares--;
                }
                else
                {
                    MineField[x, y] = 9;
                    leftSquares--;
                }
            }
        }
    }
    private void CalcNumbers()
    {
        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                if (MineField[x, y] != 9)
                {
                    MineField[x, y] = AdjMines(x, y);
                }
            }
        }
    }
    private int AdjMines(int x, int y)
    {
        int mines = 0;
        try { mines += MineField[x - 1, y + 1] == 9 ? 1 : 0; } catch { }
        try { mines += MineField[x + 0, y + 1] == 9 ? 1 : 0; } catch { }
        try { mines += MineField[x + 1, y + 1] == 9 ? 1 : 0; } catch { }
        try { mines += MineField[x + 1, y + 0] == 9 ? 1 : 0; } catch { }
        try { mines += MineField[x + 1, y - 1] == 9 ? 1 : 0; } catch { }
        try { mines += MineField[x + 0, y - 1] == 9 ? 1 : 0; } catch { }
        try { mines += MineField[x - 1, y - 1] == 9 ? 1 : 0; } catch { }
        try { mines += MineField[x - 1, y + 0] == 9 ? 1 : 0; } catch { }
        return mines;
    }
}