using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance { get; private set; }
    
    [Header("Item Data")]
    [SerializeField] private CraftItemData poisonPotionData;
    [SerializeField] private CraftItemData[] monsterDataArray;  // 4개 몬스터
    
    [Header("Crafting Settings")]
    [SerializeField] private float poisonCraftTime = 5f;
    [SerializeField] private float monsterCraftTime = 10f;
    
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    
    // 제작 중인지 여부
    private bool isCrafting = false;
    private float currentCraftTime = 0f;
    private float targetCraftTime = 0f;
    
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
        // 좌클릭으로 유닛 클릭 (단, 배치/확장 모드가 아닐 때만)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // 배치 모드나 확장 모드가 아닐 때만 유닛 클릭
            bool isPlacementMode = UnitPlacementManager.Instance != null && UnitPlacementManager.Instance.IsPlacementMode();
            bool isExpansionMode = WallExpansionManager.Instance != null && WallExpansionManager.Instance.IsExpansionMode();
            
            if (!isPlacementMode && !isExpansionMode)
            {
                HandleUnitClick();
            }
        }
    }
    
    void HandleUnitClick()
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
            
            // 유닛 이름으로 구분
            if (clickedObject.name.Contains("독통"))
            {
                ShowCraftConfirm(poisonPotionData, poisonCraftTime);
            }
            else if (clickedObject.name.Contains("보라색알"))
            {
                // 랜덤 몬스터 선택
                CraftItemData randomMonster = monsterDataArray[Random.Range(0, monsterDataArray.Length)];
                ShowCraftConfirm(randomMonster, monsterCraftTime);
            }
        }
    }
    
    void ShowCraftConfirm(CraftItemData item, float craftTime)
    {
        if (isCrafting)
        {
            Debug.Log("이미 제작 중입니다!");
            return;
        }
        
        // UI에 제작 확인 창 표시
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowCraftConfirm(item, craftTime);
        }
    }
    
    public void StartCrafting(CraftItemData item, float craftTime)
    {
        if (isCrafting) return;
        
        StartCoroutine(CraftingCoroutine(item, craftTime));
    }
    
    IEnumerator CraftingCoroutine(CraftItemData item, float craftTime)
    {
        isCrafting = true;
        currentCraftTime = 0f;
        targetCraftTime = craftTime;
        
        Debug.Log($"{item.itemName} 제작 시작! ({craftTime}초)");
        
        while (currentCraftTime < targetCraftTime)
        {
            currentCraftTime += Time.deltaTime;
            yield return null;
        }
        
        // 제작 완료 - 인벤토리에 추가
        InventoryManager inventory = FindObjectOfType<InventoryManager>();
        if (inventory != null)
        {
            inventory.AddItem(item);
        }
        
        Debug.Log($"{item.itemName} 제작 완료!");
        
        isCrafting = false;
        currentCraftTime = 0f;
    }
    
    public bool IsCrafting()
    {
        return isCrafting;
    }
    
    public float GetCraftProgress()
    {
        if (!isCrafting) return 0f;
        return currentCraftTime / targetCraftTime;
    }
}
