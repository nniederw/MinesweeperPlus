using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Board : MonoBehaviour
{
    private int SizeX = 10;
    private int SizeY = 10;
    private RectTransform rectTransform;
    [SerializeField] private GameObject Cell;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(SizeX, SizeY);
        for (int y = 0; y < SizeY; y++)
        {
            for (int x = 0; x < SizeX; x++)
            {
                var squareObj = Instantiate(Cell, transform);
                Square square = squareObj.GetComponent<Square>();
                square.SetPos(x, y);
                square.OnClick = (x, y) => Debug.Log($"pressed on {x},{y}");
                squareObj.transform.localPosition = new Vector2(x, y) - new Vector2(SizeX/2,SizeY/2);
            }
        }
    }
}