using UnityEngine;

public abstract class SkillEffectSo : ScriptableObject
{
    public abstract void ApplyEffect(StatHandler attacker, StatHandler target);
}