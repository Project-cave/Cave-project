using System;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    #region 1. 설정값

    [Header("Combat Stat")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int attackPower = 10;
    [SerializeField] protected float damageMultiplier = 0.5f;
    [Range(0, 100)]
    [SerializeField] private int criticalRate = 20;
    [SerializeField] protected float criticalMultiplier = 2.0f;
    [SerializeField] private float baseAttackSpeed = 1.0f;
    [SerializeField] private float attackMotionDelay = 0.7f;

    [Header("Movement & Sigts")]
    [SerializeField] private float moveSpeed = 5f;

    #endregion

    #region 2. 변수

    // 전투 관련
    private int currentHP;
    private float lastAttackTime;
    private float attackSpeedPercentage = 1.0f;

    // 필드 호출
    Enemy enemyOwner;
    PlayerMovement playerOwner;
    Animator anim;

    #endregion

    #region 3. 프로퍼티

    public float CurrentAttackSpeed
    {
        get
        {
            if (attackSpeedPercentage <= 0) return baseAttackSpeed;
            return baseAttackSpeed / attackSpeedPercentage;
        }
    }
    public int AttackPower => attackPower;
    public float DamageMultiplier => damageMultiplier;
    public int CriticalRate => criticalRate;
    public float CriticalMultiplier => criticalMultiplier;
    public float AttackMotionDelay => attackMotionDelay;
    public float LastAttackTime => lastAttackTime;
    public float MoveSpeed => moveSpeed;

    #endregion

    #region 4. 이벤트

    public event Action<int, int> OnHealthChanged;

    private void Awake()
    {
        enemyOwner = GetComponent<Enemy>();
        playerOwner = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();

        currentHP = maxHP;
        lastAttackTime = -attackMotionDelay;

        OnHealthChanged?.Invoke(currentHP, maxHP);
        UpdateAnimator();
    }

    #endregion

    #region 5. 함수

    // 전투
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        OnHealthChanged?.Invoke(currentHP, maxHP);

        if (currentHP <= 0)
        {
            currentHP = 0;

            if (enemyOwner != null)
            {
                enemyOwner.Death();
            }
            else if (playerOwner != null)
            {
                playerOwner.Death();
            }
        }
    }

    public void OnAttack()
    {
        lastAttackTime = Time.time;
    }

    // 스탯 조작
    public void AddAttackSpeedPercentage(float percentage)
    {
        attackSpeedPercentage += percentage;
        UpdateAnimator();
    }

    // 애니메이션
    private void UpdateAnimator()
    {
        if (anim != null && CurrentAttackSpeed > 0)
        {
            float animsAttackSpeed = 1.0f / CurrentAttackSpeed;
            anim.SetFloat("AttackSpeed", animsAttackSpeed);
        }
    }

    #endregion
}