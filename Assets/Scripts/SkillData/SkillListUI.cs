using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 보유 스킬 목록을 슬롯에 뿌려주는 UI
/// SkillDatabase SO를 받아서 목록 생성
/// </summary>
public class SkillListUI : MonoBehaviour
{
    [Header("데이터")]
    [SerializeField] private SkillDatabase database;

    [Header("UI")]
    [SerializeField] private Transform content;       
    [SerializeField] private GameObject slotPrefab;   

    private readonly List<Slot> slots = new();

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        // 기존 슬롯 제거
        foreach (var slot in slots)
            Destroy(slot.gameObject);
        slots.Clear();
        SlotManager.instance.ClearListSlots();

        // 스킬 목록 뿌리기
        foreach (var skill in database.allSkills)
        {
            var obj = Instantiate(slotPrefab, content);
            var slot = obj.GetComponent<Slot>();
            slot.SetSkill(skill);
            slot.originalSkill = skill;
            slots.Add(slot);
            SlotManager.instance.RegisterListSlot(skill.skillId, slot);
        }
    }
}
