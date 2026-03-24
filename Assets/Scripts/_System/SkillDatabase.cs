using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/SkillDatabase")]
public class SkillDatabase : ScriptableObject
{
    public List<SkillData> allSkills;

#if UNITY_EDITOR
    [ContextMenu("모든 스킬 로드")]
    private void LoadAllSkills()
    {
        allSkills = Resources.LoadAll<SkillData>("Data/So/Skills").ToList();
        allSkills.Sort((a,b) => a.skillId.CompareTo(b.skillId));
    }
#endif

    public List<SkillData> GetUnlocked()
        => allSkills.FindAll(s => s.isUnlocked);

    public SkillData GetByName(string skillName)
        => allSkills.Find(s => s.skillName == skillName);
}
