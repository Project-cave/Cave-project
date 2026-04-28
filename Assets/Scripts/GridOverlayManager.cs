using UnityEngine;
using System.Collections.Generic;

public class GridOverlayManager : MonoBehaviour
{
    public static GridOverlayManager Instance { get; private set; }

    [Header("스킬 그리드 크기")]
    [SerializeField] private float skillCellSize = 1;

    [Header("전체 그리드 비쥬얼")]
    [SerializeField] private Color gridLineColor = new Color(1, 1, 1, 0.2f);
    [SerializeField] private float gridLineWidth = 0.02f;

    [Header("스킬 범위 비쥬얼")]
    [SerializeField] private Color validColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] private Color invalidColor = new Color(1, 0, 0, 0.5f);

    private Camera mainCamera;
    private Sprite squareSprite;

    private List<GameObject> gridLines = new List<GameObject>();
    private List<GameObject> skillRangeTiles = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        mainCamera = Camera.main;
        squareSprite = CreateSquareSprite();
    }

    public void ShowFullGrid()
    {
        HideFullGrid();

        if (PathFinder.instance == null) return;

        Vector2 gridStart = PathFinder.instance.gridStartPoint;
        Vector2 gridEnd = PathFinder.instance.gridEndPoint;

        int numCols = Mathf.FloorToInt((gridEnd.x - gridStart.x) / skillCellSize);
        int numRows = Mathf.FloorToInt((gridEnd.y - gridStart.y) / skillCellSize);

        for (int x = 0; x <= numCols; x++)
        {
            float xPos = gridStart.x + x * skillCellSize;

            GameObject line = CreateGridLine(
                new Vector2(xPos, gridStart.y),
                new Vector2(xPos, gridEnd.y),
                gridLineWidth
            );
            gridLines.Add(line);
        }

        for (int y = 0; y <= numRows; y++)
        {
            float yPos = gridStart.y + y * skillCellSize;

            GameObject line = CreateGridLine(
                new Vector2(gridStart.x, yPos),
                new Vector2(gridEnd.x, yPos),
                gridLineWidth
            );
            gridLines.Add(line);
        }
    }

    private GameObject CreateGridLine(Vector2 start, Vector2 end, float width)
    {
        GameObject line = new GameObject("GridLine");
        line.transform.SetParent(transform);

        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = gridLineColor;
        lr.endColor = gridLineColor;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.sortingOrder = 90;

        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        return line;
    }

    public void HideFullGrid()
    {
        foreach (GameObject line in gridLines)
        {
            if (line != null)
                Destroy(line);
        }
        gridLines.Clear();
    }

    public void UpdateSkillRange(Vector3 mouseWorldPosition, SkillData skillData)
    {
        ClearSkillRange();

        if (skillData == null || PathFinder.instance == null) return;

        Vector2 centerCellWorldPos = GetCellCenterFromWorldPos(mouseWorldPosition);

        foreach (Vector2Int offset in skillData.affectedTiles)
        {
            Vector2 tileWorldPos = centerCellWorldPos + new Vector2(
                offset.x * skillCellSize,
                offset.y * skillCellSize
            );

            GameObject tile = CreateSkillRangeTile(tileWorldPos);

            bool isValid = IsValidSkillTile(tileWorldPos);
            SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
            sr.color = isValid ? validColor : invalidColor;

            skillRangeTiles.Add(tile);
        }
    }

    private GameObject CreateSkillRangeTile(Vector2 position)
    {
        GameObject tile = new GameObject("SkillRangeTile");
        tile.transform.position = position;
        tile.transform.SetParent(transform);

        SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
        sr.sprite = squareSprite;

        tile.transform.localScale = new Vector3(skillCellSize * 0.98f, skillCellSize * 0.98f, 1f);


        sr.sortingOrder = 95;

        return tile;
    }

    public void ClearSkillRange()
    {
        foreach (GameObject tile in skillRangeTiles)
        {
            if (tile != null)
                Destroy(tile);
        }
        skillRangeTiles.Clear();
    }

    public void ClearAll()
    {
        HideFullGrid();
        ClearSkillRange();
    }

    private Vector2 GetCellCenterFromWorldPos(Vector2 worldPos)
    {
        Vector2 gridStart = PathFinder.instance.gridStartPoint;

        Vector2 relativePos = worldPos - gridStart;
        int cellX = Mathf.FloorToInt(relativePos.x / skillCellSize);
        int cellY = Mathf.FloorToInt(relativePos.y / skillCellSize);

        return new Vector2(
            gridStart.x + cellX * skillCellSize + skillCellSize * 0.5f,
            gridStart.y + cellY * skillCellSize + skillCellSize * 0.5f
        );
    }

    private bool IsValidSkillTile(Vector2 worldPos)
    {
        Vector2 gridStart = PathFinder.instance.gridStartPoint;
        Vector2 gridEnd = PathFinder.instance.gridEndPoint;

        if (worldPos.x < gridStart.x || worldPos.y < gridStart.y ||
            worldPos.x > gridEnd.x || worldPos.y > gridEnd.y)
        {
            return false;
        }

        bool hasWall = Physics2D.OverlapBox(
            worldPos,
            new Vector2(skillCellSize * 0.9f, skillCellSize * 0.9f),
            0,
            PathFinder.instance.layerTocheckCollide
        );

        return !hasWall;
    }

    private Sprite CreateSquareSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f
        );
    }

    public Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
}