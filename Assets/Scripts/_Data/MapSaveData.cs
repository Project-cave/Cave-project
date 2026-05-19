using System.Collections.Generic;

[System.Serializable]
public class MapSaveData
{
    public List<TileInfo> removedWalls = new List<TileInfo>();
    public List<PlacedBuilding> buildings = new List<PlacedBuilding>();
    public List<PlacedUnit> units = new List<PlacedUnit>();
    public List<PlacedUnit> enemies = new List<PlacedUnit>();
}
