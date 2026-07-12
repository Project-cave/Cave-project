using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class FacilityPlacementManager : MonoBehaviour
{
    public static FacilityPlacementManager Instance { get; private set; }
    
    [Header("Tilemaps")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap backWallTilemap;
    [SerializeField] private Tilemap frontWallTilemap;
    [SerializeField] private Tilemap facilityTilemap;  // 소환진 전용 타일맵

    [Header("Placement Preview")]
    [SerializeField] private Color validColor = new Color(0f, 1f, 0f, 0.4f);
    [SerializeField] private Color invalidColor = new Color(1f, 0f, 0f, 0.4f);

    private GameObject placementIndicator;
    private SpriteRenderer indicatorRenderer;

    
    [Header("Available Facilities")]
    [SerializeField] private FacilityData[] availableFacilities;  // 건설 가능한 시설들
    
    [Header("Spawn Units (소환진용)")]
    [SerializeField] private UnitSo[] availableSpawnUnits;  // 소환 가능한 유닛들
    
    // 배치 상태
    private bool isInPlacementMode = false;
    private FacilityData selectedFacility = null;
    private UnitSo selectedSpawnUnit = null;  // 소환진에 배치할 유닛
    
    // 배치된 시설 저장
    public Dictionary<Vector3Int, GameObject> placedFacilities = new Dictionary<Vector3Int, GameObject>();
    private Dictionary<Vector3Int, FacilityData> facilityDataMap = new Dictionary<Vector3Int, FacilityData>();
    private Dictionary<Vector3Int, UnitSo> spawnFacilityUnits = new Dictionary<Vector3Int, UnitSo>();  // 소환진 → 유닛 매핑
    public Dictionary<Vector3Int, FacilityData> GetPlacedFacilityData()
    {
        return facilityDataMap;
    }
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    // 시설 선택
    public void SelectFacility(int index)
    {
        if (index < 0 || index >= availableFacilities.Length)
            return;
        
        selectedFacility = availableFacilities[index];
        selectedSpawnUnit = null;
        isInPlacementMode = true;
        
        Debug.Log($"{selectedFacility.facilityName} 배치 모드 시작");
    }

    void EnsureIndicator()
    {
        if (placementIndicator != null) return;

        placementIndicator = new GameObject("PlacementIndicator");
        indicatorRenderer = placementIndicator.AddComponent<SpriteRenderer>();
        indicatorRenderer.sprite = CreateSquareSprite();
        indicatorRenderer.sortingOrder = 10;
        placementIndicator.SetActive(false);
    }

    Sprite CreateSquareSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }

    // 마우스가 가리키는 셀에 프리뷰 표시 (TilemapManager에서 매 프레임 호출)
    public void ShowPreview(Vector3Int cellPosition)
    {
        if (!IsPlacementMode())
        {
            ClearPreview();
            return;
        }

        EnsureIndicator();

        bool valid = IsValidPlacement(cellPosition);

        Vector3 worldPos = floorTilemap.GetCellCenterWorld(cellPosition);
        placementIndicator.transform.position = worldPos;

        Vector3 cellSize = floorTilemap.cellSize;
        placementIndicator.transform.localScale = new Vector3(cellSize.x, cellSize.y, 1f);

        indicatorRenderer.color = valid ? validColor : invalidColor;
        placementIndicator.SetActive(true);
    }

    public void ClearPreview()
    {
        if (placementIndicator != null)
            placementIndicator.SetActive(false);
    }

    // 로그 없이 배치 가능 여부만 확인 (프리뷰용)
    public bool IsValidPlacement(Vector3Int cellPosition)
    {
        if (!isInPlacementMode || selectedFacility == null)
            return false;

        if (selectedFacility.isSpawnFacility)
            return CanPlaceOnFacilityTile(cellPosition, false);
        else
            return CanPlaceOnFloor(cellPosition, false);
    }

    public bool IsDragPlacementAllowed()
    {
        return selectedFacility != null && selectedFacility.allowDragPlacement;
    }

    // 시설 배치 시도
    public void TryPlaceFacility(Vector3Int cellPosition)
    {
        if (!isInPlacementMode || selectedFacility == null)
            return;

        if (selectedFacility.isSpawnFacility)
        {
            if (!CanPlaceOnFacilityTile(cellPosition, true))
                return;

            PlaceSpawnFacility(cellPosition);
        }
        else
        {
            if (!CanPlaceOnFloor(cellPosition, true))
                return;

            PlaceNormalFacility(cellPosition);
        }
    }
    
    // 소환진에 배치할 유닛 선택
    public void SelectSpawnUnit(UnitSo unit)
    {
        // 소환진 시설을 찾기
        FacilityData spawnFacility = null;
        foreach (var facility in availableFacilities)
        {
            if (facility.isSpawnFacility)
            {
                spawnFacility = facility;
                break;
            }
        }
        
        if (spawnFacility == null)
        {
            Debug.LogError("소환진 시설을 찾을 수 없습니다!");
            return;
        }
        
        selectedFacility = spawnFacility;
        selectedSpawnUnit = unit;
        isInPlacementMode = true;
        
        Debug.Log($"{unit.info.unitName} 소환진 배치 모드 시작");
    }
    
    // 배치 모드 취소
    public void CancelPlacement()
    {
        selectedFacility = null;
        selectedSpawnUnit = null;
        isInPlacementMode = false;
        
        ClearPreview();
        Debug.Log("시설 배치 모드 취소");
    }
    
    public bool IsPlacementMode()
    {
        return isInPlacementMode && selectedFacility != null;
    }
    
    public FacilityData GetSelectedFacility()
    {
        return selectedFacility;
    }
    
    // 바닥 타일에 배치 가능한지 확인
    bool CanPlaceOnFloor(Vector3Int cellPosition, bool logMessages)
    {
        if (!floorTilemap.HasTile(cellPosition))
        {
            if (logMessages) Debug.Log("바닥 타일이 아닙니다.");
            return false;
        }

        if (backWallTilemap.HasTile(cellPosition) || frontWallTilemap.HasTile(cellPosition))
        {
            if (logMessages) Debug.Log("벽이 있습니다.");
            return false;
        }

        if (placedFacilities.ContainsKey(cellPosition))
        {
            if (logMessages) Debug.Log("이미 시설이 배치되어 있습니다.");
            return false;
        }

        if (!CanAfford(selectedFacility))
        {
            if (logMessages) Debug.Log("자원이 부족합니다.");
            return false;
        }

        return true;
    }
    
    // 소환진 타일에 배치 가능한지 확인
    bool CanPlaceOnFacilityTile(Vector3Int cellPosition, bool logMessages)
    {
        if (facilityTilemap == null || !facilityTilemap.HasTile(cellPosition))
        {
            if (logMessages) Debug.Log("소환진 배치 가능 지역이 아닙니다.");
            return false;
        }

        if (placedFacilities.ContainsKey(cellPosition))
        {
            if (logMessages) Debug.Log("이미 시설이 배치되어 있습니다.");
            return false;
        }

        if (!CanAfford(selectedFacility))
        {
            if (logMessages) Debug.Log("자원이 부족합니다.");
            return false;
        }

        return true;
    }
    
    // 자원 확인
    bool CanAfford(FacilityData facility)
    {
        int wood = ResourceManager.Instance.GetResource(ResourceType.Wood);
        int scrap = ResourceManager.Instance.GetResource(ResourceType.Scrap);
        int meat = ResourceManager.Instance.GetResource(ResourceType.Meat);
        
        return wood >= facility.woodCost &&
               scrap >= facility.scrapCost &&
               meat >= facility.meatCost;
    }
    
    // 자원 지불
    void SpendResources(FacilityData facility)
    {
        if (facility.woodCost > 0)
            ResourceManager.Instance.SpendResource(ResourceType.Wood, facility.woodCost);
        
        if (facility.scrapCost > 0)
            ResourceManager.Instance.SpendResource(ResourceType.Scrap, facility.scrapCost);
        
        if (facility.meatCost > 0)
            ResourceManager.Instance.SpendResource(ResourceType.Meat, facility.meatCost);
    }
    
    // 일반 시설 배치
    void PlaceNormalFacility(Vector3Int cellPosition)
    {
        Vector3 worldPos = floorTilemap.GetCellCenterWorld(cellPosition);
        GameObject facilityObj = null;
        
        if (selectedFacility.facilityPrefab != null)
        {
            facilityObj = Instantiate(selectedFacility.facilityPrefab, worldPos, Quaternion.identity);
            // Prefab 생성 시 이름 설정
            facilityObj.name = selectedFacility.facilityName;
        }
        else
        {
            // Prefab이 없으면 기본 스프라이트로 생성
            facilityObj = new GameObject(selectedFacility.facilityName);
            facilityObj.transform.position = worldPos;
            
            SpriteRenderer sr = facilityObj.AddComponent<SpriteRenderer>();
            sr.sprite = selectedFacility.facilitySprite;
            sr.sortingOrder = 5;
        }
        
        // Box Collider 2D 자동 추가 (클릭 감지용)
        if (facilityObj.GetComponent<BoxCollider2D>() == null)
        {
            facilityObj.AddComponent<BoxCollider2D>();
        }
        
        // 저장
        placedFacilities[cellPosition] = facilityObj;
        facilityDataMap[cellPosition] = selectedFacility;
        
        // 자원 지불
        SpendResources(selectedFacility);
        
        Debug.Log($"{selectedFacility.facilityName} 배치 완료: {cellPosition}");
        
        // 배치 모드 종료
        CancelPlacement();
        ClearPreview();
    }
    
    // 소환진 배치
    void PlaceSpawnFacility(Vector3Int cellPosition)
    {
        Vector3 worldPos = floorTilemap.GetCellCenterWorld(cellPosition);
        GameObject facilityObj = null;
        
        if (selectedFacility.facilityPrefab != null)
        {
            facilityObj = Instantiate(selectedFacility.facilityPrefab, worldPos, Quaternion.identity);
            // Prefab 생성 시 이름 설정
            facilityObj.name = $"소환진_{selectedSpawnUnit?.info.unitName}";
        }
        else
        {
            facilityObj = new GameObject($"소환진_{selectedSpawnUnit?.info.unitName}");
            facilityObj.transform.position = worldPos;
            
            SpriteRenderer sr = facilityObj.AddComponent<SpriteRenderer>();
            sr.sprite = selectedFacility.facilitySprite;
            sr.sortingOrder = 5;
        }
        
        // Box Collider 2D 자동 추가 (클릭 감지용)
        if (facilityObj.GetComponent<BoxCollider2D>() == null)
        {
            facilityObj.AddComponent<BoxCollider2D>();
        }
        
        // 저장
        placedFacilities[cellPosition] = facilityObj;
        facilityDataMap[cellPosition] = selectedFacility;
        
        if (selectedSpawnUnit != null)
        {
            spawnFacilityUnits[cellPosition] = selectedSpawnUnit;
        }
        
        // 자원 지불
        SpendResources(selectedFacility);
        
        Debug.Log($"{selectedFacility.facilityName} ({selectedSpawnUnit?.info.unitName}) 배치 완료: {cellPosition}");
        
        // 배치 모드 종료
        CancelPlacement();
        ClearPreview();
    }
    
    // 시설 제거
    public void RemoveFacility(Vector3Int cellPosition)
    {
        if (!placedFacilities.ContainsKey(cellPosition))
            return;
        
        Destroy(placedFacilities[cellPosition]);
        placedFacilities.Remove(cellPosition);
        facilityDataMap.Remove(cellPosition);
        spawnFacilityUnits.Remove(cellPosition);
        
        Debug.Log($"시설 제거: {cellPosition}");
    }
    
    public FacilityData[] GetAvailableFacilities()
    {
        return availableFacilities;
    }
    
    public UnitSo[] GetAvailableSpawnUnits()
    {
        return availableSpawnUnits;
    }
}