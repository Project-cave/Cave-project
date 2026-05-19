using System.Collections.Generic;

[System.Serializable]
public class MapSaveData
{
    public List<TileInfo> removedWalls = new List<TileInfo>();
    public List<PlacedBuilding> buildings = new List<PlacedBuilding>();
    public List<PlacedUnit> units = new List<PlacedUnit>();
    public List<PlacedUnit> enemies = new List<PlacedUnit>();
    public SpawnProgress spawnProgress = new SpawnProgress();
}

[System.Serializable]
public class SpawnProgress
{
    public bool isSpawning = false;
    public int grade;
    public int stage;
    public int bronzeRemaining;
    public int silverRemaining;
    public int goldRemaining;
    public int platinumRemaining;
}
