using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Resource Display")]
    [SerializeField] private TextMeshProUGUI resourceText;
    
    [Header("Unit Buttons")]
    [SerializeField] private Button[] unitButtons;
    [SerializeField] private Image[] unitButtonImages;
    [SerializeField] private TextMeshProUGUI[] unitCostTexts;
    
    [Header("Expand Button")]
    [SerializeField] private Button expandButton;
    
    [Header("Scout Button")]
    [SerializeField] private Button scoutButton;
    [SerializeField] private TextMeshProUGUI scoutButtonText;
    
    [Header("Scout Result Panel")]
    [SerializeField] private GameObject scoutResultPanel;
    [SerializeField] private TextMeshProUGUI scoutResultText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button scoutAgainButton;
    
    [Header("Craft Confirm Panel")]
    [SerializeField] private GameObject craftConfirmPanel;
    [SerializeField] private Image craftItemImage;
    [SerializeField] private TextMeshProUGUI craftItemNameText;
    [SerializeField] private TextMeshProUGUI craftTimeText;
    [SerializeField] private Button craftYesButton;
    [SerializeField] private Button craftNoButton;
    
    [Header("Inventory Panel")]
    [SerializeField] private GameObject inventoryPanel;
    
    [Header("Unit Data")]
    [SerializeField] private UnitData[] unitDataArray;
    
    // 제작할 아이템 정보 임시 저장
    private CraftItemData pendingCraftItem;
    private float pendingCraftTime;
    
    void Start()
    {
        ResourceManager.Instance.OnResourceChanged += UpdateResourceDisplay;
        UpdateResourceDisplay(ResourceManager.Instance.GetCurrentResources());
        SetupUnitButtons();
        SetupExpandButton();
        SetupScoutButton();
        SetupScoutResultPanel();
        SetupCraftConfirmPanel();
        
        // 알림 창 초기에는 숨김
        if (scoutResultPanel != null)
            scoutResultPanel.SetActive(false);
        
        if (craftConfirmPanel != null)
            craftConfirmPanel.SetActive(false);
        
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }
    
    void OnDestroy()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnResourceChanged -= UpdateResourceDisplay;
    }
    
    void SetupUnitButtons()
    {
        for (int i = 0; i < unitButtons.Length && i < unitDataArray.Length; i++)
        {
            int index = i;
            
            if (unitButtonImages[i] != null && unitDataArray[i].unitSprite != null)
                unitButtonImages[i].sprite = unitDataArray[i].unitSprite;
            
            if (unitCostTexts[i] != null)
                unitCostTexts[i].text = $"{unitDataArray[i].cost}";
            
            unitButtons[i].onClick.AddListener(() => OnUnitButtonClicked(index));
        }
    }
    
    void SetupExpandButton()
    {
        if (expandButton != null)
            expandButton.onClick.AddListener(OnExpandButtonClicked);
    }
    
    void SetupScoutButton()
    {
        if (scoutButton != null)
            scoutButton.onClick.AddListener(OnScoutButtonClicked);
    }
    
    void SetupScoutResultPanel()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseResultPanel);
        
        if (scoutAgainButton != null)
            scoutAgainButton.onClick.AddListener(OnScoutAgain);
    }
    
    void SetupCraftConfirmPanel()
    {
        if (craftYesButton != null)
            craftYesButton.onClick.AddListener(OnCraftYes);
        
        if (craftNoButton != null)
            craftNoButton.onClick.AddListener(OnCraftNo);
    }
    
    void Update()
    {
        UpdateButtonStates();
        UpdateScoutButton();
        HandleInventoryInput();
    }
    
    // I 키로 인벤토리 토글
    void HandleInventoryInput()
    {
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            }
        }
    }
    
    void UpdateButtonStates()
    {
        int currentResources = ResourceManager.Instance.GetCurrentResources();
        
        // 유닛 버튼 상태 업데이트
        for (int i = 0; i < unitButtons.Length && i < unitDataArray.Length; i++)
        {
            bool canAfford = currentResources >= unitDataArray[i].cost;
            unitButtons[i].interactable = canAfford;
            
            if (UnitPlacementManager.Instance.GetSelectedUnit() == unitDataArray[i])
                unitButtons[i].GetComponent<Image>().color = new Color(0.7f, 1f, 0.7f);
            else
                unitButtons[i].GetComponent<Image>().color = canAfford ? Color.white : new Color(0.5f, 0.5f, 0.5f);
        }
        
        // 확장 버튼 하이라이트
        if (expandButton != null)
        {
            if (WallExpansionManager.Instance != null && WallExpansionManager.Instance.IsExpansionMode())
                expandButton.GetComponent<Image>().color = new Color(1f, 0.7f, 0.3f);
            else
                expandButton.GetComponent<Image>().color = Color.white;
        }
    }
    
    void UpdateScoutButton()
    {
        if (scoutButton == null || scoutButtonText == null || ScoutManager.Instance == null)
            return;
        
        if (ScoutManager.Instance.IsScoutActive())
        {
            scoutButton.interactable = false;
            float progress = ScoutManager.Instance.GetScoutProgress();
            scoutButtonText.text = $"정찰 중... {(int)(progress * 100)}%";
        }
        else
        {
            scoutButton.interactable = true;
            scoutButtonText.text = "정찰";
        }
    }
    
    // 정찰 결과 알림 창 표시
    public void ShowScoutResult(int reward)
    {
        if (scoutResultPanel == null || scoutResultText == null)
            return;
        
        scoutResultText.text = $"정찰 완료!\n\n자원 +{reward}";
        scoutResultPanel.SetActive(true);
    }
    
    // 제작 확인 창 표시
    public void ShowCraftConfirm(CraftItemData item, float craftTime)
    {
        if (craftConfirmPanel == null)
            return;
        
        pendingCraftItem = item;
        pendingCraftTime = craftTime;
        
        if (craftItemImage != null)
            craftItemImage.sprite = item.itemSprite;
        
        if (craftItemNameText != null)
            craftItemNameText.text = item.itemName;
        
        if (craftTimeText != null)
            craftTimeText.text = $"제작 시간: {craftTime}초";
        
        craftConfirmPanel.SetActive(true);
    }
    
    void OnCraftYes()
    {
        // 제작 시작
        if (CraftingManager.Instance != null && pendingCraftItem != null)
        {
            CraftingManager.Instance.StartCrafting(pendingCraftItem, pendingCraftTime);
        }
        
        // 창 닫기
        if (craftConfirmPanel != null)
            craftConfirmPanel.SetActive(false);
        
        pendingCraftItem = null;
    }
    
    void OnCraftNo()
    {
        // 창 닫기
        if (craftConfirmPanel != null)
            craftConfirmPanel.SetActive(false);
        
        pendingCraftItem = null;
    }
    
    void OnCloseResultPanel()
    {
        if (scoutResultPanel != null)
            scoutResultPanel.SetActive(false);
    }
    
    void OnScoutAgain()
    {
        if (scoutResultPanel != null)
            scoutResultPanel.SetActive(false);
        
        if (ScoutManager.Instance != null)
            ScoutManager.Instance.StartScout();
    }
    
    void OnUnitButtonClicked(int unitIndex)
    {
        if (WallExpansionManager.Instance != null)
            WallExpansionManager.Instance.SetExpansionMode(false);
        
        UnitPlacementManager.Instance.SelectUnit(unitIndex);
    }
    
    void OnExpandButtonClicked()
    {
        if (UnitPlacementManager.Instance != null)
            UnitPlacementManager.Instance.CancelPlacement();
        
        if (WallExpansionManager.Instance != null)
            WallExpansionManager.Instance.SetExpansionMode(!WallExpansionManager.Instance.IsExpansionMode());
    }
    
    void OnScoutButtonClicked()
    {
        if (ScoutManager.Instance != null)
            ScoutManager.Instance.StartScout();
    }
    
    void UpdateResourceDisplay(int resources)
    {
        resourceText.text = $"자원: {resources}";
    }
}