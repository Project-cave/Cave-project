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
    
    [Header("Facility Buttons (3개)")]
    [SerializeField] private Button[] facilityButtons;
    [SerializeField] private Image[] facilityButtonImages;
    [SerializeField] private TextMeshProUGUI[] facilityCostTexts;
    
    [Header("Facility Data")]
    [SerializeField] private FacilityData[] facilityDataArray;
    
    [Header("Unit Selection Panel (소환진용)")]
    [SerializeField] private GameObject unitSelectionPanel;
    [SerializeField] private Transform unitSelectionContent;
    [SerializeField] private GameObject unitSelectionButtonPrefab;
    [SerializeField] private Button unitSelectionCloseButton;
    
    [Header("Recipe Selection Panel (독거북용)")]
    [SerializeField] private GameObject recipeSelectionPanel;
    [SerializeField] private Transform recipeSelectionContent;
    [SerializeField] private GameObject recipeSelectionButtonPrefab;
    [SerializeField] private Button recipeSelectionCloseButton;
    
    [Header("Craft Confirm Panel")]
    [SerializeField] private GameObject craftConfirmPanel;
    [SerializeField] private Image craftItemImage;
    [SerializeField] private TextMeshProUGUI craftItemNameText;
    [SerializeField] private TextMeshProUGUI craftMaterialsText;  // 재료 표시
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
    
    private CraftItemData pendingOutputItem;
    private CraftItemData[] pendingInputItems;
    private int[] pendingInputCounts;
    private ResourceType[] pendingInputResources;    // 자원 재료
    private int[] pendingInputResourceCounts;
    private float pendingCraftTime;
    private int pendingOutputCount;
    private GameObject pendingFacility;
    private RaidData selectedRaid;
    private int selectedVillageId;
    
    void Start()
    {
        ResourceManager.Instance.OnResourceChanged += UpdateResourceDisplay;
        UpdateResourceDisplay();
        SetupFacilityButtons();
        SetupCraftConfirmPanel();
        SetupUnitSelectionPanel();
        SetupRecipeSelectionPanel();
        SetupRaidPanels();
        
        if (craftConfirmPanel != null)
            craftConfirmPanel.SetActive(false);
        
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
        
        if (unitSelectionPanel != null)
            unitSelectionPanel.SetActive(false);
        
        if (recipeSelectionPanel != null)
            recipeSelectionPanel.SetActive(false);
        
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
    
    void SetupFacilityButtons()
    {
        for (int i = 0; i < facilityButtons.Length && i < facilityDataArray.Length; i++)
        {
            int index = i;
            
            if (facilityButtonImages[i] != null && facilityDataArray[i].facilitySprite != null)
                facilityButtonImages[i].sprite = facilityDataArray[i].facilitySprite;
            
            if (facilityCostTexts[i] != null)
            {
                string costText = "";
                if (facilityDataArray[i].woodCost > 0)
                    costText += $"목{facilityDataArray[i].woodCost} ";
                if (facilityDataArray[i].scrapCost > 0)
                    costText += $"고{facilityDataArray[i].scrapCost} ";
                if (facilityDataArray[i].stoneCost > 0)
                    costText += $"돌{facilityDataArray[i].stoneCost}";
                
                facilityCostTexts[i].text = costText.Trim();
            }
            
            facilityButtons[i].onClick.AddListener(() => OnFacilityButtonClicked(index));
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
    
    void SetupRecipeSelectionPanel()
    {
        if (recipeSelectionCloseButton != null)
            recipeSelectionCloseButton.onClick.AddListener(() => recipeSelectionPanel.SetActive(false));
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
        for (int i = 0; i < facilityButtons.Length && i < facilityDataArray.Length; i++)
        {
            int wood = ResourceManager.Instance.GetResource(ResourceType.Wood);
            int scrap = ResourceManager.Instance.GetResource(ResourceType.Scrap);
            int stone = ResourceManager.Instance.GetResource(ResourceType.Stone);
            
            bool canAfford = wood >= facilityDataArray[i].woodCost &&
                           scrap >= facilityDataArray[i].scrapCost &&
                           stone >= facilityDataArray[i].stoneCost;
            
            facilityButtons[i].interactable = canAfford;
            
            if (FacilityPlacementManager.Instance != null && 
                FacilityPlacementManager.Instance.GetSelectedFacility() == facilityDataArray[i])
                facilityButtons[i].GetComponent<Image>().color = new Color(0.7f, 1f, 0.7f);
            else
                facilityButtons[i].GetComponent<Image>().color = canAfford ? Color.white : new Color(0.5f, 0.5f, 0.5f);
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
    
    void OnFacilityButtonClicked(int index)
    {
        if (index < 0 || index >= facilityDataArray.Length)
            return;
        
        FacilityData facility = facilityDataArray[index];
        
        if (WallExpansionManager.Instance != null)
            WallExpansionManager.Instance.SetExpansionMode(false);
        
        if (facility.isSpawnFacility)
        {
            OpenUnitSelectionPanel();
        }
        else
        {
            if (FacilityPlacementManager.Instance != null)
            {
                FacilityPlacementManager.Instance.SelectFacility(index);
            }
        }
    }
    
    public void OpenUnitSelectionPanel()
    {
        if (unitSelectionPanel == null || unitSelectionContent == null || unitSelectionButtonPrefab == null)
            return;
        
        foreach (Transform child in unitSelectionContent)
        {
            Destroy(child.gameObject);
        }
        
        if (FacilityPlacementManager.Instance == null)
            return;
        
        UnitSo[] units = FacilityPlacementManager.Instance.GetAvailableSpawnUnits();
        
        foreach (var unit in units)
        {
            GameObject btnObj = Instantiate(unitSelectionButtonPrefab, unitSelectionContent);
            Button btn = btnObj.GetComponent<Button>();
            
            if (btn != null)
            {
                UnitSo unitData = unit;
                btn.onClick.AddListener(() => OnSpawnUnitSelected(unitData));
                
                TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                {
                    btnText.text = unit.unitName;
                }
            }
        }
        
        unitSelectionPanel.SetActive(true);
    }
    
    void OnSpawnUnitSelected(UnitSo unit)
    {
        if (FacilityPlacementManager.Instance != null)
        {
            FacilityPlacementManager.Instance.SelectSpawnUnit(unit);
        }
        
        if (unitSelectionPanel != null)
            unitSelectionPanel.SetActive(false);
        
        Debug.Log($"{unit.unitName} 소환진 배치 모드 시작!");
    }
    
    // 레시피 선택 패널 표시 (독거북용)
    public void ShowRecipeSelection(GameObject facility, CraftingManager.RecipeData[] recipes)
    {
        if (recipeSelectionPanel == null || recipeSelectionContent == null)
        {
            Debug.LogError("레시피 선택 패널이 설정되지 않았습니다!");
            return;
        }
        
        // 기존 버튼 삭제
        foreach (Transform child in recipeSelectionContent)
        {
            Destroy(child.gameObject);
        }
        
        // 각 레시피마다 버튼 생성
        foreach (var recipe in recipes)
        {
            GameObject btnObj;
            
            if (recipeSelectionButtonPrefab != null)
            {
                btnObj = Instantiate(recipeSelectionButtonPrefab, recipeSelectionContent);
            }
            else
            {
                // 프리팹 없으면 자동 생성
                btnObj = CreateRecipeButton(recipe);
            }
            
            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                CraftingManager.RecipeData recipeData = recipe;
                btn.onClick.AddListener(() => OnRecipeSelected(facility, recipeData));
                
                // 버튼 텍스트 설정
                TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                {
                    // 아이템 재료 체크
                    bool hasItems = true;
                    if (recipe.inputItems != null && recipe.inputCounts != null)
                    {
                        hasItems = InventoryManager.Instance.HasItems(recipe.inputItems, recipe.inputCounts);
                    }
                    
                    // 자원 재료 체크
                    bool hasResources = true;
                    if (recipe.inputResources != null && recipe.inputResourceCounts != null)
                    {
                        for (int i = 0; i < recipe.inputResources.Length; i++)
                        {
                            int current = ResourceManager.Instance.GetResource(recipe.inputResources[i]);
                            if (current < recipe.inputResourceCounts[i])
                            {
                                hasResources = false;
                                break;
                            }
                        }
                    }
                    
                    bool canCraft = hasItems && hasResources;
                    string color = canCraft ? "white" : "red";
                    
                    // 재료 텍스트 생성
                    string inputText = "";
                    
                    // 아이템 재료 표시
                    if (recipe.inputItems != null && recipe.inputCounts != null)
                    {
                        for (int i = 0; i < recipe.inputItems.Length; i++)
                        {
                            int currentCount = InventoryManager.Instance.GetItemCount(recipe.inputItems[i]);
                            inputText += $"{recipe.inputItems[i].itemName} {currentCount}/{recipe.inputCounts[i]}";
                            if (i < recipe.inputItems.Length - 1 || (recipe.inputResources != null && recipe.inputResources.Length > 0))
                                inputText += ", ";
                        }
                    }
                    
                    // 자원 재료 표시
                    if (recipe.inputResources != null && recipe.inputResourceCounts != null)
                    {
                        for (int i = 0; i < recipe.inputResources.Length; i++)
                        {
                            int currentCount = ResourceManager.Instance.GetResource(recipe.inputResources[i]);
                            string resName = recipe.inputResources[i] == ResourceType.Wood ? "목재" :
                                           recipe.inputResources[i] == ResourceType.Scrap ? "고철" : "돌";
                            inputText += $"{resName} {currentCount}/{recipe.inputResourceCounts[i]}";
                            if (i < recipe.inputResources.Length - 1)
                                inputText += ", ";
                        }
                    }
                    
                    btnText.text = $"<color={color}>{recipe.recipeName}\n{inputText}\n→ {recipe.outputItem.itemName} x{recipe.outputCount}</color>";
                }
            }
        }
        
        recipeSelectionPanel.SetActive(true);
    }
    
    GameObject CreateRecipeButton(CraftingManager.RecipeData recipe)
    {
        GameObject btnObj = new GameObject($"RecipeButton_{recipe.recipeName}");
        btnObj.transform.SetParent(recipeSelectionContent);
        btnObj.transform.localScale = Vector3.one;
        
        Button btn = btnObj.AddComponent<Button>();
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        
        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 80);
        
        // 텍스트 추가
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform);
        textObj.transform.localScale = Vector3.one;
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 16;
        
        RectTransform textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;
        
        return btnObj;
    }
    
    void OnRecipeSelected(GameObject facility, CraftingManager.RecipeData recipe)
    {
        // 레시피 선택 패널 닫기
        if (recipeSelectionPanel != null)
            recipeSelectionPanel.SetActive(false);
        
        // 제작 확인 창 열기 (자원 포함)
        ShowCraftConfirm(
            facility,
            recipe.outputItem,
            recipe.inputItems,
            recipe.inputCounts,
            recipe.inputResources,
            recipe.inputResourceCounts,
            recipe.craftTime,
            recipe.outputCount
        );
    }
    
    // 제작 확인 창 표시 (재료 정보 포함) - 시설 정보 + 자원 포함
    public void ShowCraftConfirm(GameObject facility, CraftItemData outputItem, 
                                 CraftItemData[] inputItems, int[] inputCounts,
                                 ResourceType[] inputResources, int[] inputResourceCounts,
                                 float craftTime, int outputCount = 1)
    {
        Debug.Log($"=== UIManager.ShowCraftConfirm 호출됨 (시설: {facility.name}) ===");
        Debug.Log($"출력 아이템: {outputItem.itemName} x{outputCount}");
        
        if (craftConfirmPanel == null)
        {
            Debug.LogError("craftConfirmPanel이 NULL입니다! Inspector에서 연결하세요!");
            return;
        }
        
        Debug.Log("craftConfirmPanel 있음");
        
        pendingFacility = facility;
        pendingOutputItem = outputItem;
        pendingInputItems = inputItems;
        pendingInputCounts = inputCounts;
        pendingInputResources = inputResources;
        pendingInputResourceCounts = inputResourceCounts;
        pendingCraftTime = craftTime;
        pendingOutputCount = outputCount;
        
        if (craftItemImage != null)
            craftItemImage.sprite = outputItem.itemSprite;
        else
            Debug.LogWarning("craftItemImage가 NULL");
        
        if (craftItemNameText != null)
            craftItemNameText.text = $"{outputItem.itemName} x{outputCount}";
        else
            Debug.LogWarning("craftItemNameText가 NULL");
        
        // 재료 표시 (아이템 + 자원)
        if (craftMaterialsText != null)
        {
            string materialsText = "필요 재료:\n";
            
            // 아이템 재료 표시
            if (inputItems != null && inputCounts != null)
            {
                for (int i = 0; i < inputItems.Length; i++)
                {
                    int currentCount = InventoryManager.Instance.GetItemCount(inputItems[i]);
                    string color = currentCount >= inputCounts[i] ? "green" : "red";
                    materialsText += $"<color={color}>{inputItems[i].itemName} {currentCount}/{inputCounts[i]}</color>\n";
                }
            }
            
            // 자원 재료 표시
            if (inputResources != null && inputResourceCounts != null)
            {
                for (int i = 0; i < inputResources.Length; i++)
                {
                    int currentCount = ResourceManager.Instance.GetResource(inputResources[i]);
                    string color = currentCount >= inputResourceCounts[i] ? "green" : "red";
                    string resName = inputResources[i] == ResourceType.Wood ? "목재" :
                                   inputResources[i] == ResourceType.Scrap ? "고철" : "돌";
                    materialsText += $"<color={color}>{resName} {currentCount}/{inputResourceCounts[i]}</color>\n";
                }
            }
            
            craftMaterialsText.text = materialsText;
            Debug.Log($"재료 텍스트 설정: {materialsText}");
        }
        else
        {
            Debug.LogWarning("craftMaterialsText가 NULL");
        }
        
        if (craftTimeText != null)
            craftTimeText.text = $"제작 시간: {craftTime}초";
        else
            Debug.LogWarning("craftTimeText가 NULL");
        
        Debug.Log("패널 활성화!");
        craftConfirmPanel.SetActive(true);
    }
    
    // 기존 호환성을 위한 오버로드 (자원 없는 경우)
    public void ShowCraftConfirm(GameObject facility, CraftItemData outputItem, 
                                 CraftItemData[] inputItems, int[] inputCounts, 
                                 float craftTime, int outputCount = 1)
    {
        ShowCraftConfirm(facility, outputItem, inputItems, inputCounts, null, null, craftTime, outputCount);
    }
    
    // 기존 호환성을 위한 오버로드
    public void ShowCraftConfirm(CraftItemData outputItem, CraftItemData[] inputItems, int[] inputCounts, float craftTime, int outputCount = 1)
    {
        Debug.Log("=== UIManager.ShowCraftConfirm 호출됨 ===");
        Debug.Log($"출력 아이템: {outputItem.itemName}");
        
        if (craftConfirmPanel == null)
        {
            Debug.LogError("craftConfirmPanel이 NULL입니다! Inspector에서 연결하세요!");
            return;
        }
        
        Debug.Log("craftConfirmPanel 있음");
        
        pendingOutputItem = outputItem;
        pendingInputItems = inputItems;
        pendingInputCounts = inputCounts;
        pendingCraftTime = craftTime;
        
        if (craftItemImage != null)
            craftItemImage.sprite = outputItem.itemSprite;
        else
            Debug.LogWarning("craftItemImage가 NULL");
        
        if (craftItemNameText != null)
            craftItemNameText.text = outputItem.itemName;
        else
            Debug.LogWarning("craftItemNameText가 NULL");
        
        // 재료 표시
        if (craftMaterialsText != null)
        {
            string materialsText = "필요 재료:\n";
            for (int i = 0; i < inputItems.Length; i++)
            {
                int currentCount = InventoryManager.Instance.GetItemCount(inputItems[i]);
                string color = currentCount >= inputCounts[i] ? "green" : "red";
                materialsText += $"<color={color}>{inputItems[i].itemName} {currentCount}/{inputCounts[i]}</color>\n";
            }
            craftMaterialsText.text = materialsText;
            Debug.Log($"재료 텍스트 설정: {materialsText}");
        }
        else
        {
            Debug.LogWarning("craftMaterialsText가 NULL");
        }
        
        if (craftTimeText != null)
            craftTimeText.text = $"제작 시간: {craftTime}초";
        else
            Debug.LogWarning("craftTimeText가 NULL");
        
        Debug.Log("패널 활성화!");
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
        if (CraftingManager.Instance != null && pendingOutputItem != null && pendingFacility != null)
        {
            CraftingManager.Instance.StartCrafting(
                pendingFacility, 
                pendingOutputItem, 
                pendingInputItems, 
                pendingInputCounts,
                pendingInputResources,
                pendingInputResourceCounts,
                pendingCraftTime, 
                pendingOutputCount
            );
        }
        
        if (craftConfirmPanel != null)
            craftConfirmPanel.SetActive(false);
        
        pendingFacility = null;
        pendingOutputItem = null;
        pendingInputItems = null;
        pendingInputCounts = null;
        pendingInputResources = null;
        pendingInputResourceCounts = null;
        pendingOutputCount = 1;
    }
    
    void OnCraftNo()
    {
        if (craftConfirmPanel != null)
            craftConfirmPanel.SetActive(false);
        
        pendingFacility = null;
        pendingOutputItem = null;
        pendingInputItems = null;
        pendingInputCounts = null;
        pendingInputResources = null;
        pendingInputResourceCounts = null;
        pendingOutputCount = 1;
    }
}