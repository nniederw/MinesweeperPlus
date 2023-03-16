using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(Grid))]
public class Board : MonoBehaviour
{
    private int SizeX = 10;
    private int SizeY = 10;
    //private GridLayoutGroup Grid;
    [SerializeField] private GameObject Cell;
    void Start()
    {
        //Grid = GetComponent<GridLayoutGroup>();
        for (int i = 0; i < 10; i++)
        {
            Instantiate(Cell, transform);
            //Grid.
        }
    }
    void Update()
    {

    }
}