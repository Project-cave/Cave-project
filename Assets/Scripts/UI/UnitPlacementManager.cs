using UnityEngine;

public class UnitPlacementManager : MonoBehaviour
{
    public static UnitPlacementManager Instance { get; private set; }
    
    [Header("Unit Data")]
    [SerializeField] private UnitData[] unitDataArray;
    
    private UnitData selectedUnit = null;
    private bool isInPlacementMode = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SelectUnit(int unitIndex)
    {
        if (unitIndex >= 0 && unitIndex < unitDataArray.Length)
        {
            selectedUnit = unitDataArray[unitIndex];
            isInPlacementMode = true;
            Debug.Log($"선택된 유닛: {selectedUnit.unitName} (비용: {selectedUnit.cost})");
        }
    }
    
    public void CancelPlacement()
    {
        selectedUnit = null;
        isInPlacementMode = false;
    }
    
    public bool IsPlacementMode()
    {
        return isInPlacementMode && selectedUnit != null;
    }
    
    public void TryPlaceUnit(Vector3Int cellPosition)
    {
        if (!isInPlacementMode || selectedUnit == null)
            return;
        
        if (!TilemapManager.Instance.CanPlaceUnit(cellPosition))
        {
            Debug.Log("이 위치에는 유닛을 배치할 수 없습니다!");
            return;
        }
        
        if (!ResourceManager.Instance.CanAfford(selectedUnit.cost))
        {
            Debug.Log("자원이 부족합니다!");
            return;
        }
        
        // 자원 소모
        ResourceManager.Instance.SpendResources(selectedUnit.cost);
        
        // 유닛 배치
        TilemapManager.Instance.PlaceUnit(cellPosition, selectedUnit.unitPrefab);
        
        Debug.Log($"{selectedUnit.unitName} 배치 완료!");
        
        // 배치 모드 종료
        CancelPlacement();
    }
    
    public UnitData GetSelectedUnit()
    {
        return selectedUnit;
    }
}