using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    private Weapon weapon;
    private Scanner scanner;
    private UnitAnimator unitAnimator;
    private UnitStatHandler stat;
    private Unit owner;
    private Animator anim;

    public float bulletSpeed;

    private void Awake()
    {
        scanner = GetComponent<Scanner>();
        unitAnimator = GetComponent<UnitAnimator>();
        stat = GetComponent<UnitStatHandler>();
        owner = GetComponent<Unit>();
        weapon = GetComponent<Weapon>();
        anim = GetComponent<Animator>();
    }

    public bool CanAttack()
    {
        if (stat == null) return false;

        return HasTarget() && IsEnemyInRange() && (Time.time - stat.LastAttackTime >= (1f / stat.BaseAttackSpeed));
    }

    public void SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public int GetWeapon()
    {
        return weapon.id;
    }

    public void SetAttackRange(float range)
    {
        if (scanner != null)
        {
            scanner.attackRange = range;
        }
    }

    public void AimAtTarget()
    {
        owner.rigid.linearVelocity = Vector2.zero;

        if (!CanAttack()) return;

        Transform target = scanner.attackTarget;
        unitAnimator?.FaceTarget(target);

        // wizard 원거리 공격 위치 조정
        if (weapon != null)
        {
            if (target.position.x >= transform.position.x)
                weapon.transform.localPosition = new Vector3(0.5f, 0, 0);
            else
                weapon.transform.localPosition = new Vector3(-0.5f, 0, 0);
        }

        bool isCrit = (Random.Range(0, 100) <= stat.CriticalRate);
        owner.isCriticalContext = isCrit;

        int skillIndex = isCrit ? 1 : 0;

        anim.SetInteger("AttackIndex", skillIndex);
        if (unitAnimator != null) unitAnimator.PlayAttackMotion();

        stat.OnAttack();
    }

    public void ExecuteMeleeAttack()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Sword);
        owner.OnAnimAttackHit();
    }

    public void ExecuteRangedAttack()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Magic);

        weapon.Fire();
        owner.OnAnimAttackHit();
    }

    public bool IsEnemyInRange()
    {
        return scanner != null && scanner.inAttackRange;
    }

    public bool HasTarget()
    {
        return scanner != null && scanner.attackTarget != null;
    }
}
