using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitContorller : MonoBehaviour
{
    private static FruitContorller choosedBall;
    private SpriteRenderer ballRenderer;
    public Vector2Int position;

    void Start()
    {
        ballRenderer = GetComponent<SpriteRenderer>();
    }
    public void Choose()
    {
        ballRenderer.color = Color.grey;
    }
    public void Unchoose()
    {
        ballRenderer.color = Color.white;
    }
    private void OnMouseDown()
    {
        if (choosedBall != null)
        {
            if (choosedBall == this)
                return;
            choosedBall.Unchoose();
            if (Vector2Int.Distance(choosedBall.position, position) == 1)
            {
                GameController.Instance.SwapCell(position,choosedBall.position);
                choosedBall = null;
            }
            else
            {
                choosedBall = this;
                Choose();
            }
        }
        else
        {
            choosedBall = this;
            Choose();
        }
    }
}