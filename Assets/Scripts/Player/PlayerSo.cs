using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Scriptable Object/player")]
public class PlayerSo : ScriptableObject
{
    public SkillSo[] skills;
}
