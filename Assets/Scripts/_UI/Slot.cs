using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    [Header("슬롯 설정")]
    [SerializeField] private int slotIndex;
    [SerializeField] private bool isEquipSlot; // 장착 슬롯인지 목록 슬롯인지

    [Header("SoEvent 연결")]
    [SerializeField] private SkillEvent onSkillEquipped;
    [SerializeField] private SkillEvent onSkillRemoved;

    private Image image;
    private SkillDragHandler skillHandler;

    public SkillData originalSkill;
    public SkillData CurrentSkill => skillHandler?.GetSkillData();
    public int SlotIndex => slotIndex;

    private void Awake()
    {
        image = GetComponent<Image>();
        skillHandler = GetComponentInChildren<SkillDragHandler>(true);
    }

    public void OnPointerEnter(PointerEventData eventData) => image.color = Color.yellow;
    public void OnPointerExit(PointerEventData eventData) => image.color = Color.white;

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag?.GetComponent<SkillDragHandler>();
        if (dragged == null) return;

        if (isEquipSlot)
            HandleEquipDrop(dragged);
        else
            HandleListDrop(dragged);
    }

    private void HandleEquipDrop(SkillDragHandler dragged)
    {
        var incomingSkill = dragged.GetSkillData();
        var currentSkill = CurrentSkill;
        var originalSlot = dragged.OriginalParent;
        Debug.Log($"currentSkill : {currentSkill} , incomingSkill : {incomingSkill}");

        if(currentSkill != null)
        {
            if (dragged.IsEquipped) // 장착 중인 두 스킬 교체
            {
                dragged.SetSkillData(currentSkill);
                SetSkill(incomingSkill);
                onSkillEquipped?.Raise(originalSlot.GetComponent<Slot>().SlotIndex, currentSkill);
            }
            else if (!dragged.IsEquipped) // 장착 중이지 않은 스킬이 장착 중인 스킬 교체
            {
                dragged.SetSkillData(null);
                SetSkill(incomingSkill);
                SlotManager.instance.GetListSlot(currentSkill.skillId).SetSkill(currentSkill);
            }
        }
        else
        {           
            dragged.SetSkillData(currentSkill);
            SetSkill(incomingSkill);
            if (dragged.IsEquipped) // 장착 중인 스킬을 내 스킬 창 빈 슬롯에 옮길 때
            {
                onSkillRemoved?.Raise(originalSlot.GetComponent<Slot>().SlotIndex, null);
            }
        }

        onSkillEquipped?.Raise(slotIndex, incomingSkill);
    }

    private void HandleListDrop(SkillDragHandler dragged)
    {
        if (!dragged.IsEquipped) return;

        var incomingSkill = dragged.GetSkillData();
        var currentSkill = CurrentSkill;
        var originalSlot = dragged.OriginalParent;

        if(originalSkill != null)
            if (incomingSkill != originalSkill) return;

        dragged.SetSkillData(currentSkill);
        SetSkill(incomingSkill);

        Debug.Log(originalSlot.GetComponent<Slot>().SlotIndex);
        onSkillRemoved?.Raise(originalSlot.GetComponent<Slot>().SlotIndex, null);
    }

    public void SetSkill(SkillData skill) => skillHandler?.SetSkillData(skill);
}
