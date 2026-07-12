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

    private Vector3Int lastDragCell = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
    
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
    bool inPlacementMode = FacilityPlacementManager.Instance != null &&
                            FacilityPlacementManager.Instance.IsPlacementMode();

    if (inPlacementMode)
    {
        UpdatePlacementPreview();
        // 우클릭으로 배치 취소
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            FacilityPlacementManager.Instance.CancelPlacement();
            return;
        }

        // ESC 키로 배치 취소
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            FacilityPlacementManager.Instance.CancelPlacement();
            return;
        }
    }

    if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
    {
        HandleMouseClick();
    }

    // 드래그 연속 배치 (버튼을 누르고 있는 동안)
    if (inPlacementMode &&
        FacilityPlacementManager.Instance.IsDragPlacementAllowed() &&
        Mouse.current != null && Mouse.current.leftButton.isPressed)
    {
        HandleDragPlacement();
    }

    if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
    {
        lastDragCell = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
    }
}

bool IsPointerOverUI()
{
    return UnityEngine.EventSystems.EventSystem.current != null &&
           UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
}

Vector3Int GetMouseCellPosition()
{
    Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
    Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
    mouseWorldPos.z = 0;
    return floorTilemap.WorldToCell(mouseWorldPos);
}

void UpdatePlacementPreview()
{
    if (IsPointerOverUI())
    {
        FacilityPlacementManager.Instance.ClearPreview();
        return;
    }

    Vector3Int cellPosition = GetMouseCellPosition();
    FacilityPlacementManager.Instance.ShowPreview(cellPosition);
}

void HandleDragPlacement()
{
    if (IsPointerOverUI()) return;

    Vector3Int cellPosition = GetMouseCellPosition();
    if (cellPosition == lastDragCell) return;

    lastDragCell = cellPosition;
    FacilityPlacementManager.Instance.TryPlaceFacility(cellPosition);
}
    
void HandleMouseClick()
{
    if (IsPointerOverUI())
        return;

    Vector3Int cellPosition = GetMouseCellPosition();

    if (FacilityPlacementManager.Instance != null && FacilityPlacementManager.Instance.IsPlacementMode())
    {
        // 드래그 배치 시설은 클릭이 아니라 드래그로만 배치 (중복 배치 방지)
        if (!FacilityPlacementManager.Instance.IsDragPlacementAllowed())
        {
            FacilityPlacementManager.Instance.TryPlaceFacility(cellPosition);
        }
    }
    else if (WallExpansionManager.Instance != null && WallExpansionManager.Instance.IsExpansionMode())
    {
        WallExpansionManager.Instance.TryExpand(cellPosition);
    }
}
}