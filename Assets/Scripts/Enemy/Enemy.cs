using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    #region 1. 설정값
    [Header("Reference")]
    SpriteRenderer sr;
    public Animator anim;
    public Rigidbody2D rigid;
    public Scanner scanner;
    public StatHandler stat;
    public PathFinder pathFinder;

    #endregion

    #region 2. 변수

    // 전투 관련
    [HideInInspector] public bool isCriticalContext;

    // 이동 관련
    public LinkedList<Vector2> currentPath = new LinkedList<Vector2>();

    // 상태 머신
    EnemyState state;
    public ExploreState explore;
    public ChaseState chase;
    public AttackState attack;
    public InteractState interact;

    #endregion

    #region 3. 이벤트

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        scanner = GetComponent<Scanner>();
        stat = GetComponent<StatHandler>();
        sr = GetComponent<SpriteRenderer>();

        explore = new ExploreState(this);
        chase = new ChaseState(this);
        attack = new AttackState(this);
        interact = new InteractState(this);
    }

    protected virtual void Start()
    {
        pathFinder = PathFinder.instance;

        if (pathFinder == null)
        {
            Debug.LogError($"{gameObject.name}: PathFinder instance를 찾을 수 없습니다!");
        }

        ChangeState(explore);
    }

    protected virtual void Update()
    {
        if (Time.time - stat.LastAttackTime < stat.AttackMotionDelay)
        {
            rigid.linearVelocity = Vector2.zero;
            return;
        }

        scanner.ExploreTiles();

        if (state != null)
        {
            state.Execute();
        }
    }

    #endregion

    #region 4. 오버라이딩

    public abstract void AttackAction();
    public abstract void OnCombatBehaviour();

    #endregion

    #region 5. 함수

    // 상태 변경
    public void ChangeState(EnemyState newState)
    {
        if (state != null) state.Exit();
        state = newState;
        state.Enter();
    }

    // 사망 로직
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

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

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

        Destroy(gameObject);
    }

    // 애니메이션
    public void OnAnimAttackHit()
    {
        AttackAction();
    }

    public void LookAt(Vector3 goal)
    {
        if (goal.x > transform.position.x)
        {
            sr.flipX = false;
        }
        else if (goal.x < transform.position.x)
        {
            sr.flipX = true;
        }
    }

    // 이동 로직
    public void MoveToDestination()
    {
        if (currentPath == null || currentPath.Count == 0) return;

        Vector2 targetPos = currentPath.First.Value;
        float speed = stat.MoveSpeed;

        LookAt(targetPos);

        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector2.Distance(rigid.position, targetPos) < 0.1f)
        {
            currentPath.RemoveFirst();
            if (currentPath.Count == 0)
            {
                ArrivingTarget(Vector2Int.RoundToInt(targetPos));
            }
        }
    }


    // 타겟 로직
    private void ArrivingTarget(Vector2Int pos)
    {
        if (scanner.nearestTarget == null || !scanner.nearestTarget.gameObject.activeSelf)
        {
            scanner.nearestTarget = null;
            currentPath = null;
            return;
        }

        if (Vector2Int.RoundToInt(transform.position) == Vector2Int.RoundToInt(scanner.nearestTarget.position))
        {
            // TODO 시설에게 도착했다는 신호 보내기
            scanner.nearestTarget = null;
            currentPath = null;
            return;
        }
    }

    public bool IsTargetActive()
    {
        if (scanner.nearestTarget == null || !scanner.nearestTarget.gameObject.activeSelf) return false;
        return true;
    }

    // 경로 로직
    public bool HasPath
    {
        get
        {
            if (currentPath == null || currentPath.Count == 0) return false;
            return true;
        }
    }

    #endregion
}
