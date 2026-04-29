using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance { get; private set; }
    
    [Header("Item Data")]
    [SerializeField] private CraftItemData poisonPotionData;
    [SerializeField] private CraftItemData meatData;
    [SerializeField] private CraftItemData poisonGasData;
    [SerializeField] private CraftItemData poisonCrystalData;
    
    [Header("Fire Salamander Items")]
    [SerializeField] private CraftItemData ashData;
    [SerializeField] private CraftItemData fireScaleData;
    [SerializeField] private CraftItemData ironIngotData;
    [SerializeField] private CraftItemData steelData;
    [SerializeField] private CraftItemData hellSteelData;
    
    [Header("Arachne Items")]
    [SerializeField] private CraftItemData threadData;
    [SerializeField] private CraftItemData ropeData;
    [SerializeField] private CraftItemData clothData;
    [SerializeField] private CraftItemData clothArmorData;
    
    [Header("Death Statue Items")]
    [SerializeField] private CraftItemData humanData;          // 인간
    [SerializeField] private CraftItemData soulData;           // 영혼
    [SerializeField] private CraftItemData soulAshData;        // 영혼재
    [SerializeField] private CraftItemData soulSteelData;      // 영혼철
    
    [Header("Dwarf Workshop Items")]
    [SerializeField] private CraftItemData boneData;           // 뼈
    [SerializeField] private CraftItemData steelSwordData;     // 강철검
    [SerializeField] private CraftItemData steelArmorData;     // 강철갑옷
    [SerializeField] private CraftItemData hellArmorData;      // 지옥갑옷
    [SerializeField] private CraftItemData hellSwordData;      // 지옥검
    
    [Header("Crafting Settings")]
    [SerializeField] private float poisonCraftTime = 5f;
    [SerializeField] private int meatRequiredCount = 2;
    
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    
    // 시설별 제작 상태 (각 독통마다 독립적으로 제작 가능)
    private Dictionary<GameObject, CraftingState> facilityStates = new Dictionary<GameObject, CraftingState>();
    
    // 제작 상태 클래스
    private class CraftingState
    {
        public bool isCrafting;
        public float currentTime;
        public float targetTime;
        public CraftItemData outputItem;
        public Coroutine craftCoroutine;
    }
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        if (mainCamera == null)
            mainCamera = Camera.main;
    }
    
    void Start()
    {
        // 시작 시 테스트 아이템 추가
        if (InventoryManager.Instance != null)
        {
            // 기본 재료
            if (meatData != null)
            {
                InventoryManager.Instance.AddItem(meatData, 30);
                Debug.Log("===== 게임 시작: 고기 30개 추가됨 =====");
            }
            
            if (poisonPotionData != null)
            {
                InventoryManager.Instance.AddItem(poisonPotionData, 10);
                Debug.Log("===== 게임 시작: 독 포션 10개 추가됨 =====");
            }
        }
        else
        {
            Debug.LogError("아이템 추가 실패! InventoryManager가 NULL");
        }
    }
    
    void Update()
    {
        // 좌클릭으로 시설 클릭
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            bool isPlacementMode = FacilityPlacementManager.Instance != null && 
                                  FacilityPlacementManager.Instance.IsPlacementMode();
            bool isExpansionMode = WallExpansionManager.Instance != null && 
                                  WallExpansionManager.Instance.IsExpansionMode();
            
            if (!isPlacementMode && !isExpansionMode)
            {
                HandleFacilityClick();
            }
        }
    }
    
    void HandleFacilityClick()
    {
        // UI 클릭 무시
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;
        
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;
        
        // 2D Raycast로 클릭한 오브젝트 찾기
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        
        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;
            
            // 독통 클릭 시
            if (clickedObject.name.Contains("독통"))
            {
                TryStartPoisonPotionCraft(clickedObject);
            }
            // 독거북 클릭 시
            else if (clickedObject.name.Contains("독거북"))
            {
                ShowPoisonTurtleRecipeSelection(clickedObject);
            }
            // 불도마뱀 클릭 시
            else if (clickedObject.name.Contains("불도마뱀"))
            {
                ShowFireSalamanderRecipeSelection(clickedObject);
            }
            // 아라크네 클릭 시
            else if (clickedObject.name.Contains("아라크네"))
            {
                ShowArachneRecipeSelection(clickedObject);
            }
            // 사신 조각상 클릭 시
            else if (clickedObject.name.Contains("사신"))
            {
                ShowDeathStatueRecipeSelection(clickedObject);
            }
            // 드워프 공방 클릭 시
            else if (clickedObject.name.Contains("드워프"))
            {
                ShowDwarfWorkshopRecipeSelection(clickedObject);
            }
        }
    }
    
    void TryStartPoisonPotionCraft(GameObject facility)
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager가 없습니다!");
            return;
        }
        
        // 이 시설이 제작 중인지 확인
        if (IsFacilityCrafting(facility))
        {
            Debug.Log($"{facility.name}은(는) 이미 제작 중입니다!");
            return;
        }
        
        // 재료 부족해도 패널은 열기 (UI에서 빨간색으로 표시됨)
        ShowCraftConfirm(
            facility,
            poisonPotionData,
            new CraftItemData[] { meatData },
            new int[] { meatRequiredCount },
            poisonCraftTime
        );
    }
    
    void ShowPoisonTurtleRecipeSelection(GameObject facility)
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager가 없습니다!");
            return;
        }
        
        // 이 시설이 제작 중인지 확인
        if (IsFacilityCrafting(facility))
        {
            Debug.Log($"{facility.name}은(는) 이미 제작 중입니다!");
            return;
        }
        
        // 3개 레시피 준비
        RecipeData[] recipes = new RecipeData[]
        {
            new RecipeData
            {
                outputItem = poisonPotionData,
                outputCount = 1,
                inputItems = new CraftItemData[] { meatData },
                inputCounts = new int[] { 2 },
                craftTime = 5f,
                recipeName = "독 포션 제작"
            },
            new RecipeData
            {
                outputItem = poisonGasData,
                outputCount = 2,
                inputItems = new CraftItemData[] { poisonPotionData },
                inputCounts = new int[] { 1 },
                craftTime = 5f,
                recipeName = "독가스 제작"
            },
            new RecipeData
            {
                outputItem = poisonCrystalData,
                outputCount = 1,
                inputItems = new CraftItemData[] { poisonGasData },
                inputCounts = new int[] { 10 },
                craftTime = 5f,
                recipeName = "맹독결정 제작"
            }
        };
        
        // UIManager에 레시피 선택 패널 표시 요청
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowRecipeSelection(facility, recipes);
        }
    }
    
    void ShowFireSalamanderRecipeSelection(GameObject facility)
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager가 없습니다!");
            return;
        }
        
        // 이 시설이 제작 중인지 확인
        if (IsFacilityCrafting(facility))
        {
            Debug.Log($"{facility.name}은(는) 이미 제작 중입니다!");
            return;
        }
        
        // 5개 레시피 준비
        RecipeData[] recipes = new RecipeData[]
        {
            new RecipeData
            {
                outputItem = ashData,
                outputCount = 1,
                inputItems = null,
                inputCounts = null,
                inputResources = new ResourceType[] { ResourceType.Wood },
                inputResourceCounts = new int[] { 1 },
                craftTime = 5f,
                recipeName = "재 제작"
            },
            new RecipeData
            {
                outputItem = fireScaleData,
                outputCount = 1,
                inputItems = new CraftItemData[] { meatData, ashData },
                inputCounts = new int[] { 10, 10 },
                inputResources = null,
                inputResourceCounts = null,
                craftTime = 5f,
                recipeName = "불꽃비늘 제작"
            },
            new RecipeData
            {
                outputItem = ironIngotData,
                outputCount = 1,
                inputItems = null,
                inputCounts = null,
                inputResources = new ResourceType[] { ResourceType.Scrap },
                inputResourceCounts = new int[] { 2 },
                craftTime = 5f,
                recipeName = "철 주괴 제작"
            },
            new RecipeData
            {
                outputItem = steelData,
                outputCount = 1,
                inputItems = new CraftItemData[] { ironIngotData },
                inputCounts = new int[] { 5 },
                inputResources = null,
                inputResourceCounts = null,
                craftTime = 5f,
                recipeName = "강철 제작"
            },
            new RecipeData
            {
                outputItem = hellSteelData,
                outputCount = 1,
                inputItems = new CraftItemData[] { steelData, fireScaleData },
                inputCounts = new int[] { 10, 10 },
                inputResources = null,
                inputResourceCounts = null,
                craftTime = 5f,
                recipeName = "지옥철 제작"
            }
        };
        
        // UIManager에 레시피 선택 패널 표시 요청
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowRecipeSelection(facility, recipes);
        }
    }
    
    void ShowArachneRecipeSelection(GameObject facility)
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager가 없습니다!");
            return;
        }
        
        // 이 시설이 제작 중인지 확인
        if (IsFacilityCrafting(facility))
        {
            Debug.Log($"{facility.name}은(는) 이미 제작 중입니다!");
            return;
        }
        
        // 4개 레시피 준비
        RecipeData[] recipes = new RecipeData[]
        {
            new RecipeData
            {
                outputItem = threadData,
                outputCount = 1,
                inputItems = new CraftItemData[] { meatData },
                inputCounts = new int[] { 2 },
                inputResources = null,
                inputResourceCounts = null,
                craftTime = 5f,
                recipeName = "실 제작"
            },
            new RecipeData
            {
                outputItem = ropeData,
                outputCount = 1,
                inputItems = new CraftItemData[] { threadData },
                inputCounts = new int[] { 2 },
                inputResources = null,
                inputResourceCounts = null,
                craftTime = 5f,
                recipeName = "밧줄 제작"
            },
            new RecipeData
            {
                outputItem = clothData,
                outputCount = 1,
                inputItems = new CraftItemData[] { threadData },
                inputCounts = new int[] { 10 },
                inputResources = null,
                inputResourceCounts = null,
                craftTime = 5f,
                recipeName = "천 제작"
            },
            new RecipeData
            {
                outputItem = clothArmorData,
                outputCount = 1,
                inputItems = new CraftItemData[] { clothData },
                inputCounts = new int[] { 10 },
                inputResources = null,
                inputResourceCounts = null,
                craftTime = 5f,
                recipeName = "천옷 제작"
            }
        };
        
        // UIManager에 레시피 선택 패널 표시 요청
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowRecipeSelection(facility, recipes);
        }
    }
    
    void ShowDeathStatueRecipeSelection(GameObject facility)
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager가 없습니다!");
            return;
        }
        
        // 이 시설이 제작 중인지 확인
        if (IsFacilityCrafting(facility))
        {
            Debug.Log($"{facility.name}은(는) 이미 제작 중입니다!");
            return;
        }
        
        // 3개 레시피 준비
        RecipeData[] recipes = new RecipeData[]
        {
            new RecipeData
            {
                outputItem = soulData,
                outputCount = 1,
                inputItems = new CraftItemData[] { humanData },
                inputCounts = new int[] { 1 },
                inputResources = null,
                inputResourceCounts = null,
                craftTime = 5f,
                recipeName = "영혼 추출"
            },
            new RecipeData
            {
                outputItem = soulAshData,
                outputCount = 1,
                inputItems = new CraftItemData[] { soulData, ashData },
                inputCounts = new int[] { 1, 10 },
                inputResources = null,
                inputResourceCounts = null,
                craftTime = 5f,
                recipeName = "영혼재 제작"
            },
            new RecipeData
            {
                outputItem = soulSteelData,
                outputCount = 1,
                inputItems = new CraftItemData[] { soulData, hellSteelData },
                inputCounts = new int[] { 1, 1 },
                inputResources = null,
                inputResourceCounts = null,
                craftTime = 5f,
                recipeName = "영혼철 제작"
            }
        };
        
        // UIManager에 레시피 선택 패널 표시 요청
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowRecipeSelection(facility, recipes);
        }
    }
    
    void ShowDwarfWorkshopRecipeSelection(GameObject facility)
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager가 없습니다!");
            return;
        }
        
        // 이 시설이 제작 중인지 확인
        if (IsFacilityCrafting(facility))
        {
            Debug.Log($"{facility.name}은(는) 이미 제작 중입니다!");
            return;
        }
        
        // 5개 레시피 준비
        RecipeData[] recipes = new RecipeData[]
        {
            new RecipeData
            {
                outputItem = boneData,
                outputCount = 1,
                inputItems = new CraftItemData[] { meatData },
                inputCounts = new int[] { 2 },
                inputResources = null,
                inputResourceCounts = null,
                craftTime = 5f,
                recipeName = "뼈 제작"
            },
            new RecipeData
            {
                outputItem = steelSwordData,
                outputCount = 1,
                inputItems = new CraftItemData[] { steelData },
                inputCounts = new int[] { 5 },
                inputResources = null,
                inputResourceCounts = null,
                craftTime = 5f,
                recipeName = "강철검 제작"
            },
            new RecipeData
            {
                outputItem = steelArmorData,
                outputCount = 1,
                inputItems = new CraftItemData[] { steelData },
                inputCounts = new int[] { 5 },
                inputResources = null,
                inputResourceCounts = null,
                craftTime = 5f,
                recipeName = "강철갑옷 제작"
            },
            new RecipeData
            {
                outputItem = hellArmorData,
                outputCount = 1,
                inputItems = new CraftItemData[] { hellSteelData },
                inputCounts = new int[] { 5 },
                inputResources = null,
                inputResourceCounts = null,
                craftTime = 5f,
                recipeName = "지옥갑옷 제작"
            },
            new RecipeData
            {
                outputItem = hellSwordData,
                outputCount = 1,
                inputItems = new CraftItemData[] { hellSteelData },
                inputCounts = new int[] { 5 },
                inputResources = null,
                inputResourceCounts = null,
                craftTime = 5f,
                recipeName = "지옥검 제작"
            }
        };
        
        // UIManager에 레시피 선택 패널 표시 요청
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowRecipeSelection(facility, recipes);
        }
    }
    
    // 레시피 데이터 클래스
    public class RecipeData
    {
        public CraftItemData outputItem;
        public int outputCount;
        public CraftItemData[] inputItems;      // 아이템 재료
        public int[] inputCounts;
        public ResourceType[] inputResources;   // 자원 재료 (목재, 고철, 돌)
        public int[] inputResourceCounts;
        public float craftTime;
        public string recipeName;
    }
    
    void ShowCraftConfirm(GameObject facility, CraftItemData outputItem, 
                         CraftItemData[] inputItems, int[] inputCounts, float craftTime)
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowCraftConfirm(facility, outputItem, inputItems, inputCounts, null, null, craftTime);
        }
    }
    
    void ShowCraftConfirm(GameObject facility, CraftItemData outputItem, 
                         CraftItemData[] inputItems, int[] inputCounts,
                         ResourceType[] inputResources, int[] inputResourceCounts, float craftTime)
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowCraftConfirm(facility, outputItem, inputItems, inputCounts, inputResources, inputResourceCounts, craftTime);
        }
    }
    
    public void StartCrafting(GameObject facility, CraftItemData outputItem, 
                             CraftItemData[] inputItems, int[] inputCounts,
                             ResourceType[] inputResources, int[] inputResourceCounts,
                             float craftTime, int outputCount = 1)
    {
        // 이미 제작 중인지 확인
        if (IsFacilityCrafting(facility))
        {
            Debug.Log($"{facility.name}은(는) 이미 제작 중입니다!");
            return;
        }
        
        // 아이템 재료 확인
        if (inputItems != null && inputCounts != null)
        {
            if (!InventoryManager.Instance.HasItems(inputItems, inputCounts))
            {
                Debug.Log("아이템 재료가 부족합니다!");
                return;
            }
        }
        
        // 자원 재료 확인
        if (inputResources != null && inputResourceCounts != null)
        {
            for (int i = 0; i < inputResources.Length; i++)
            {
                int currentAmount = ResourceManager.Instance.GetResource(inputResources[i]);
                if (currentAmount < inputResourceCounts[i])
                {
                    Debug.Log($"자원 재료가 부족합니다! {inputResources[i]} (필요: {inputResourceCounts[i]}, 보유: {currentAmount})");
                    return;
                }
            }
        }
        
        // 제작 상태 생성
        CraftingState state = new CraftingState
        {
            isCrafting = true,
            currentTime = 0f,
            targetTime = craftTime,
            outputItem = outputItem,
            craftCoroutine = null
        };
        
        facilityStates[facility] = state;
        
        // 제작 코루틴 시작
        state.craftCoroutine = StartCoroutine(CraftingCoroutine(facility, inputItems, inputCounts, inputResources, inputResourceCounts, outputCount));
    }
    
    IEnumerator CraftingCoroutine(GameObject facility, CraftItemData[] inputItems, int[] inputCounts, 
                                  ResourceType[] inputResources, int[] inputResourceCounts, int outputCount)
    {
        CraftingState state = facilityStates[facility];
        
        Debug.Log($"{facility.name}에서 {state.outputItem.itemName} x{outputCount} 제작 시작! ({state.targetTime}초)");
        
        // 아이템 재료 소비
        if (inputItems != null && inputCounts != null)
        {
            InventoryManager.Instance.RemoveItems(inputItems, inputCounts);
        }
        
        // 자원 재료 소비
        if (inputResources != null && inputResourceCounts != null)
        {
            for (int i = 0; i < inputResources.Length; i++)
            {
                ResourceManager.Instance.AddResource(inputResources[i], -inputResourceCounts[i]);
                Debug.Log($"{inputResources[i]} {inputResourceCounts[i]}개 소비");
            }
        }
        
        // 제작 진행
        while (state.currentTime < state.targetTime)
        {
            state.currentTime += Time.deltaTime;
            yield return null;
        }
        
        // 제작 완료 - 인벤토리에 추가
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(state.outputItem, outputCount);
        }
        
        Debug.Log($"{facility.name}에서 {state.outputItem.itemName} x{outputCount} 제작 완료!");
        
        // 제작 상태 제거
        facilityStates.Remove(facility);
    }
    
    public bool IsFacilityCrafting(GameObject facility)
    {
        return facilityStates.ContainsKey(facility) && facilityStates[facility].isCrafting;
    }
    
    public float GetCraftProgress(GameObject facility)
    {
        if (!facilityStates.ContainsKey(facility))
            return 0f;
        
        CraftingState state = facilityStates[facility];
        return state.currentTime / state.targetTime;
    }
    
    // 전역 제작 여부 (호환성)
    public bool IsCrafting()
    {
        return facilityStates.Count > 0;
    }
    
    public float GetCraftProgress()
    {
        // 아무 시설이나 반환 (호환성)
        foreach (var state in facilityStates.Values)
        {
            return state.currentTime / state.targetTime;
        }
        return 0f;
    }
}