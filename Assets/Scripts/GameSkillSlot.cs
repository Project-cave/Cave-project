using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class GameSkillSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private SkillData skillData;
    [SerializeField] private Image skillIcon;

    private Canvas canvas;
    private GameObject dragPreview;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        skillIcon = GetComponentsInChildren<Image>()[1];
    }

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();

        if (skillData != null && skillIcon != null)
        {
            skillIcon.sprite = skillData.icon;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"Begin Drag: {skillData?.skillName}");

        if (skillData == null)
        {
            Debug.LogError("SkillData is null!");
            return;
        }

        dragPreview = new GameObject("Drag Preview");
        dragPreview.transform.SetParent(canvas.transform, false);

        Image previewImage = dragPreview.AddComponent<Image>();
        previewImage.sprite = skillData.icon;
        previewImage.color = new Color(1, 1, 1, 0.6f);
        previewImage.raycastTarget = false;

        RectTransform previewRect = dragPreview.GetComponent<RectTransform>();
        previewRect.sizeDelta = rectTransform.sizeDelta;
        previewRect.position = eventData.position;

        GridOverlayManager.Instance.ShowFullGrid();

        Vector2 mouseWorldPos = GridOverlayManager.Instance.GetMouseWorldPosition();
        GridOverlayManager.Instance.UpdateSkillRange(mouseWorldPos, skillData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragPreview != null)
        {
            dragPreview.transform.position = eventData.position;
        }

        if (GridOverlayManager.Instance != null && skillData != null)
        {
            Vector2 mouseWorldPos = GridOverlayManager.Instance.GetMouseWorldPosition();
            GridOverlayManager.Instance.UpdateSkillRange(mouseWorldPos, skillData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");

        if (dragPreview != null)
        {
            Destroy(dragPreview);
        }

        if (skillData != null && GridOverlayManager.Instance != null)
        {
            Vector2 mouseWorldPos = GridOverlayManager.Instance.GetMouseWorldPosition();
            ExecuteSkill(mouseWorldPos);
        }

        GridOverlayManager.Instance?.ClearAll();
    }

    private void ExecuteSkill(Vector2 worldPosition)
    {
        Debug.Log($"{skillData.skillName} , {worldPosition}");

        GameObject skill = GameManager.instance.pool.Get(15);

        skill.transform.position = worldPosition;
    }
}