using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Square : MonoBehaviour
{
    private int X;
    private int Y;
    public Action<int, int> OnOpen;
    public Action<int, int> OnClick;
    public Action OnFlagg;
    public Action OnUnFlagg;
    private int Number; // 0-8 numbers, 9 meaning mine
    [SerializeField] private TMP_Text NumberText = null;
    private SpriteRenderer SpriteRenderer;
    private bool Revealed = false;
    public bool Flagged = false;
    [SerializeField] private Sprite UnOpenedSquare = null;
    [SerializeField] private Sprite OpenedSquare = null;
    [SerializeField] private Sprite FlaggedSquare = null;
    [SerializeField] private Sprite OpenedMine = null;
    [SerializeField] private Sprite Mine = null;
    private RectTransform Rect
    {
        get
        {
            if (_Rect == null)
            {
                _Rect = GetComponent<RectTransform>();
            }
            return _Rect;
        }
    }
    private RectTransform _Rect;
    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetNumber(int number)
    {
        Number = number;
        if (Number == 9)
        {
            NumberText.text = "";
        }
        else if (Number == 0)
        {
            NumberText.text = "";
        }
        else
        {
            NumberText.text = number.ToString();
        }
    }
    public void SetPos(int x, int y)
    {
        X = x;
        Y = y;
        Rect.anchorMin = new Vector2(0.5f, 0.5f);
        Rect.anchorMax = new Vector2(0.5f, 0.5f);
        Rect.sizeDelta = Vector2.one;
    }
    public void Reveal()
    {
        if (!Revealed)
        {
            Revealed = true;
            OnOpen?.Invoke(X, Y);
        }
        UpdateVisuals();
    }
    public void RevealGameover()
    {
        if (Number == 9)
        {
            Reveal();
        }
    }
    private void OnMouseUp()
    {
        if (!Revealed)
        {
            OnOpen?.Invoke(X, Y);
            Revealed = true;
            UpdateVisuals();
            if (Number == 9)
            {
                SpriteRenderer.sprite = OpenedMine;
            }
        }
        else
        {
            OnClick?.Invoke(X, Y);
        }
    }
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Flagged = !Flagged;
            if (Flagged) { OnFlagg?.Invoke(); }
            else { OnUnFlagg?.Invoke(); }
            UpdateVisuals();
        }
    }
    private void UpdateVisuals()
    {
        if (Revealed)
        {
            NumberText.enabled = true;
            SpriteRenderer.sprite = Number == 9 ? Mine : OpenedSquare;
            return;
        }
        NumberText.enabled = false;
        SpriteRenderer.sprite = Flagged ? FlaggedSquare : UnOpenedSquare;
    }
}