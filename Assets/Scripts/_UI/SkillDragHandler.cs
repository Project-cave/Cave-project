using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SkillDragHandler : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("스킬 데이터")]
    [SerializeField] private SkillData skillData;

    // 스킬 목록에서 온 건지, 장착 슬롯에서 온 건지 구분
    public bool IsEquipped = false;

    private Image image;
    private CanvasGroup canvasGroup;
    private RectTransform rect;
    private Transform canvas;
    private Transform originalParent;
    public Transform OriginalParent { get => originalParent; set => originalParent = value; }
    public int originalSlotInt;
    private Vector2 originalPosition;

    private void Awake()
    {
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
        canvas = FindFirstObjectByType<Canvas>().transform;

        RefreshIcon();
    }

    public SkillData GetSkillData() => skillData;

    public void SetSkillData(SkillData data)
    {
        skillData = data;
        RefreshIcon();
    }

    private void RefreshIcon()
    {
        if (image == null) return;
        image.sprite = skillData != null ? skillData.icon : null;
        image.color = skillData != null
            ? (skillData.isUnlocked ? Color.white : new Color(1, 1, 1, 0.4f))
            : Color.white;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (skillData == null || !skillData.isUnlocked) // 미습득 스킬은 드래그 막기
        {
            eventData.pointerDrag = null; 
            return;
        }
        originalParent = transform.parent;
        originalPosition = rect.anchoredPosition;

        transform.SetParent(canvas);
        transform.SetAsLastSibling();

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ReturnToOriginal();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void ReturnToOriginal()
    {
        transform.SetParent(originalParent);
        rect.anchoredPosition = originalPosition;
    }
}
