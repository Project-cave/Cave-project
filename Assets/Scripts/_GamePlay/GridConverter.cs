using System.Collections.Generic;
using UnityEngine;

public static class GridConverter
{
    public const float LOGICAL_CELL_SIZE = 1.0f;
    public const float HALF_LOGICAL_CELL = 0.5f;

    public static Vector2 SnapToLogicalGridCenter(Vector2 worldPos)
    {
        float x = Mathf.Floor(worldPos.x) + HALF_LOGICAL_CELL;
        float y = Mathf.Floor(worldPos.y) + HALF_LOGICAL_CELL;
        return new Vector2(x, y);
    }

    public static LinkedList<Vector2> CompressPathToLogicalGrid(LinkedList<Vector2> rawPath)
    {
        if (rawPath == null || rawPath.Count == 0) return rawPath;

        LinkedList<Vector2> compressedPath = new LinkedList<Vector2>();
        Vector2 lastAddedNode = new Vector2(float.MinValue, float.MinValue);

        foreach (Vector2 rawPos in rawPath)
        {
            Vector2 snappedPos = SnapToLogicalGridCenter(rawPos);

            if (snappedPos != lastAddedNode)
            {
                compressedPath.AddLast(snappedPos);
                lastAddedNode = snappedPos;
            }
        }

        return compressedPath;
    }
}