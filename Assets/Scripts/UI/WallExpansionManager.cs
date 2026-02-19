using UnityEngine;
using UnityEngine.Tilemaps;

public class WallExpansionManager : MonoBehaviour
{
    public static WallExpansionManager Instance { get; private set; }
    
    [Header("Tilemap References")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    
    [Header("Settings")]
    [SerializeField] private float expansionDelay = 5f;
    
    private bool isExpansionMode = false;
    
    // 4방향 (상하좌우, 대각선 제외)
    private static readonly Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(0, 1, 0),   // 위
        new Vector3Int(0, -1, 0),  // 아래
        new Vector3Int(1, 0, 0),   // 오른쪽
        new Vector3Int(-1, 0, 0)   // 왼쪽
    };
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public void SetExpansionMode(bool mode)
    {
        isExpansionMode = mode;
    }
    
    public bool IsExpansionMode()
    {
        return isExpansionMode;
    }
    
    public void TryExpand(Vector3Int cellPosition)
    {
        if (!isExpansionMode) return;
        
        // 1. 해당 위치에 벽 타일이 있는지 확인
        Vector3 worldPos = floorTilemap.GetCellCenterWorld(cellPosition);
        Vector3Int wallCell = wallTilemap.WorldToCell(worldPos);
        
        if (!wallTilemap.HasTile(wallCell))
        {
            Debug.Log("이 위치에 벽 타일이 없습니다!");
            return;
        }
        
        // 2. 상하좌우 인접 중 하나라도 노출된 바닥 타일이 있는지 확인
        if (!HasAdjacentFloor(cellPosition))
        {
            Debug.Log("이 벽 타일은 바닥 타일과 접하지 않습니다!");
            return;
        }
        
        // 3. 5초 후 벽 타일 제거
        StartCoroutine(RemoveWallAfterDelay(wallCell));
        
        // 확장 모드 종료
        isExpansionMode = false;
    }
    
    // 상하좌우 중 벽이 없는 바닥 타일이 있는지 확인
    bool HasAdjacentFloor(Vector3Int cellPosition)
    {
        foreach (var dir in directions)
        {
            Vector3Int adjacentCell = cellPosition + dir;
            
            // 인접 셀에 바닥 타일이 있는지
            if (!floorTilemap.HasTile(adjacentCell))
                continue;
            
            // 인접 셀에 벽 타일이 없는지 (= 노출된 바닥)
            Vector3 adjWorldPos = floorTilemap.GetCellCenterWorld(adjacentCell);
            Vector3Int adjWallCell = wallTilemap.WorldToCell(adjWorldPos);
            
            if (!wallTilemap.HasTile(adjWallCell))
                return true;
        }
        return false;
    }
    
    System.Collections.IEnumerator RemoveWallAfterDelay(Vector3Int wallCell)
    {
        yield return new WaitForSeconds(expansionDelay);
        wallTilemap.SetTile(wallCell, null);
    }
}
