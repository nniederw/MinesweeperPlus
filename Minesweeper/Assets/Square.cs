using System;
using UnityEngine;

public class Square : MonoBehaviour
{
    private int X;
    private int Y;
    public Action<int, int> OnClick;
    private OpenSquare OpenSquareNumber;
    public enum OpenSquare
    {
        Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Mine
    }
    public void SetPos(int x, int y)
    {
        X = x;
        Y = y;
    }
    public void SetOpenNumber(OpenSquare open)
    {
        OpenSquareNumber = open;
        //TODO update graphics
    }
    private void OnMouseUp()
    {
        OnClick?.Invoke(X, Y);
    }
}
