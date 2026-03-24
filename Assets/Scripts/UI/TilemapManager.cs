using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TilemapManager : MonoBehaviour
{
    public static TilemapManager Instance { get; private set; }
    
    [Header("Tilemap References")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    
    [Header("Placement")]
    [SerializeField] private Camera mainCamera;
    
    private Dictionary<Vector3Int, GameObject> placedUnits = new Dictionary<Vector3Int, GameObject>();
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        if (mainCamera == null)
            mainCamera = Camera.main;
    }
    
    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleMouseClick();
        }
    }
    
    void HandleMouseClick()
    {
        // UI 위에서 클릭한 경우 무시
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;
        
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;
        
        Vector3Int cellPosition = floorTilemap.WorldToCell(mouseWorldPos);
        
        // 유닛 배치 모드
        if (UnitPlacementManager.Instance != null && UnitPlacementManager.Instance.IsPlacementMode())
        {
            UnitPlacementManager.Instance.TryPlaceUnit(cellPosition);
        }
        // 확장 모드
        else if (WallExpansionManager.Instance != null && WallExpansionManager.Instance.IsExpansionMode())
        {
            WallExpansionManager.Instance.TryExpand(cellPosition);
        }
    }
    
    public bool CanPlaceUnit(Vector3Int cellPosition)
    {
        if (!floorTilemap.HasTile(cellPosition))
            return false;
        
        Vector3 worldPos = floorTilemap.GetCellCenterWorld(cellPosition);
        Vector3Int wallCell = wallTilemap.WorldToCell(worldPos);
        if (wallTilemap.HasTile(wallCell))
            return false;
        
        if (placedUnits.ContainsKey(cellPosition))
            return false;
        
        return true;
    }
    
    public void PlaceUnit(Vector3Int cellPosition, GameObject unitPrefab)
    {
        if (CanPlaceUnit(cellPosition))
        {
            Vector3 worldPosition = floorTilemap.GetCellCenterWorld(cellPosition);
            GameObject unit = Instantiate(unitPrefab, worldPosition, Quaternion.identity);
            placedUnits[cellPosition] = unit;
        }
    }
    
    public void RemoveUnit(Vector3Int cellPosition)
    {
        if (placedUnits.ContainsKey(cellPosition))
        {
            Destroy(placedUnits[cellPosition]);
            placedUnits.Remove(cellPosition);
        }
    }
}
