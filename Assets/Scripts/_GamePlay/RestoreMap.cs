using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RestoreMap : MonoBehaviour
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap floorTilemap;

    [Header("Prefab")]
    [SerializeField] private UnitData[] buildingData;
    [SerializeField] private UnitSo[] unitData;

    [SerializeField] private PoolManager poolManager;

    private void Start()
    {
        Restore();
    }

    void Restore()
    {
        RestoreWalls();
        RestoreBuildings(); 
        RestoreUnits();
    }

    private void RestoreWalls()
    {
        var removedWalls = MapManager.instance.GetRemovedWalls();
        foreach(var wallCell in removedWalls)
        {
            Debug.Log($"맵 복원 {wallCell}");
            wallTilemap.SetTile(wallCell, null);
        } 
    }

    private void RestoreBuildings()
    {
        var buildings = MapManager.instance.GetPlacedBuildings();

        foreach(var kvp in buildings)
        {
            Vector3Int cellPos = kvp.Key;
            string prefabName = kvp.Value;

            UnitData data = System.Array.Find(buildingData, p => p.unitName == prefabName);
            GameObject prefab = data.unitPrefab;
            Debug.Log(prefab);

            if (prefab == null)
            {
                Debug.LogWarning($"[MapRestorer] 건물 프리팹을 찾을 수 없음: {prefabName}");
                continue;
            }

           

            // 월드 좌표로 변환
            Vector3 worldPos = floorTilemap.GetCellCenterWorld(cellPos);

            // 건물 생성
            GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity);

            //// TilemapManager에 등록
            //if (TilemapManager.Instance != null)
            //{
            //    TilemapManager.Instance.RegisterBuilding(cellPos, instance);
            //}
        }
    }

    private void RestoreUnits()
    {
        var units = MapManager.instance.GetCurrentUnits();
        Debug.Log(units.Values.Count);

        foreach (var kvp in units)
        {
            Vector3Int cellPos = kvp.Key;
            string unitNamesStr = kvp.Value;

            // "Sword,Wizard,Archer" 형태를 분리
            string[] unitNames = unitNamesStr.Split(',');

            

            foreach (string unitName in unitNames)
            {
                string trimmedName = unitName.Trim();

                // unitData에서 해당 유닛 찾기
                UnitSo data = System.Array.Find(unitData, u => u.unitName == trimmedName);

                if (data == null)
                {
                    Debug.LogWarning($"[MapRestorer] 유닛 데이터를 찾을 수 없음: {trimmedName}");
                    continue;
                }

                // 유닛 인덱스 찾기
                int unitIndex = System.Array.IndexOf(unitData, data);

                if (unitIndex < 0 || poolManager == null)
                {
                    Debug.LogWarning($"[MapRestorer] 유닛 인덱스 또는 PoolManager가 없음: {trimmedName}");
                    continue;
                }

                // 풀에서 유닛 가져오기
                GameObject unit = poolManager.Get(unitIndex);

                // 월드 좌표로 변환
                Vector3 worldPos = floorTilemap.GetCellCenterWorld(cellPos);
                unit.transform.position = worldPos;

                // 유닛 초기화
                Unit unitComponent = unit.GetComponent<Unit>();
                if (unitComponent != null)
                {
                    unitComponent.InitUnit(data);
                }

                UnitManager.instance.RegisterUnit(unit);

                Debug.Log($"유닛 복원: {trimmedName} at {cellPos} (index: {unitIndex})");
            }
        }
    }
}
