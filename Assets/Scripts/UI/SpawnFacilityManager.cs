using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class SpawnFacilityManager : MonoBehaviour
{
    public static SpawnFacilityManager Instance { get; private set; }
    
    [Header("Tilemap")]
    [SerializeField] private Tilemap facilityTilemap;  // 유닛 생산 시설 전용 타일맵
    [SerializeField] private Tilemap floorTilemap;     // 바닥 타일맵 (좌표 변환용)
    
    [Header("Available Units")]
    [SerializeField] private UnitSo[] availableUnits;  // 배치 가능한 유닛들
    
    [Header("Facility Settings")]
    [SerializeField] private GameObject facilityPrefab;  // 생산 시설 프리팹
    [SerializeField] private Sprite defaultFacilitySprite;  // 기본 포탈 스프라이트
    
    // 배치된 시설 정보 저장 (셀 위치 -> 유닛)
    public Dictionary<Vector3Int, UnitSo> placedFacilities { get; private set; } = new Dictionary<Vector3Int, UnitSo>();
    // 배치된 시설 GameObject 저장
    private Dictionary<Vector3Int, GameObject> facilityObjects = new Dictionary<Vector3Int, GameObject>();
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        FindFacilityLocations();
    }
    
    void FindFacilityLocations()
    {
        if (facilityTilemap == null)
            return;
        
        BoundsInt bounds = facilityTilemap.cellBounds;
        
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                
                if (facilityTilemap.HasTile(cellPos))
                {
                    Debug.Log($"유닛 생산 시설 위치 발견: {cellPos}");
                }
            }
        }
    }
    
    public bool IsFacilityLocation(Vector3Int cellPosition)
    {
        if (facilityTilemap == null)
            return false;
        
        return facilityTilemap.HasTile(cellPosition);
    }
    
    public bool HasFacility(Vector3Int cellPosition)
    {
        return placedFacilities.ContainsKey(cellPosition);
    }
    
    public bool CanPlaceFacility(Vector3Int cellPosition)
    {
        if (!IsFacilityLocation(cellPosition))
        {
            Debug.Log("생산 시설 배치 가능 지역이 아닙니다.");
            return false;
        }
        
        if (HasFacility(cellPosition))
        {
            Debug.Log("이미 시설이 배치되어 있습니다.");
            return false;
        }
        
        return true;
    }
    
    public void PlaceFacility(Vector3Int cellPosition, UnitSo unitData)
    {
        if (!CanPlaceFacility(cellPosition))
            return;
        
        // 시설 GameObject 생성
        Vector3 worldPos = floorTilemap.GetCellCenterWorld(cellPosition);
        GameObject facilityObj = Instantiate(facilityPrefab, worldPos, Quaternion.identity);
        
        // 스프라이트 설정 (기본 포탈 이미지 사용)
        SpriteRenderer sr = facilityObj.GetComponent<SpriteRenderer>();
        if (sr != null && defaultFacilitySprite != null)
        {
            sr.sprite = defaultFacilitySprite;
        }
        
        // 저장
        placedFacilities[cellPosition] = unitData;
        facilityObjects[cellPosition] = facilityObj;
        
        Debug.Log($"{unitData.info.unitName} 생산 시설 배치 완료: {cellPosition}");
    }
    
    public void RemoveFacility(Vector3Int cellPosition)
    {
        if (!HasFacility(cellPosition))
            return;
        
        if (facilityObjects.ContainsKey(cellPosition))
        {
            Destroy(facilityObjects[cellPosition]);
            facilityObjects.Remove(cellPosition);
        }
        
        placedFacilities.Remove(cellPosition);
        
        Debug.Log($"생산 시설 제거: {cellPosition}");
    }
    
    public UnitSo GetFacilityUnit(Vector3Int cellPosition)
    {
        if (placedFacilities.ContainsKey(cellPosition))
            return placedFacilities[cellPosition];
        
        return null;
    }
    
    public UnitSo[] GetAvailableUnits()
    {
        return availableUnits;
    }
}