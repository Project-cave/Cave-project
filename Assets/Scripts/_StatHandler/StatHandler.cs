using System;
using UnityEngine;

public abstract class StatHandler : MonoBehaviour
{
    #region 1. 설정값

    [field: Header("Stat")]
    [field: SerializeField] public int MaxHP { get; protected set; }
    [field: SerializeField] public int AttackPower { get; protected set; }
    [field: SerializeField] public int Defence { get; protected set; }
    [field: SerializeField] public float MoveSpeed { get; protected set; }
    [field: SerializeField] public float BaseAttackSpeed { get; protected set; }
    [field: SerializeField] public float AttackRange { get; protected set; }
    [field: SerializeField] public float DamageMultiplier { get; protected set; }
    [field: SerializeField] public float CollisionSpeed { get; protected set; }
    [field: SerializeField] public string CollisionEffect { get; protected set; }
    [Range(0, 100)]
    [field: SerializeField] public int CriticalRate { get; protected set; }
    [field: SerializeField] public float CriticalMultiplier { get; protected set; }
    [field: SerializeField] public string SpecialGimmick { get; protected set; }
    [field: SerializeField] public float AttackMotionDelay { get; protected set; }

    #endregion

    #region 2. 변수

    // 전투 관련
    private int currentHP;
    public float LastAttackTime { get; protected set; }
    private float attackSpeedPercentage = 1.0f;

    #endregion

    #region 3. 프로퍼티

    public float CurrentAttackSpeed
    {
        get
        {
            if (attackSpeedPercentage <= 0) return BaseAttackSpeed;
            return BaseAttackSpeed / attackSpeedPercentage;
        }
    }

    public int CurrentHP {
        get => currentHP;
        set
        {
            currentHP = Mathf.Clamp(value, 0, MaxHP);
            OnHealthChanged?.Invoke(currentHP, MaxHP);
        }
    }

    public bool isDead { get; protected set; }

    #endregion

    #region 4. 이벤트

    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;

    #endregion  

    #region 5. 함수

    // 전투
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        CurrentHP -= damage;

        if (currentHP <= 0)
        {
            isDead = true;
            OnDeath?.Invoke();
        }
    }

    public void OnAttack()
    {
        LastAttackTime = Time.time;
    }

    // 스탯 조작
    public void AddAttackSpeedPercentage(float percentage)
    {
        attackSpeedPercentage += percentage;
    }

    #endregion
}