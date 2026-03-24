using UnityEngine;

[CreateAssetMenu(fileName = "ThunderEffect", menuName = "Scriptable Object/Skill Effects/Thunder")]
public class ThunderEffectSo : SkillEffectSo
{
    public int paralysisTime = 3;

    public override void ApplyEffect(StatHandler attacker, StatHandler target)
    {
        // 전기 공격 시 마비 상태이상 부여
    }
}