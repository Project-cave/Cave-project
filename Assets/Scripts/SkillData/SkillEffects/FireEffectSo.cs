using UnityEngine;

[CreateAssetMenu(fileName = "FireEffect", menuName = "Scriptable Object/Skill Effects/Fire")]
public class FireEffectSo : SkillEffectSo
{
    public int burnDamagePercentage = 1;
    public int burnTick = 5;

    public override void ApplyEffect(StatHandler attacker, StatHandler target)
    {
        // 화염 속성 공격 + 치명타 발생 시 Burn 상태이상 발동 
    }
}