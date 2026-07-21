using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Resource Display")]
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI scrapText;
    [SerializeField] private TextMeshProUGUI meatText;

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
    [SerializeField] private TextMeshProUGUI craftMaterialsText;
    [SerializeField] private TextMeshProUGUI craftTimeText;
    [SerializeField] private Button craftYesButton;
    [SerializeField] private Button craftNoButton;

    [Header("Inventory Panel")]
    [SerializeField] private GameObject inventoryPanel;

    // 제작 확인창 임시 저장
    private CraftItemData pendingOutputItem;
    private CraftItemData[] pendingInputItems;
    private int[] pendingInputCounts;
    private ResourceType[] pendingInputResources;
    private int[] pendingInputResourceCounts;
    private float pendingCraftTime;
    private int pendingOutputCount;
    private GameObject pendingFacility;

    void Start()
    {
        ResourceManager.Instance.OnResourceChanged += UpdateResourceDisplay;
        UpdateResourceDisplay();
        SetupFacilityButtons();
        SetupCraftConfirmPanel();
        SetupUnitSelectionPanel();
        SetupRecipeSelectionPanel();

        if (craftConfirmPanel != null)   craftConfirmPanel.SetActive(false);
        if (inventoryPanel != null)      inventoryPanel.SetActive(false);
        if (unitSelectionPanel != null)  unitSelectionPanel.SetActive(false);
        if (recipeSelectionPanel != null) recipeSelectionPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnResourceChanged -= UpdateResourceDisplay;
    }

    // ── 시설 버튼 ────────────────────────────────────────────────────

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
                if (facilityDataArray[i].woodCost  > 0) costText += $"목{facilityDataArray[i].woodCost} ";
                if (facilityDataArray[i].scrapCost > 0) costText += $"고{facilityDataArray[i].scrapCost} ";
                if (facilityDataArray[i].meatCost > 0) costText += $"고기{facilityDataArray[i].meatCost}";
                facilityCostTexts[i].text = costText.Trim();
            }

            facilityButtons[i].onClick.AddListener(() => OnFacilityButtonClicked(index));
        }
    }

    void OnFacilityButtonClicked(int index)
    {
        if (index < 0 || index >= facilityDataArray.Length) return;

        FacilityData facility = facilityDataArray[index];

        if (WallExpansionManager.Instance != null)
            WallExpansionManager.Instance.SetExpansionMode(false);

        if (facility.isSpawnFacility)
            OpenUnitSelectionPanel();
        else if (FacilityPlacementManager.Instance != null)
            FacilityPlacementManager.Instance.SelectFacility(index);
    }

    // ── 소환진 유닛 선택 패널 ────────────────────────────────────────

    void SetupUnitSelectionPanel()
    {
        if (unitSelectionCloseButton != null)
            unitSelectionCloseButton.onClick.AddListener(() => unitSelectionPanel.SetActive(false));
    }

    public void OpenUnitSelectionPanel()
    {
        if (unitSelectionPanel == null || unitSelectionContent == null || unitSelectionButtonPrefab == null) return;
        if (FacilityPlacementManager.Instance == null) return;

        foreach (Transform child in unitSelectionContent)
            Destroy(child.gameObject);

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
                if (btnText != null) btnText.text = unit.info.unitName;
            }
        }

        unitSelectionPanel.SetActive(true);
    }

    void OnSpawnUnitSelected(UnitSo unit)
    {
        FacilityPlacementManager.Instance?.SelectSpawnUnit(unit);
        if (unitSelectionPanel != null) unitSelectionPanel.SetActive(false);
        Debug.Log($"{unit.info.unitName} 소환진 배치 모드 시작!");
    }

    // ── 레시피 선택 패널 ─────────────────────────────────────────────

    void SetupRecipeSelectionPanel()
    {
        if (recipeSelectionCloseButton != null)
            recipeSelectionCloseButton.onClick.AddListener(() => recipeSelectionPanel.SetActive(false));
    }

    public void ShowRecipeSelection(GameObject facility, CraftingManager.RecipeData[] recipes)
    {
        if (recipeSelectionPanel == null || recipeSelectionContent == null)
        {
            Debug.LogError("레시피 선택 패널이 설정되지 않았습니다!");
            return;
        }

        foreach (Transform child in recipeSelectionContent)
            Destroy(child.gameObject);

        foreach (var recipe in recipes)
        {
            GameObject btnObj = recipeSelectionButtonPrefab != null
                ? Instantiate(recipeSelectionButtonPrefab, recipeSelectionContent)
                : CreateRecipeButton(recipe);

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                CraftingManager.RecipeData recipeData = recipe;
                btn.onClick.AddListener(() => OnRecipeSelected(facility, recipeData));

                TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                {
                    bool hasItems = recipe.inputItems == null ||
                                    InventoryManager.Instance.HasItems(recipe.inputItems, recipe.inputCounts);

                    bool hasResources = true;
                    if (recipe.inputResources != null)
                    {
                        for (int i = 0; i < recipe.inputResources.Length; i++)
                        {
                            if (ResourceManager.Instance.GetResource(recipe.inputResources[i]) < recipe.inputResourceCounts[i])
                            {
                                hasResources = false;
                                break;
                            }
                        }
                    }

                    bool canCraft = hasItems && hasResources;
                    string color = canCraft ? "white" : "red";
                    string inputText = "";

                    if (recipe.inputItems != null)
                    {
                        for (int i = 0; i < recipe.inputItems.Length; i++)
                        {
                            int cur = InventoryManager.Instance.GetItemCount(recipe.inputItems[i]);
                            inputText += $"{recipe.inputItems[i].itemName} {cur}/{recipe.inputCounts[i]}";
                            if (i < recipe.inputItems.Length - 1 || (recipe.inputResources != null && recipe.inputResources.Length > 0))
                                inputText += ", ";
                        }
                    }

                    if (recipe.inputResources != null)
                    {
                        for (int i = 0; i < recipe.inputResources.Length; i++)
                        {
                            int cur = ResourceManager.Instance.GetResource(recipe.inputResources[i]);
                            string resName = recipe.inputResources[i] == ResourceType.Wood  ? "목재" :
                                             recipe.inputResources[i] == ResourceType.Scrap ? "고철" : "고기기";
                            inputText += $"{resName} {cur}/{recipe.inputResourceCounts[i]}";
                            if (i < recipe.inputResources.Length - 1) inputText += ", ";
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

        btnObj.AddComponent<Button>();
        btnObj.AddComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        btnObj.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 80);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform);
        textObj.transform.localScale = Vector3.one;

        var text = textObj.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 16;

        var textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;

        return btnObj;
    }

    void OnRecipeSelected(GameObject facility, CraftingManager.RecipeData recipe)
    {
        if (recipeSelectionPanel != null) recipeSelectionPanel.SetActive(false);

        ShowCraftConfirm(facility, recipe.outputItem, recipe.inputItems, recipe.inputCounts,
                         recipe.inputResources, recipe.inputResourceCounts,
                         recipe.craftTime, recipe.outputCount);
    }

    // ── 제작 확인창 ──────────────────────────────────────────────────

    void SetupCraftConfirmPanel()
    {
        if (craftYesButton != null) craftYesButton.onClick.AddListener(OnCraftYes);
        if (craftNoButton  != null) craftNoButton.onClick.AddListener(OnCraftNo);
    }

    public void ShowCraftConfirm(GameObject facility, CraftItemData outputItem,
                                 CraftItemData[] inputItems, int[] inputCounts,
                                 ResourceType[] inputResources, int[] inputResourceCounts,
                                 float craftTime, int outputCount = 1)
    {
        if (craftConfirmPanel == null) { Debug.LogError("craftConfirmPanel이 NULL!"); return; }

        pendingFacility              = facility;
        pendingOutputItem            = outputItem;
        pendingInputItems            = inputItems;
        pendingInputCounts           = inputCounts;
        pendingInputResources        = inputResources;
        pendingInputResourceCounts   = inputResourceCounts;
        pendingCraftTime             = craftTime;
        pendingOutputCount           = outputCount;

        if (craftItemImage    != null) craftItemImage.sprite      = outputItem.itemSprite;
        if (craftItemNameText != null) craftItemNameText.text     = $"{outputItem.itemName} x{outputCount}";
        if (craftTimeText     != null) craftTimeText.text         = $"제작 시간: {craftTime}초";

        if (craftMaterialsText != null)
        {
            string mat = "필요 재료:\n";
            if (inputItems != null)
            {
                for (int i = 0; i < inputItems.Length; i++)
                {
                    int cur = InventoryManager.Instance.GetItemCount(inputItems[i]);
                    string c = cur >= inputCounts[i] ? "green" : "red";
                    mat += $"<color={c}>{inputItems[i].itemName} {cur}/{inputCounts[i]}</color>\n";
                }
            }
            if (inputResources != null)
            {
                for (int i = 0; i < inputResources.Length; i++)
                {
                    int cur = ResourceManager.Instance.GetResource(inputResources[i]);
                    string c = cur >= inputResourceCounts[i] ? "green" : "red";
                    string resName = inputResources[i] == ResourceType.Wood  ? "목재" :
                                     inputResources[i] == ResourceType.Scrap ? "고철" : "고기";
                    mat += $"<color={c}>{resName} {cur}/{inputResourceCounts[i]}</color>\n";
                }
            }
            craftMaterialsText.text = mat;
        }

        craftConfirmPanel.SetActive(true);
    }

    // 오버로드 (자원 없는 경우)
    public void ShowCraftConfirm(GameObject facility, CraftItemData outputItem,
                                 CraftItemData[] inputItems, int[] inputCounts,
                                 float craftTime, int outputCount = 1)
    {
        ShowCraftConfirm(facility, outputItem, inputItems, inputCounts, null, null, craftTime, outputCount);
    }

    void OnCraftYes()
    {
        if (CraftingManager.Instance != null && pendingOutputItem != null && pendingFacility != null)
        {
            CraftingManager.Instance.StartCrafting(
                pendingFacility, pendingOutputItem,
                pendingInputItems, pendingInputCounts,
                pendingInputResources, pendingInputResourceCounts,
                pendingCraftTime, pendingOutputCount);
        }
        ClearPending();
    }

    void OnCraftNo() => ClearPending();

    void ClearPending()
    {
        if (craftConfirmPanel != null) craftConfirmPanel.SetActive(false);
        pendingFacility            = null;
        pendingOutputItem          = null;
        pendingInputItems          = null;
        pendingInputCounts         = null;
        pendingInputResources      = null;
        pendingInputResourceCounts = null;
        pendingOutputCount         = 1;
    }

    // ── 약탈 메뉴 (RaidMapUI로 위임) ─────────────────────────────────

    public void OpenRaidMenu()
    {
        if (RaidMapUI.Instance != null)
            RaidMapUI.Instance.OpenMapPanel();
    }

    // ── 자원 표시 / 버튼 상태 ────────────────────────────────────────

    void UpdateResourceDisplay()
    {
        if (woodText  != null) woodText.text  = $": {ResourceManager.Instance.GetResource(ResourceType.Wood)}";
        if (scrapText != null) scrapText.text = $": {ResourceManager.Instance.GetResource(ResourceType.Scrap)}";
        if (meatText != null) meatText.text = $": {ResourceManager.Instance.GetResource(ResourceType.Meat)}";
    }

    void Update()
    {
        UpdateButtonStates();
        HandleInventoryInput();
    }

    void HandleInventoryInput()
    {
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
            if (inventoryPanel != null) inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    void UpdateButtonStates()
    {
        if (FacilityPlacementManager.Instance == null || ResourceManager.Instance == null) return;

        for (int i = 0; i < facilityButtons.Length && i < facilityDataArray.Length; i++)
        {
            int wood  = ResourceManager.Instance.GetResource(ResourceType.Wood);
            int scrap = ResourceManager.Instance.GetResource(ResourceType.Scrap);
            int meat = ResourceManager.Instance.GetResource(ResourceType.Meat);

            bool canAfford = wood  >= facilityDataArray[i].woodCost &&
                             scrap >= facilityDataArray[i].scrapCost &&
                             meat >= facilityDataArray[i].meatCost;

            facilityButtons[i].interactable = canAfford;

            bool isSelected = FacilityPlacementManager.Instance.GetSelectedFacility() == facilityDataArray[i];
            facilityButtons[i].GetComponent<Image>().color =
                isSelected  ? new Color(0.7f, 1f, 0.7f) :
                canAfford   ? Color.white :
                              new Color(0.5f, 0.5f, 0.5f);
        }
    }
}