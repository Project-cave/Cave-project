using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [Header("Resource Display")]
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI scrapText;
    [SerializeField] private TextMeshProUGUI stoneText;
    
    [Header("Unit Buttons")]
    [SerializeField] private Button[] unitButtons;  // 4개 버튼 (독통, 보라색알, 가시함정, 생산포탈)
    [SerializeField] private Image[] unitButtonImages;
    [SerializeField] private TextMeshProUGUI[] unitCostTexts;
    
    [Header("Special Unit Data")]
    [SerializeField] private UnitData spawnPortalUnitData;  // 생산 포탈 UnitData
    
    [Header("Craft Confirm Panel")]
    [SerializeField] private GameObject craftConfirmPanel;
    [SerializeField] private Image craftItemImage;
    [SerializeField] private TextMeshProUGUI craftItemNameText;
    [SerializeField] private TextMeshProUGUI craftTimeText;
    [SerializeField] private Button craftYesButton;
    [SerializeField] private Button craftNoButton;
    
    [Header("Unit Selection Panel (생산 시설)")]
    [SerializeField] private GameObject unitSelectionPanel;
    [SerializeField] private Transform unitSelectionContent;
    [SerializeField] private GameObject unitSelectionButtonPrefab;
    [SerializeField] private Button unitSelectionCloseButton;
    
    [Header("Raid Village Panel (1단계: 마을 선택)")]
    [SerializeField] private GameObject raidVillagePanel;
    [SerializeField] private Button village1Button;
    [SerializeField] private Button village2Button;
    [SerializeField] private Button village3Button;
    [SerializeField] private Button raidVillageCloseButton;
    
    [Header("Raid Difficulty Panel (2단계: 난이도 선택)")]
    [SerializeField] private GameObject raidDifficultyPanel;
    [SerializeField] private TextMeshProUGUI selectedVillageText;
    [SerializeField] private Transform difficultyContent;
    [SerializeField] private GameObject difficultyButtonPrefab;
    [SerializeField] private Button raidDifficultyBackButton;
    
    [Header("Raid Confirm Panel (3단계: 확인)")]
    [SerializeField] private GameObject raidConfirmPanel;
    [SerializeField] private TextMeshProUGUI raidLocationNameText;
    [SerializeField] private TextMeshProUGUI raidSuccessRateText;
    [SerializeField] private TextMeshProUGUI raidTimeText;
    [SerializeField] private Button raidStartButton;
    [SerializeField] private Button raidCancelButton;
    
    [Header("Raid Result Panel")]
    [SerializeField] private GameObject raidResultPanel;
    [SerializeField] private TextMeshProUGUI raidResultText;
    [SerializeField] private Button raidResultCloseButton;
    
    [Header("Inventory Panel")]
    [SerializeField] private GameObject inventoryPanel;
    
    [Header("Unit Data")]
    [SerializeField] private UnitData[] unitDataArray;  // 4개 (독통, 보라색알, 가시함정, 생산포탈)
    
    private CraftItemData pendingCraftItem;
    private float pendingCraftTime;
    private RaidData selectedRaid;
    private int selectedVillageId;
    
    void Start()
    {
        ResourceManager.Instance.OnResourceChanged += UpdateResourceDisplay;
        UpdateResourceDisplay();
        SetupUnitButtons();
        SetupCraftConfirmPanel();
        SetupUnitSelectionPanel();
        SetupRaidPanels();
        
        if (craftConfirmPanel != null)
            craftConfirmPanel.SetActive(false);
        
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
        
        if (unitSelectionPanel != null)
            unitSelectionPanel.SetActive(false);
        
        if (raidVillagePanel != null)
            raidVillagePanel.SetActive(false);
        
        if (raidDifficultyPanel != null)
            raidDifficultyPanel.SetActive(false);
        
        if (raidConfirmPanel != null)
            raidConfirmPanel.SetActive(false);
        
        if (raidResultPanel != null)
            raidResultPanel.SetActive(false);
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
    
    void SetupCraftConfirmPanel()
    {
        if (craftYesButton != null)
            craftYesButton.onClick.AddListener(OnCraftYes);
        
        if (craftNoButton != null)
            craftNoButton.onClick.AddListener(OnCraftNo);
    }
    
    void SetupUnitSelectionPanel()
    {
        if (unitSelectionCloseButton != null)
            unitSelectionCloseButton.onClick.AddListener(() => unitSelectionPanel.SetActive(false));
    }
    
    void SetupRaidPanels()
    {
        if (village1Button != null)
            village1Button.onClick.AddListener(() => OnVillageSelected(1));
        
        if (village2Button != null)
            village2Button.onClick.AddListener(() => OnVillageSelected(2));
        
        if (village3Button != null)
            village3Button.onClick.AddListener(() => OnVillageSelected(3));
        
        if (raidVillageCloseButton != null)
            raidVillageCloseButton.onClick.AddListener(() => raidVillagePanel.SetActive(false));
        
        if (raidDifficultyBackButton != null)
            raidDifficultyBackButton.onClick.AddListener(OnDifficultyBack);
        
        if (raidStartButton != null)
            raidStartButton.onClick.AddListener(OnRaidStart);
        
        if (raidCancelButton != null)
            raidCancelButton.onClick.AddListener(() => raidConfirmPanel.SetActive(false));
        
        if (raidResultCloseButton != null)
            raidResultCloseButton.onClick.AddListener(() => raidResultPanel.SetActive(false));
    }
    
    void Update()
    {
        UpdateButtonStates();
        HandleInventoryInput();
    }
    
    void HandleInventoryInput()
    {
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            if (inventoryPanel != null)
                inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }
    
    void UpdateButtonStates()
    {
        if (UnitPlacementManager.Instance == null) return;

        for (int i = 0; i < unitButtons.Length && i < unitDataArray.Length; i++)
        {
            int wood = ResourceManager.Instance.GetResource(ResourceType.Wood);
            bool canAfford = wood >= unitDataArray[i].cost;
            unitButtons[i].interactable = canAfford;
            
            if (UnitPlacementManager.Instance.GetSelectedUnit() == unitDataArray[i])
                unitButtons[i].GetComponent<Image>().color = new Color(0.7f, 1f, 0.7f);
            else
                unitButtons[i].GetComponent<Image>().color = canAfford ? Color.white : new Color(0.5f, 0.5f, 0.5f);
        }
    }
    
    void UpdateResourceDisplay()
    {
        if (woodText != null)
            woodText.text = $"나무: {ResourceManager.Instance.GetResource(ResourceType.Wood)}";
        
        if (scrapText != null)
            scrapText.text = $"고철: {ResourceManager.Instance.GetResource(ResourceType.Scrap)}";
        
        if (stoneText != null)
            stoneText.text = $"돌: {ResourceManager.Instance.GetResource(ResourceType.Stone)}";
    }
    
    void OnUnitButtonClicked(int unitIndex)
    {
        if (unitIndex < 0 || unitIndex >= unitDataArray.Length)
            return;
        
        // 확장 모드 끄기
        if (WallExpansionManager.Instance != null)
            WallExpansionManager.Instance.SetExpansionMode(false);
        
        // 생산 포탈 유닛인지 확인
        if (spawnPortalUnitData != null && unitDataArray[unitIndex] == spawnPortalUnitData)
        {
            // 생산 포탈 → 유닛 선택 패널 열기
            OpenUnitSelectionPanel();
        }
        else
        {
            // 일반 유닛 → 기존 배치 모드
            UnitPlacementManager.Instance.SelectUnit(unitIndex);
        }
    }
    
    // 유닛 선택 패널 열기
    public void OpenUnitSelectionPanel()
    {
        if (unitSelectionPanel == null || unitSelectionContent == null || unitSelectionButtonPrefab == null)
            return;
        
        // 기존 버튼 삭제
        foreach (Transform child in unitSelectionContent)
        {
            Destroy(child.gameObject);
        }
        
        // 12개 유닛 버튼 생성
        if (SpawnFacilityManager.Instance == null)
            return;
        
        UnitSo[] units = SpawnFacilityManager.Instance.GetAvailableUnits();
        
        foreach (var unit in units)
        {
            GameObject btnObj = Instantiate(unitSelectionButtonPrefab, unitSelectionContent);
            Button btn = btnObj.GetComponent<Button>();
            
            if (btn != null)
            {
                UnitSo unitData = unit;
                btn.onClick.AddListener(() => OnUnitSelected(unitData));
                
                // 버튼 텍스트 설정
                TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                {
                    btnText.text = unit.info.unitName;
                }
            }
        }
        
        unitSelectionPanel.SetActive(true);
    }
    
    void OnUnitSelected(UnitSo unit)
    {
        // 시설 배치 모드 시작
        if (FacilityPlacementController.Instance != null)
        {
            FacilityPlacementController.Instance.StartPlacement(unit);
        }
        
        // 패널 닫기
        if (unitSelectionPanel != null)
            unitSelectionPanel.SetActive(false);
        
        Debug.Log($"{unit.info.unitName} 선택! 맵에서 보라색 타일을 클릭하세요.");
    }
    
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
    
    public void OpenRaidMenu()
    {
        if (raidVillagePanel != null)
            raidVillagePanel.SetActive(true);
    }
    
    void OnVillageSelected(int villageId)
    {
        selectedVillageId = villageId;
        raidVillagePanel.SetActive(false);
        ShowDifficultyPanel(villageId);
    }
    
    void ShowDifficultyPanel(int villageId)
    {
        if (raidDifficultyPanel == null || difficultyContent == null || difficultyButtonPrefab == null)
            return;
        
        if (selectedVillageText != null)
            selectedVillageText.text = $"마을 {villageId}";
        
        foreach (Transform child in difficultyContent)
        {
            Destroy(child.gameObject);
        }
        
        RaidData[] allRaids = RaidManager.Instance.GetRaidLocations();
        RaidData[] villageRaids = allRaids.Where(r => r.villageId == villageId).ToArray();
        
        foreach (var raid in villageRaids)
        {
            GameObject btnObj = Instantiate(difficultyButtonPrefab, difficultyContent);
            Button btn = btnObj.GetComponent<Button>();
            
            if (btn != null)
            {
                RaidData raidData = raid;
                btn.onClick.AddListener(() => OnDifficultySelected(raidData));
                
                TextMeshProUGUI[] texts = btnObj.GetComponentsInChildren<TextMeshProUGUI>();
                if (texts.Length >= 3)
                {
                    texts[0].text = raid.GetDifficultyName();
                    texts[1].text = GetRewardText(raid);
                    texts[2].text = $"성공률: {raid.baseSuccessRate}%";
                }
            }
        }
        
        raidDifficultyPanel.SetActive(true);
    }
    
    string GetRewardText(RaidData raid)
    {
        string text = "";
        foreach (var reward in raid.possibleRewards)
        {
            string resName = reward.resourceType == ResourceType.Wood ? "나무" :
                           reward.resourceType == ResourceType.Scrap ? "고철" : "돌";
            text += $"{resName} {reward.minAmount}~{reward.maxAmount}\n";
        }
        return text.TrimEnd('\n');
    }
    
    void OnDifficultyBack()
    {
        raidDifficultyPanel.SetActive(false);
        raidVillagePanel.SetActive(true);
    }
    
    void OnDifficultySelected(RaidData raid)
    {
        selectedRaid = raid;
        raidDifficultyPanel.SetActive(false);
        ShowRaidConfirmPanel(raid);
    }
    
    void ShowRaidConfirmPanel(RaidData raid)
    {
        if (raidConfirmPanel == null)
            return;
        
        if (raidLocationNameText != null)
            raidLocationNameText.text = $"{raid.GetLocationName()} - {raid.GetDifficultyName()}";
        
        if (raidSuccessRateText != null)
            raidSuccessRateText.text = $"성공률: {raid.baseSuccessRate}%";
        
        if (raidTimeText != null)
            raidTimeText.text = $"획득 시간: {raid.raidDuration}초";
        
        raidConfirmPanel.SetActive(true);
    }
    
    void OnRaidStart()
    {
        if (selectedRaid != null && RaidManager.Instance != null)
        {
            RaidManager.Instance.StartRaid(selectedRaid);
            raidConfirmPanel.SetActive(false);
        }
    }
    
    public void ShowRaidResult(bool success, RaidData raid)
    {
        if (raidResultPanel == null || raidResultText == null)
            return;
        
        if (success)
        {
            raidResultText.text = $"{raid.GetLocationName()} 약탈 성공!\n\n자원 획득";
        }
        else
        {
            raidResultText.text = $"{raid.GetLocationName()} 약탈 실패!";
        }
        
        raidResultPanel.SetActive(true);
    }
    
    void OnCraftYes()
    {
        if (CraftingManager.Instance != null && pendingCraftItem != null)
        {
            CraftingManager.Instance.StartCrafting(pendingCraftItem, pendingCraftTime);
        }
        
        if (craftConfirmPanel != null)
            craftConfirmPanel.SetActive(false);
        
        pendingCraftItem = null;
    }
    
    void OnCraftNo()
    {
        if (craftConfirmPanel != null)
            craftConfirmPanel.SetActive(false);
        
        pendingCraftItem = null;
    }
}