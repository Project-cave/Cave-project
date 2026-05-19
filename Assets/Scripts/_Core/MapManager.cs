using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    private HashSet<Vector3Int> removedWalls = new HashSet<Vector3Int>();
    private Dictionary<Vector3Int, string> placedBuildings = new Dictionary<Vector3Int, string>();
    private Dictionary<Vector3Int, string> currentUnits = new Dictionary<Vector3Int, string>();
    private Dictionary<int, string> currentEnemies = new Dictionary<int, string>();
    private SpawnProgress spawnProgress = new SpawnProgress();

    public SpawnProgress GetSpawnProgress() => spawnProgress;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region wall

    public void RecordWallRemoval(Vector3Int wallCell)
    {
        removedWalls.Add(wallCell);
        Debug.Log($"벽 제거 기록: {wallCell}");
    }

    public HashSet<Vector3Int> GetRemovedWalls()
    {
        return removedWalls;
    }

    #endregion



    #region Building

    public void RecordPlacedBuilding(Vector3Int cellPosition, string prefabName)
    {
        placedBuildings[cellPosition] = prefabName;
        Debug.Log($"건물 기록: {prefabName}");
    }

    public void RemovePlacedObject(Vector3Int cellPosition)
    {
        if (placedBuildings.ContainsKey(cellPosition))
        {
            placedBuildings.Remove(cellPosition);
            Debug.Log($"건물 제거 기록: {cellPosition}");
        }
    }
    
    public Dictionary<Vector3Int, string> GetPlacedBuildings()
    {
        return placedBuildings;
    }

    #endregion



    #region Unit

    public void UpdateCurrentUnits(Dictionary<Vector3Int, string> units)
    {
        currentUnits = new Dictionary<Vector3Int, string>(units);
        Debug.Log($"유닛 위치 업데이트: {units.Count}개");
    }

    public Dictionary<Vector3Int, string> GetCurrentUnits()
    {
        return currentUnits;
    }

    public bool HasSavedEnemies()
    {
        return currentEnemies != null && currentEnemies.Count > 0;
    }

    public void UpdateCurrentEnemies(Dictionary<int, string> enemies)
    {
        currentEnemies = new Dictionary<int, string>(enemies);
    }

    public Dictionary<int, string> GetCurrentEnemies()
    {
        return currentEnemies;
    }

    public void SaveSpawnProgress(SpawnProgress progress)
    {
        spawnProgress = progress;
        Save();
    }

    public void ClearSpawnProgress()
    {
        spawnProgress = new SpawnProgress();
        Save();
    }

    #endregion



    #region Save / Load

    public void Save()
    {
        MapSaveData data = new MapSaveData();

        // 1. 제거된 벽
        foreach(var wall in removedWalls)
        {
            data.removedWalls.Add(new TileInfo(wall));
        }

        // 2. 건물
        foreach(var kvp in placedBuildings)
        {
            data.buildings.Add(new PlacedBuilding
            {
                x = kvp.Key.x,
                y = kvp.Key.y,
                z = kvp.Key.z,
                buildingPrefabName = kvp.Value
            });
        }

        // 3. 유닛
        foreach (var kvp in currentUnits)
        {
            data.units.Add(new PlacedUnit
            {
                x = kvp.Key.x,
                y = kvp.Key.y,
                z = kvp.Key.z,
                unitPrefabName = kvp.Value
            });
        }

        // 4. 적
        foreach (var kvp in currentEnemies)
        {
            data.enemies.Add(new PlacedUnit
            {
                x = kvp.Key, // int ID로 재활용
                y = 0,
                z = 0,
                unitPrefabName = kvp.Value
            });
        }

        data.spawnProgress = spawnProgress;

        string json = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(SavePath(), json);
    }

    public void Load()
    {
        string path = SavePath();
        if (!System.IO.File.Exists(path))
        {
            Debug.Log("저장된 맵 데이터 없음 - 새 게임");
            return;
        }

        string json = System.IO.File.ReadAllText(path);
        MapSaveData data = JsonUtility.FromJson<MapSaveData>(json);

        // 1. 벽
        removedWalls.Clear();
        foreach(var wall in data.removedWalls)
        {
            removedWalls.Add(wall.ToVector3Int());
            Debug.Log($"벽 제거 : {wall.ToVector3Int()}");
        }

        // 2. 건물
        placedBuildings.Clear();
        foreach (var unit in data.buildings)
        {
            Vector3Int pos = new Vector3Int(unit.x, unit.y, unit.z);
            placedBuildings[pos] = unit.buildingPrefabName;
        }

        // 3. 유닛
        currentUnits.Clear();
        foreach (var unit in data.units)
        {
            Vector3Int pos = new Vector3Int(unit.x, unit.y, unit.z);
            currentUnits[pos] = unit.unitPrefabName;
        }

        // 4. 적
        currentEnemies.Clear();
        foreach (var e in data.enemies)
        {
            currentEnemies[e.x] = e.unitPrefabName;
        }

        spawnProgress = data.spawnProgress ?? new SpawnProgress();

    }

    private string SavePath()
    {
        return Application.persistentDataPath + "/mapData.json";
    }

    #endregion
}