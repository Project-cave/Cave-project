using System.Linq;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    public static PlayerSkillManager instance;

    [Header("장착 슬롯 수")]
    [SerializeField] private int maxSlots = 5;
    [SerializeField] private SkillData[] equippedSkills;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject); 
        equippedSkills = new SkillData[maxSlots];
    }

    public void SaveSkillToPlayerManager()
    {
        PlayerManager.instance.SetSkills(equippedSkills.ToList());
    }

    public void OnSkillEquipped(int slotIndex, SkillData skill)
    {
        if (!IsValidIndex(slotIndex)) return;
        equippedSkills[slotIndex] = skill;
    }

    public void OnSkillRemoved(int slotIndex, SkillData _)
    {
        if (!IsValidIndex(slotIndex)) return;
        equippedSkills[slotIndex] = null;
    }

    // 아직 사용 안하는 중
    public SkillData GetEquippedSkill(int slotIndex)
    {
        if (!IsValidIndex(slotIndex)) return null;
        return equippedSkills[slotIndex];
    }

    public SkillData[] GetAllEquipped() => equippedSkills;

    private bool IsValidIndex(int index)
        => index >= 0 && index < maxSlots;
}
