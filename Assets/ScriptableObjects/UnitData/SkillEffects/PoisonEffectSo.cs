using UnityEngine;

[CreateAssetMenu(fileName = "PoisonEffect", menuName = "Scriptable Object/Skill Effects/Poison")]
public class PoisonEffectSo : SkillEffectSo
{
    public int poisonDamage = 1;
    public int poisonTick = 10;

    public override void ApplyEffect(StatHandler attacker, StatHandler target)
    {
        // 독 공격 시 독 상태이상 부여
    }
}