using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("Menu Objects")]
    [SerializeField] private GameObject menuToggleButton;
    [SerializeField] private TextMeshProUGUI toggleButtonText;
    [SerializeField] private GameObject menuPanel;
    
    [Header("Menu Buttons")]
    [SerializeField] private Button unitPlacementButton;  // 건설
    [SerializeField] private Button expandButton;  // 확장
    [SerializeField] private Button raidButton;  // 약탈
    
    [Header("Sub Panels")]
    [SerializeField] private GameObject unitButtonPanel;  // 유닛 3개 버튼
    
    private bool isMenuOpen = false;
    private bool isUnitPanelOpen = false;
    
    void Start()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);
        
        if (unitButtonPanel != null)
            unitButtonPanel.SetActive(false);
        
        SetupButtons();
    }
    
    void SetupButtons()
    {
        if (unitPlacementButton != null)
            unitPlacementButton.onClick.AddListener(OnUnitPlacementClicked);
        
        if (expandButton != null)
            expandButton.onClick.AddListener(OnExpandClicked);
        
        if (raidButton != null)
            raidButton.onClick.AddListener(OnRaidClicked);
    }
    
    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        
        if (menuPanel != null)
            menuPanel.SetActive(isMenuOpen);
        
        // 화살표 방향 바꾸기
        if (toggleButtonText != null)
            toggleButtonText.text = isMenuOpen ? "◀" : "▶";
        
        // 메뉴 닫을 때 유닛 패널도 닫기
        if (!isMenuOpen && isUnitPanelOpen)
        {
            isUnitPanelOpen = false;
            if (unitButtonPanel != null)
                unitButtonPanel.SetActive(false);
        }
    }
    
    void OnUnitPlacementClicked()
    {
        isUnitPanelOpen = !isUnitPanelOpen;
        
        if (unitButtonPanel != null)
            unitButtonPanel.SetActive(isUnitPanelOpen);
    }
    
    void OnExpandClicked()
    {
        if (WallExpansionManager.Instance != null)
        {
            WallExpansionManager.Instance.SetExpansionMode(!WallExpansionManager.Instance.IsExpansionMode());
        }
    }
    
    void OnRaidClicked()
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.OpenRaidMenu();
        }
    }
}