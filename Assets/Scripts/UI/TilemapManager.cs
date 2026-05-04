using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class TilemapManager : MonoBehaviour
{
    public static TilemapManager Instance { get; private set; }
    
    [Header("Tilemap References")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    
    [Header("Placement")]
    [SerializeField] private Camera mainCamera;
    
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
        
        // 시설 배치 모드
        if (FacilityPlacementManager.Instance != null && FacilityPlacementManager.Instance.IsPlacementMode())
        {
            FacilityPlacementManager.Instance.TryPlaceFacility(cellPosition);
        }
        // 확장 모드
        else if (WallExpansionManager.Instance != null && WallExpansionManager.Instance.IsExpansionMode())
        {
            WallExpansionManager.Instance.TryExpand(cellPosition);
        }
    }
}