using UnityEngine;
using UnityEngine.EventSystems;

public class Skill : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    Transform canvas;
    public Transform previousParent;
    RectTransform rect;
    CanvasGroup canvasGroup;
    public SkillSo skill;
    public bool isDragged = false;

    private void Awake()
    {
        canvas = FindFirstObjectByType<Canvas>().transform;
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragged = true;
        previousParent = transform.parent;

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

        isDragged = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        bool isInvalidDrop = transform.parent == canvas;

        if (isInvalidDrop) { RevertToParent(); return; }      
        PlayerManager.instance.PushSkill(skill, GetComponentInParent<ItemSlot>().index);
        previousParent = transform.parent;
    }

    private void RevertToParent()
    {
        transform.SetParent(previousParent);
        rect.position = previousParent.GetComponent<RectTransform>().position;
    }
}
