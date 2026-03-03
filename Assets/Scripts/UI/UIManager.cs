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
    [SerializeField] private Button[] unitButtons;
    [SerializeField] private Image[] unitButtonImages;
    [SerializeField] private TextMeshProUGUI[] unitCostTexts;
    
    [Header("Craft Confirm Panel")]
    [SerializeField] private GameObject craftConfirmPanel;
    [SerializeField] private Image craftItemImage;
    [SerializeField] private TextMeshProUGUI craftItemNameText;
    [SerializeField] private TextMeshProUGUI craftTimeText;
    [SerializeField] private Button craftYesButton;
    [SerializeField] private Button craftNoButton;
    
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
    [SerializeField] private UnitData[] unitDataArray;
    
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
        SetupRaidPanels();
        
        if (craftConfirmPanel != null)
            craftConfirmPanel.SetActive(false);
        
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
        
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
    
    void OnUnitButtonClicked(int unitIndex)
    {
        if (WallExpansionManager.Instance != null)
            WallExpansionManager.Instance.SetExpansionMode(false);
        
        UnitPlacementManager.Instance.SelectUnit(unitIndex);
    }
}