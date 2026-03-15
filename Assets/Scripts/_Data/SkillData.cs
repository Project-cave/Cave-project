using UnityEngine;

[CreateAssetMenu(menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public int skillId;     
    public bool isUnlocked;
    public int damage;
    public float cooldown;
    public Sprite icon;
    public string description;
}
