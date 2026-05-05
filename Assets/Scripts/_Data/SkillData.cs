using System.Collections.Generic;
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

    [Header("레퍼런스")]
    public GameObject SkillPrefab;

    [HideInInspector]
    public List<Vector2Int> affectedTiles = new List<Vector2Int>();
}