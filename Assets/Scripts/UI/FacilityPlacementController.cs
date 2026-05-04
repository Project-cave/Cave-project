using UnityEngine;

public class FacilityPlacementController : MonoBehaviour
{
    public static FacilityPlacementController Instance { get; private set; }
    
    private bool isInPlacementMode = false;
    private UnitSo selectedUnit = null;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public void StartPlacement(UnitSo unit)
    {
        selectedUnit = unit;
        isInPlacementMode = true;
        
        Debug.Log($"{unit.info.unitName} 생산 시설 배치 모드 시작");
    }
    
    public void CancelPlacement()
    {
        selectedUnit = null;
        isInPlacementMode = false;
        
        Debug.Log("생산 시설 배치 모드 취소");
    }
    
    public bool IsPlacementMode()
    {
        return isInPlacementMode && selectedUnit != null;
    }
    
    public void TryPlaceFacility(Vector3Int cellPosition)
    {
        if (!isInPlacementMode || selectedUnit == null)
            return;
        
        if (SpawnFacilityManager.Instance == null)
            return;
        
        if (SpawnFacilityManager.Instance.CanPlaceFacility(cellPosition))
        {
            SpawnFacilityManager.Instance.PlaceFacility(cellPosition, selectedUnit);
            CancelPlacement();
        }
    }
    
    public UnitSo GetSelectedUnit()
    {
        return selectedUnit;
    }
}