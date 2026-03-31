using UnityEngine;

[System.Serializable]
public class TileInfo
{
    public int x, y, z;

    public TileInfo(Vector3Int v)
    {
        x = v.x; y = v.y; z = v.z;
    }

    public Vector3Int ToVector3Int()
    {
        return new Vector3Int(x, y, z);
    }
}
