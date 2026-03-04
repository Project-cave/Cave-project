using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Skill : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    Transform canvas;
    Transform previousParent;
    RectTransform rect;
    CanvasGroup canvasGroup;
    public SkillSo skill;
    public bool isDragged = false;
    public bool mySkill = false;
    Image image;
    public Skill home;

    private void Awake()
    {
        canvas = FindFirstObjectByType<Canvas>().transform;
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
        previousParent = transform.parent;
        home = this;
        if(skill == null) gameObject.SetActive(false);
        else image.sprite = skill.icon;
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
        if (!mySkill) { HandleSkillList(eventData); }
        else HandleMySkill(eventData);
        ResetDragState();                 
    }

    private void HandleSkillList(PointerEventData eventData)
    {
        var other = eventData.pointerCurrentRaycast.gameObject;
        if (other == null) { RevertToParent(); return; }

        if (other.CompareTag("Skill")) // 기존 스킬 원래 리스트 위치로 되돌리기
        {
            var otherSkill = other.GetComponent<Skill>();
            otherSkill.home.gameObject.SetActive(true);
            otherSkill.home.skill = otherSkill.skill;
            otherSkill.home.image.sprite = otherSkill.skill.icon;
            otherSkill.skill = skill;
            otherSkill.image.sprite = skill.icon;
            otherSkill.home = home;
            PlayerManager.instance.PushSkill(skill, otherSkill.GetComponentInParent<ItemSlot>().index);
            gameObject.SetActive(false);
        }
        else if(other.CompareTag("Slot"))
        {
            var otherSkill = other.GetComponentInChildren<Skill>(true);
            otherSkill.gameObject.SetActive(true);
            otherSkill.skill = skill;
            otherSkill.image.sprite = skill.icon;
            otherSkill.home = home;
            PlayerManager.instance.PushSkill(skill, otherSkill.GetComponentInParent<ItemSlot>().index);
            gameObject.SetActive(false);
        }
       
        RevertToParent();
    }

    private void HandleMySkill(PointerEventData eventData)
    {
        var other = eventData.pointerCurrentRaycast.gameObject;
        if (other == null) { RevertToParent(); return; }

        var otherSkill = other.GetComponent<Skill>();
        if (other.CompareTag("SkillList"))
        {
            if (other.GetComponent<ItemSlot>().skill != skill) { RevertToParent(); return; }
            otherSkill = other.GetComponentInChildren<Skill>(true);
            otherSkill.gameObject.SetActive(true);
            otherSkill.skill = skill;
            otherSkill.image.sprite = skill.icon;
            gameObject.SetActive(false);
            PlayerManager.instance.PopSkill(previousParent.GetComponentInParent<ItemSlot>().index);
            RevertToParent();
            return;
        }

        if (otherSkill != null && otherSkill.mySkill){ SwapSkillData(otherSkill); }

        RevertToParent();
    }

    private void SwapSkillData(Skill other)
    {
        // SkillSo 데이터만 교환
        (skill, other.skill) = (other.skill, skill);
        (home, other.home) = (other.home, home);

        // 이미지 업데이트
        image.sprite = skill.icon;
        other.image.sprite = other.skill.icon;

        // PlayerManager 업데이트
        int myIndex = previousParent.GetComponent<ItemSlot>().index;
        int otherIndex = other.transform.parent.GetComponent<ItemSlot>().index;
        PlayerManager.instance.PushSkill(skill, myIndex);
        PlayerManager.instance.PushSkill(other.skill, otherIndex);
    }

    private void RevertToParent()
    {
        transform.SetParent(previousParent);
        rect.position = previousParent.GetComponent<RectTransform>().position;
    }

    private void ResetDragState()
    {
        isDragged = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}
