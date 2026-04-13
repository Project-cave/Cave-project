using UnityEngine;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;

    private Dictionary<Vector3Int, List<GameObject>> activeUnits = new Dictionary<Vector3Int, List<GameObject>>();
    private Dictionary<GameObject, Vector3Int> unitLastPosition = new Dictionary<GameObject, Vector3Int>();

    [SerializeField] private Grid grid;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterUnit(GameObject unit)
    {
        if (unit == null || grid == null) return;

        Vector3Int cellPos = grid.WorldToCell(unit.transform.position);

        if (!activeUnits.ContainsKey(cellPos))
        {
            activeUnits[cellPos] = new List<GameObject>();
        }

        if (!activeUnits[cellPos].Contains(unit))
        {
            activeUnits[cellPos].Add(unit);
        }

        unitLastPosition[unit] = cellPos;
    }

    public void UnRegisterUnit(GameObject unit)
    {
        if (unit == null) return;

        if (unitLastPosition.ContainsKey(unit))
        {
            Vector3Int cellPos = unitLastPosition[unit];

            if (activeUnits.ContainsKey(cellPos))
            {
                activeUnits[cellPos].Remove(unit);

                if (activeUnits[cellPos].Count == 0)
                {
                    activeUnits.Remove(cellPos);
                }
            }

            unitLastPosition.Remove(unit);
            
        }
    }

    public void OnUnitMoved(GameObject unit, Vector3Int oldPos, Vector3Int newPos)
    {
        if (unit == null || grid == null) return;
        if (oldPos == newPos) return; // 같은 위치면 무시

        // 이전 위치에서 제거
        if (activeUnits.ContainsKey(oldPos))
        {
            activeUnits[oldPos].Remove(unit);

            if (activeUnits[oldPos].Count == 0)
            {
                activeUnits.Remove(oldPos);
            }
        }

        // 새 위치에 등록
        if (!activeUnits.ContainsKey(newPos))
        {
            activeUnits[newPos] = new List<GameObject>();
        }

        if (!activeUnits[newPos].Contains(unit))
        {
            activeUnits[newPos].Add(unit);
        }

        unitLastPosition[unit] = newPos;

        UpdateMapManager();
        Debug.Log($"[UnitManager] 유닛 이동: {unit.name} {oldPos} → {newPos}");
    }

    private void UpdateMapManager()
    {
        if (MapManager.instance == null) return;

        Dictionary<Vector3Int, string> unitData = new Dictionary<Vector3Int, string>();

        Debug.Log(activeUnits.Values.Count);

        foreach (var kvp in activeUnits)
        {
            List<string> unitNames = new List<string>();

            foreach (GameObject unit in kvp.Value)
            {
                if (unit != null && unit.activeSelf)
                {
                    string prefabName = unit.GetComponent<Unit>().unitData.unitName;
                    unitNames.Add(prefabName);
                }
            }

            if (unitNames.Count > 0)
            {
                unitData[kvp.Key] = string.Join(",", unitNames);
            }
        }

        MapManager.instance.UpdateCurrentUnits(unitData);
    }
}
