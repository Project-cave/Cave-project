using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    // 컴포넌트 참조
    private UnitMovement movement;
    private UnitAnimator unitAnimator;
    private Animator anim;
    private UnitCombat combat;
    private Scanner scanner;
    private UnitStatHandler statHandler;
    public Rigidbody2D rigid;
    private SpriteRenderer sr;

    public UnitMovement Movement => movement;
    public UnitAnimator Animator => unitAnimator;
    public UnitCombat Combat => combat;
    public Scanner Scanner => scanner;

    // StateMachine
    private StateMachine stateMachine;
    private IState state;
    private IdleState idleState;
    private MoveState moveState;
    private UnitAttackState attackState;

    public IdleState IdleState => idleState;
    public MoveState MoveState => moveState;
    public UnitAttackState AttackState => attackState;

    public UnitSo unitData;
    public bool moveable = false;
    public bool hasPlayerCommand = false;
    public bool HasPlayerCommand => hasPlayerCommand;

    [HideInInspector] public bool isCriticalContext;
    private void Awake()
    {
        movement = GetComponent<UnitMovement>();
        unitAnimator = GetComponent<UnitAnimator>();
        combat = GetComponent<UnitCombat>();
        scanner = GetComponent<Scanner>();
        statHandler = GetComponent<UnitStatHandler>();
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (statHandler != null)
        {
            statHandler.OnDeath += Death;
        }

        stateMachine = new StateMachine();
        idleState = new IdleState(this);
        moveState = new MoveState(this);
        attackState = new UnitAttackState(this);
    }

    private void Start()
    {
        stateMachine.ChangeState(idleState);
        InitUnit(unitData);
    }

    private void OnEnable()
    {
        if (stateMachine != null && IdleState != null)
        {
            stateMachine.ChangeState(IdleState);
        }
    }

    private void Update()
    {
        if (statHandler.isDead) return;

        if (Time.time - statHandler.LastAttackTime < statHandler.AttackMotionDelay)
        {
            rigid.linearVelocity = Vector2.zero;
            return;
        }

        stateMachine.Update();
    }

    protected virtual void OnDestroy()
    {
        if (statHandler != null)
        {
            statHandler.OnDeath -= Death;
        }
    }

    public void ChangeState(IState newState)
    {
        stateMachine.ChangeState(newState);
    }

    public void InitUnit(UnitSo unitData)
    {
        this.unitData = unitData;
        unitAnimator.SetAnimator(unitData.animController);
        movement.SetSpeed(unitData.combatStats.baseMoveSpeed);
        combat.SetAttackRange(unitData.combatStats.baseAttackRange);
        combat.bulletSpeed = unitData.combatStats.baseAttackSpeed;

        statHandler.InitializeStats(unitData);
    }

    public void MoveToPosition(Vector3 targetPosition)
    {
        if (!moveable)
        {
            Debug.LogWarning($"[{name}] moveable이 false입니다!");
            return;
        }

        hasPlayerCommand = true;
        movement.SetDestination(targetPosition);
        ChangeState(moveState);
    }

    public void ClearPlayerCommand()
    {
        hasPlayerCommand = false;
    }

    public void AttackTarget(Transform target)
    {
        if (!moveable) return;

        hasPlayerCommand = true;
        movement.SetDestination(target.position);
        ChangeState(moveState);
    }

    // 무기 설정
    public void SetWeapon(Weapon weapon)
    {
        combat.SetWeapon(weapon);
    }

    public void OnAnimAttackHit()
    {
        if (unitData.info.attackType == UnitSo.UnitAttackType.Normal_Melee) {
            if (scanner.attackTarget != null)
            {
                StatHandler targetStat = scanner.attackTarget.GetComponent<StatHandler>();

                if (targetStat != null)
                {
                    float finalDamage = statHandler.AttackPower * statHandler.DamageMultiplier;

                    if (isCriticalContext)
                    {
                        finalDamage *= statHandler.CriticalMultiplier;
                    }
                    targetStat.TakeDamage(Mathf.RoundToInt(finalDamage), transform);

                    Debug.Log(statHandler.AttackPower + "과 " + statHandler.DamageMultiplier + " = " + finalDamage);
                }
            }
        }

        else
        {
            if (scanner.attackTarget != null && GameManager.instance.pool != null)
            {
                Vector2 spawnPos = transform.position;

                GameObject bulletObj = GameManager.instance.pool.Get(combat.GetWeapon());

                if (bulletObj == null) return;
                bulletObj.transform.position = spawnPos;

                Bullet bullet = bulletObj.GetComponent<Bullet>();

                if (bullet != null)
                {
                    float finalDamage = statHandler.AttackPower * statHandler.DamageMultiplier;

                    if (isCriticalContext)
                    {
                        finalDamage *= statHandler.CriticalMultiplier;
                    }

                    Vector2 dir = ((Vector2)scanner.attackTarget.position - spawnPos).normalized;

                    string dynamicTargetTag = scanner.attackTarget.tag;

                    bullet.Init(gameObject.GetInstanceID(), finalDamage, 0, dir, statHandler.CollisionSpeed, dynamicTargetTag, transform);
                }
            }
        }
    }

    public void Death()
    {
        if (state != null)
        {
            state.Exit();
            state = null;
        }

        GetComponent<Collider2D>().enabled = false;
        rigid.linearVelocity = Vector2.zero;
        rigid.bodyType = RigidbodyType2D.Kinematic;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        anim.SetTrigger("DeathTrigger");

        yield return null;

        float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
        float waitTime = 0f;
        while (waitTime < animLength)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }

        float fadeTime = 1.0f;
        float startAlpha = sr.color.a;
        float time = 0;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0, time / fadeTime);

            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

            yield return null;
        }

        gameObject.SetActive(false);

        if (UnitManager.instance != null)
        {
            UnitManager.instance.UnRegisterUnit(this.gameObject);
        }
    }
}
