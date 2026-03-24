using UnityEngine;

public class Unit : MonoBehaviour
{
    Animator anim;
    PlayerMovement movement;
    Scanner scanner;
    UnitStatHandler stat;
    public float bulletSpeed;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        scanner = GetComponent<Scanner>();
        stat = GetComponent<UnitStatHandler>();
    }

    private void Start()
    {
        InitUnit(); 
    }

    public void InitUnit()
    {
        if (stat != null)
        {
            stat.InitializeStats();
            if (anim != null && stat.unitData != null) anim.runtimeAnimatorController = stat.unitData.animController;
            if (movement != null) movement.Movement = stat.MoveSpeed;
            if (scanner != null) scanner.attackRange = stat.AttackRange;
            bulletSpeed = stat.CollisionSpeed;
        }
    }
}
