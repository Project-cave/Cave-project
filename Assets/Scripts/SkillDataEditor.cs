#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillData))]
public class SkillDataEditor : Editor
{
    private const int GRID_SIZE = 11;
    private const int CELL_SIZE = 25;
    private const int CENTER = GRID_SIZE / 2;

    private bool[,] grid = new bool[GRID_SIZE, GRID_SIZE];
    private bool isDragging = false;
    private bool dragMode = true;

    private void OnEnable()
    {
        LoadGridFromSkillData();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("Skill Pattern", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("좌클릭: 그리기 | 우클릭: 지우기", MessageType.Info);

        DrawGrid();

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Clear", GUILayout.Height(30)))
        {
            ClearGrid();
        }

        SkillData skillData = (SkillData)target;
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField($"Total Tiles: {skillData.affectedTiles.Count}", EditorStyles.miniLabel);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(skillData);
        }
    }

    private void DrawGrid()
    {
        Rect startRect = GUILayoutUtility.GetRect(GRID_SIZE * CELL_SIZE, GRID_SIZE * CELL_SIZE);
        Event e = Event.current;

        for (int y = 0; y < GRID_SIZE; y++)
        {
            for (int x = 0; x < GRID_SIZE; x++)
            {
                Rect cellRect = new Rect(
                    startRect.x + x * CELL_SIZE,
                    startRect.y + y * CELL_SIZE,
                    CELL_SIZE - 1,
                    CELL_SIZE - 1
                );

                Color cellColor;
                if (x == CENTER && y == CENTER)
                {
                    cellColor = new Color(0.3f, 0.5f, 1f, 0.8f);
                }
                else if (grid[x, y])
                {
                    cellColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);
                }
                else
                {
                    cellColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
                }

                EditorGUI.DrawRect(cellRect, cellColor);

                if (cellRect.Contains(e.mousePosition))
                {
                    if (x == CENTER && y == CENTER) continue;

                    if (e.type == EventType.MouseDown && e.button == 0)
                    {
                        isDragging = true;
                        dragMode = !grid[x, y];
                        grid[x, y] = dragMode;
                        SaveGridToSkillData();
                        e.Use();
                    }
                    else if (e.type == EventType.MouseDown && e.button == 1)
                    {
                        isDragging = true;
                        dragMode = false;
                        grid[x, y] = false;
                        SaveGridToSkillData();
                        e.Use();
                    }
                    else if (isDragging && e.type == EventType.MouseDrag)
                    {
                        grid[x, y] = dragMode;
                        SaveGridToSkillData();
                        e.Use();
                    }
                }
            }
        }

        if (e.type == EventType.MouseUp)
        {
            isDragging = false;
        }

        GUIStyle centerStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        centerStyle.normal.textColor = Color.white;

        Rect centerRect = new Rect(
            startRect.x + CENTER * CELL_SIZE,
            startRect.y + CENTER * CELL_SIZE,
            CELL_SIZE - 1,
            CELL_SIZE - 1
        );
        GUI.Label(centerRect, "C", centerStyle);
    }

    private void LoadGridFromSkillData()
    {
        SkillData skillData = (SkillData)target;

        for (int y = 0; y < GRID_SIZE; y++)
        {
            for (int x = 0; x < GRID_SIZE; x++)
            {
                grid[x, y] = false;
            }
        }

        grid[CENTER, CENTER] = true;

        foreach (Vector2Int tile in skillData.affectedTiles)
        {
            int gridX = CENTER + tile.x;
            int gridY = CENTER - tile.y;

            if (gridX >= 0 && gridX < GRID_SIZE && gridY >= 0 && gridY < GRID_SIZE)
            {
                grid[gridX, gridY] = true;
            }
        }
    }

    private void SaveGridToSkillData()
    {
        SkillData skillData = (SkillData)target;
        skillData.affectedTiles.Clear();

        for (int y = 0; y < GRID_SIZE; y++)
        {
            for (int x = 0; x < GRID_SIZE; x++)
            {
                if (grid[x, y])
                {
                    Vector2Int offset = new Vector2Int(x - CENTER, CENTER - y);
                    skillData.affectedTiles.Add(offset);
                }
            }
        }

        EditorUtility.SetDirty(skillData);
    }

    private void ClearGrid()
    {
        for (int y = 0; y < GRID_SIZE; y++)
        {
            for (int x = 0; x < GRID_SIZE; x++)
            {
                grid[x, y] = false;
            }
        }
        grid[CENTER, CENTER] = true;
        SaveGridToSkillData();
    }
}
#endif