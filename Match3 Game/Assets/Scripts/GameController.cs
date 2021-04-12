using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private int xSize = 7, ySize = 7;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private UnityEvent scoreUpdate;
    public List<Sprite> fruitSprites = new List<Sprite>();
    private GameObject[,] grid;
    public Vector2Int position;
    public int matchCount;

    public static GameController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        CreateGrid();
    }
    
    private void CreateGrid()
    {
        grid = new GameObject[xSize, ySize];
        Vector2 centr = transform.position - new Vector3(xSize / 2, ySize / 2);
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject newCell = Instantiate(cellPrefab);
                List<Sprite> possibleSprites = new List<Sprite>(fruitSprites);

                Sprite left1 = GetSpriteAt(x - 1, y);
                Sprite left2 = GetSpriteAt(x - 2, y);
                if (left2 != null && left1 == left2)
                {
                    possibleSprites.Remove(left1);
                }

                Sprite down1 = GetSpriteAt(x, y - 1);
                Sprite down2 = GetSpriteAt(x, y - 2);
                if (down2 != null && down1 == down2)
                {
                    possibleSprites.Remove(down1);
                }

                SpriteRenderer cellSprite = newCell.GetComponent<SpriteRenderer>();
                int randomFruit = Random.Range(0, possibleSprites.Count);
                cellSprite.sprite = possibleSprites[randomFruit];

                FruitContorller fruit = newCell.AddComponent<FruitContorller>();
                fruit.position = new Vector2Int(x, y);

                newCell.transform.parent = transform;
                newCell.transform.position = new Vector2(x, y) + centr;
                grid[x, y] = newCell;
            }
        }
    }
    Sprite GetSpriteAt(int y, int x)
    {
        if (y < 0 || y >= ySize
            || x < 0 || x >= xSize)
            return null;
        GameObject cell = grid[y, x];
        SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();
        return renderer.sprite;
    }
    public void SwapCell(Vector2Int firstCellPos, Vector2Int secondCellPos)
    {
        GameObject cell1 = grid[firstCellPos.x, firstCellPos.y];
        SpriteRenderer sprite1 = cell1.GetComponent<SpriteRenderer>();

        GameObject cell2 = grid[secondCellPos.x, secondCellPos.y];
        SpriteRenderer sprite2 = cell2.GetComponent<SpriteRenderer>();

        Sprite i = sprite1.sprite;
        sprite1.sprite = sprite2.sprite;
        sprite2.sprite = i;

        bool check = CheckMatches();
        if (!check)
        {
            i = sprite1.sprite;
            sprite1.sprite = sprite2.sprite;
            sprite2.sprite = i;
        }
        else
        {
            NewFruitGenerator();
        }
    }

    SpriteRenderer GetSpriteRendererAt(int x, int y)
    {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize)
            return null;
        GameObject cell = grid[x, y];
        SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();
        return renderer;
    }
    bool CheckMatches()
    {
        HashSet<SpriteRenderer> matchedTiles = new HashSet<SpriteRenderer>();
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                SpriteRenderer current = GetSpriteRendererAt(y, x);
                List<SpriteRenderer> horizontalMatches = FindMatchY(y, x, current.sprite);
                if (horizontalMatches.Count >= 2)
                {

                    matchedTiles.UnionWith(horizontalMatches);
                    matchedTiles.Add(current);
                }

                List<SpriteRenderer> verticalMatches = FindMatchX(y, x, current.sprite);
                if (verticalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(verticalMatches);
                    matchedTiles.Add(current);
                }
            }
        }
        foreach (SpriteRenderer renderer in matchedTiles)
        {
            renderer.sprite = null;
        }
        matchCount = matchedTiles.Count;
        scoreUpdate.Invoke();
        return matchedTiles.Count > 0;
    }
    List<SpriteRenderer> FindMatchY(int y, int x, Sprite sprite)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        for (int i = y + 1; i < ySize; i++)
        {
            SpriteRenderer nextY = GetSpriteRendererAt(i, x);
            if (nextY.sprite != sprite)
            {
                break;
            }
            result.Add(nextY);
        }
        return result;
    }
    List<SpriteRenderer> FindMatchX(int y, int x, Sprite sprite)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        for (int i = x + 1; i < xSize; i++)
        {
            SpriteRenderer nextX = GetSpriteRendererAt(y, i);
            if (nextX.sprite != sprite)
            {
                break;
            }
            result.Add(nextX);
        }
        return result;
    }

    void NewFruitGenerator()
    {
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                while (GetSpriteRendererAt(y, x).sprite == null)
                {
                    for (int i = x; i < xSize - 1; i++)
                    {
                        SpriteRenderer current = GetSpriteRendererAt(y, i);
                        SpriteRenderer next = GetSpriteRendererAt(y, i + 1);
                        current.sprite = next.sprite;
                    }
                    SpriteRenderer last = GetSpriteRendererAt(y, ySize - 1);
                    last.sprite = fruitSprites[Random.Range(0, fruitSprites.Count)];
                }
            }
        }
        bool checkNull = CheckMatches();         
        if (!checkNull)
        {
            return;
        }
        else
        {
            NewFruitGenerator();
        }
    }
}