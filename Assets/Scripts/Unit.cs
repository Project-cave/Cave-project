using UnityEngine;

public class Unit : MonoBehaviour
{
    Animator anim;
    PlayerMovement movement;
    Scanner scanner;
    public int health;
    public int bulletSpeed;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        scanner = GetComponent<Scanner>();
    }

    public void InitUnit(UnitSo unitData)
    {
        anim.runtimeAnimatorController = unitData.animController;
        movement.Movement = unitData.speed;
        scanner.AttackRange = unitData.range;
        health = unitData.health;
        bulletSpeed = unitData.bulletSpeed;
    }
    
}
