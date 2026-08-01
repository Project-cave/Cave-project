using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// 약탈 지도 UI.
/// 
/// 씬 구성 예시:
///   RaidMapPanel (이 스크립트)
///     ├─ MapArea (지도 이미지 배경)
///     │    ├─ Pin_Village1 (Button) ─ RaidMapPin 컴포넌트
///     │    ├─ Pin_Village2 (Button) ─ RaidMapPin 컴포넌트
///     │    └─ Pin_Village3 (Button) ─ RaidMapPin 컴포넌트
///     ├─ InfoPanel (기본 비활성화)
///     │    ├─ CloseButton
///     │    ├─ LocationNameText
///     │    ├─ DescriptionText
///     │    ├─ TimeText          (소요 시간)
///     │    ├─ RewardText        (획득 자원)
///     │    ├─ SuccessRateText   (성공률)
///     │    ├─ DifficultyGroup   (난이도 버튼 3개)
///     │    │    ├─ EasyButton
///     │    │    ├─ NormalButton
///     │    │    └─ HardButton
///     │    └─ StartButton       (파견 시작)
///     ├─ ConfirmPopup (기본 비활성화)   ← 재확인 팝업
///     │    ├─ ConfirmSuccessRateText
///     │    ├─ ConfirmTimeText
///     │    ├─ ConfirmRewardText
///     │    ├─ ConfirmYesButton          (예)
///     │    └─ ConfirmNoButton           (아니요)
///     └─ CloseMapButton         (X 버튼, 지도 전체 닫기)
/// </summary>
public class RaidMapUI : MonoBehaviour
{
    public static RaidMapUI Instance { get; private set; }

    [Header("패널")]
    public GameObject raidMapPanel;
    public GameObject infoPanel;

    [Header("핀 (지도 위 마을 버튼)")]
    public RaidMapPin[] pins;  // Inspector에서 각 핀 연결

    [Header("정보 패널 - 텍스트")]
    public TextMeshProUGUI locationNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI successRateText;

    [Header("난이도 버튼")]
    public Button easyButton;
    public Button normalButton;
    public Button hardButton;

    [Header("파견 시작 / 닫기")]
    public Button startButton;
    public Button closeMapButton;    // 지도 전체 닫기

    [Header("결과 패널")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public Button resultCloseButton;

    [Header("재확인 팝업")]
    public GameObject confirmPopup;
    public TextMeshProUGUI confirmTitleText;
    public TextMeshProUGUI confirmSuccessRateText;
    public TextMeshProUGUI confirmTimeText;
    public TextMeshProUGUI confirmRewardText;
    public Button confirmYesButton;
    public Button confirmNoButton;

    // 내부 상태
    private RaidData selectedRaid;       // 최종 선택된 RaidData (마을 + 난이도 확정)
    private int selectedVillageId = -1;  // 현재 클릭된 마을
    private RaidDifficulty selectedDifficulty = RaidDifficulty.Easy;

    // 난이도 버튼 색상
    private static readonly Color COL_SELECTED   = new Color(0.9f, 0.7f, 0.2f);  // 선택됨 (황금)
    private static readonly Color COL_UNSELECTED = new Color(0.4f, 0.4f, 0.4f);  // 미선택 (회색)
    private static readonly Color COL_DISABLED   = new Color(0.25f, 0.25f, 0.25f);

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 버튼 이벤트 등록
        easyButton?.onClick.AddListener(() => SelectDifficulty(RaidDifficulty.Easy));
        normalButton?.onClick.AddListener(() => SelectDifficulty(RaidDifficulty.Normal));
        hardButton?.onClick.AddListener(() => SelectDifficulty(RaidDifficulty.Hard));
        startButton?.onClick.AddListener(OnStartRaid);
        closeMapButton?.onClick.AddListener(CloseMapPanel);
        resultCloseButton?.onClick.AddListener(() => resultPanel?.SetActive(false));
        confirmYesButton?.onClick.AddListener(OnConfirmYes);
        confirmNoButton?.onClick.AddListener(() => confirmPopup?.SetActive(false));

        // 핀 이벤트 등록
        foreach (var pin in pins)
            pin.OnPinClicked += OnPinClicked;

        // 초기 상태
        infoPanel?.SetActive(false);
        raidMapPanel?.SetActive(false);
        resultPanel?.SetActive(false);
        confirmPopup?.SetActive(false);
    }

    // ── 외부 호출 ──────────────────────────────────────────────

    /// <summary>지도 패널 열기 (MenuController 또는 UIManager에서 호출)</summary>
    public void OpenMapPanel()
    {
        Debug.Log($"RaidMapPanel: {raidMapPanel}, destroyed: {raidMapPanel == null}");

        raidMapPanel?.SetActive(true);
        infoPanel?.SetActive(false);
        selectedVillageId = -1;
        selectedRaid = null;

        // 진행 중인 약탈이 있으면 해당 핀 강조
        RefreshPinStates();
    }

    // ── 핀 클릭 ──────────────────────────────────────────────

    void OnPinClicked(int villageId)
    {
        selectedVillageId = villageId;

        // 기본 난이도 Easy로 세팅 후 정보 패널 표시
        SelectDifficulty(RaidDifficulty.Easy);
        infoPanel?.SetActive(true);

        RefreshPinStates();
    }

    // ── 난이도 선택 ──────────────────────────────────────────

    void SelectDifficulty(RaidDifficulty diff)
    {
        selectedDifficulty = diff;

        // RaidData 찾기 (마을 ID + 난이도 일치)
        selectedRaid = RaidManager.Instance
            .GetRaidLocations()
            .FirstOrDefault(r => r.villageId == selectedVillageId && r.difficulty == diff);

        RefreshInfoPanel();
        RefreshDifficultyButtons();
    }

    // ── 정보 패널 갱신 ────────────────────────────────────────

    void RefreshInfoPanel()
    {
        if (selectedRaid == null)
        {
            // 해당 마을에 이 난이도 데이터가 없으면 회색 처리
            if (locationNameText) locationNameText.text = $"마을 {selectedVillageId}";
            if (descriptionText)  descriptionText.text  = "해당 난이도 데이터 없음";
            if (timeText)         timeText.text          = "-";
            if (rewardText)       rewardText.text        = "-";
            if (successRateText)  successRateText.text   = "-";
            if (startButton)      startButton.interactable = false;
            return;
        }

        if (locationNameText) locationNameText.text = selectedRaid.GetLocationName();
        if (timeText)         timeText.text         = $"소요시간: {selectedRaid.raidDuration}초";
        if (successRateText)  successRateText.text  = $"성공확률: {selectedRaid.baseSuccessRate}%";

        // 보상 텍스트 조합
        if (rewardText)
        {
            string reward = "";
            foreach (var r in selectedRaid.possibleRewards)
            {
                string resName = r.resourceType == ResourceType.Wood  ? "획득 자원: 나무" :
                                 r.resourceType == ResourceType.Scrap ? "획득 자원: 고철" : "획득 자원: 고기";
                reward += $"{resName} {r.minAmount}~{r.maxAmount}\n";
            }
            rewardText.text = reward.TrimEnd('\n');
        }

        // 약탈 중이면 시작 버튼 비활성화
        bool raiding = RaidManager.Instance != null && RaidManager.Instance.IsRaidActive();
        if (startButton) startButton.interactable = !raiding;
    }

    void RefreshDifficultyButtons()
    {
        SetDifficultyButtonColor(easyButton,   RaidDifficulty.Easy);
        SetDifficultyButtonColor(normalButton, RaidDifficulty.Normal);
        SetDifficultyButtonColor(hardButton,   RaidDifficulty.Hard);
    }

    void SetDifficultyButtonColor(Button btn, RaidDifficulty diff)
    {
        if (btn == null) return;

        // 해당 마을에 이 난이도 데이터가 있는지 확인
        bool exists = RaidManager.Instance
            .GetRaidLocations()
            .Any(r => r.villageId == selectedVillageId && r.difficulty == diff);

        btn.interactable = exists;

        var img = btn.GetComponent<Image>();
        if (img == null) return;

        if (!exists)
            img.color = COL_DISABLED;
        else
            img.color = (diff == selectedDifficulty) ? COL_SELECTED : COL_UNSELECTED;
    }

    // ── 핀 상태 갱신 ─────────────────────────────────────────

    void RefreshPinStates()
    {
        foreach (var pin in pins)
            pin.SetSelected(pin.VillageId == selectedVillageId);
    }

    // ── 파견 시작 ─────────────────────────────────────────────

    void OnStartRaid()
    {
        if (selectedRaid == null || RaidManager.Instance == null) return;
        if (RaidManager.Instance.IsRaidActive())
        {
            Debug.Log("이미 약탈 중입니다!");
            return;
        }

        // 바로 시작하지 않고 재확인 팝업 표시
        if (confirmTitleText)       confirmTitleText.text       = $"{selectedRaid.GetLocationName()} - {selectedRaid.GetDifficultyName()}";
        if (confirmSuccessRateText) confirmSuccessRateText.text = $"성공률 {selectedRaid.baseSuccessRate}%";
        if (confirmTimeText)        confirmTimeText.text        = $"소요 시간 {selectedRaid.raidDuration}초";

        if (confirmRewardText)
        {
            string reward = "";
            foreach (var r in selectedRaid.possibleRewards)
            {
                string resName = r.resourceType == ResourceType.Wood  ? "나무" :
                                 r.resourceType == ResourceType.Scrap ? "고철" : "고기";
                reward += $"획득 자원 {resName} {r.minAmount}~{r.maxAmount}\n";
            }
            confirmRewardText.text = reward.TrimEnd('\n');
        }

        confirmPopup?.SetActive(true);
    }

    // 재확인 팝업에서 "예" 클릭
    void OnConfirmYes()
    {
        confirmPopup?.SetActive(false);
        RaidManager.Instance.StartRaid(selectedRaid);
        CloseMapPanel();
    }

    // ── 닫기 ─────────────────────────────────────────────────

    void CloseInfoPanel()
    {
        infoPanel?.SetActive(false);
        selectedVillageId = -1;
        selectedRaid = null;
        RefreshPinStates();
    }

    void CloseMapPanel()
    {
        raidMapPanel?.SetActive(false);
    }

    // ── 약탈 결과 표시 (RaidManager에서 호출) ────────────────────────

    public void ShowRaidResult(bool success, RaidData raid)
    {
        if (resultPanel == null || resultText == null) return;

        if (success)
        {
            string rewardText = "";
            foreach (var r in raid.possibleRewards)
            {
                string resName = r.resourceType == ResourceType.Wood  ? "나무" :
                                 r.resourceType == ResourceType.Scrap ? "고철" : "고기";
                rewardText += $"{resName} 획득\n";
            }
            resultText.text = $"{raid.GetLocationName()} 약탈 성공!\n\n{rewardText.TrimEnd('\n')}";
        }
        else
        {
            resultText.text = $"{raid.GetLocationName()} 약탈 실패!";
        }

        resultPanel.SetActive(true);
    }
}