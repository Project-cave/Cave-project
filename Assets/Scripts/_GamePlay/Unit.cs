using UnityEngine;

public class Unit : MonoBehaviour
{
    // 컴포넌트 참조
    private UnitMovement movement;
    private UnitAnimator unitAnimator;
    private UnitCombat combat;
    private Scanner scanner;
    private UnitStatHandler statHandler;

    public UnitMovement Movement => movement;
    public UnitAnimator Animator => unitAnimator;
    public UnitCombat Combat => combat;
    public Scanner Scanner => scanner;

    // StateMachine
    private StateMachine stateMachine;
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

    private void Awake()
    {
        movement = GetComponent<UnitMovement>();
        unitAnimator = GetComponent<UnitAnimator>();
        combat = GetComponent<UnitCombat>();
        scanner = GetComponent<Scanner>();
        statHandler = GetComponent<UnitStatHandler>();

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

    private void Update()
    {
        stateMachine.Update();
    }

    public void ChangeState(IState newState)
    {
        stateMachine.ChangeState(newState);
    }

    public void InitUnit(UnitSo unitData)
    {
        this.unitData = unitData;
        unitAnimator.SetAnimator(unitData.animController);
        movement.SetSpeed(unitData.baseMoveSpeed);
        combat.SetAttackRange(unitData.baseAttackRange);
        combat.health = unitData.baseHP;
        combat.bulletSpeed = unitData.baseAttackSpeed;

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

    private void OnDisable()
    {
        if (UnitManager.instance != null)
        {
            UnitManager.instance.UnRegisterUnit(gameObject);
        }
    }
}
