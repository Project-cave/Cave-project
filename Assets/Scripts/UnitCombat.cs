using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    private Weapon weapon;
    private Scanner scanner;
    private UnitAnimator unitAnimator;

    public int health;
    public int bulletSpeed;

    private void Awake()
    {
        scanner = GetComponent<Scanner>();
        unitAnimator = GetComponent<UnitAnimator>();
    }

    public void SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
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
        if (scanner == null || scanner.attackTarget == null) return;

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
    }

    public void ExecuteMeleeAttack()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Sword);
    }

    public void ExecuteRangedAttack()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Magic);

        if (weapon != null)
            weapon.Fire();
    }

    public bool IsEnemyInRange()
    {
        return scanner != null && scanner.inAttackRange;
    }

    public bool HasTarget()
    {
        return scanner != null && scanner.attackTarget != null;
    }

    public void Death()
    {
    }
}
