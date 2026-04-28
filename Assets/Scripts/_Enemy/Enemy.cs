using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    #region 1. 설정값
    [Header("Reference")]
    SpriteRenderer sr;
    BoxCollider2D col;
    Transform healthBarTransform;
    public Animator anim;
    public Rigidbody2D rigid;
    public Scanner scanner;
    public EnemyStatHandler stat;
    public PathFinder pathFinder;

    #endregion

    #region 2. 변수

    // 전투 관련
    [HideInInspector] public bool isCriticalContext;
    [HideInInspector] public bool isFleeing = false;

    // 이동 관련
    public LinkedList<Vector2> currentPath = new LinkedList<Vector2>();
    private float lastExploreTime = 0f;

    // 상태 머신
    EnemyState state;
    public ExploreState explore;
    public ChaseState chase;
    public AttackState attack;
    public InteractState interact;

    // 임시 종족 구분용 텍스트
    public TMP_Text raceText;

    // 이모티콘 세팅
    [HideInInspector] public Transform lastDetectedTarget = null;
    private SpriteRenderer emoticonSR;
    private Sprite detectionSprite;
    private Sprite painSprite;
    private Coroutine emoticonCoroutine;

    #endregion

    #region 3. 이벤트

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        scanner = GetComponent<Scanner>();
        stat = GetComponent<EnemyStatHandler>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        healthBarTransform = GetComponentInChildren<HealthBar>().transform;

        if (stat != null)
        {
            stat.OnDeath += Death;
            stat.OnHit += HandleHit;
        }

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
    }

    protected virtual void Update()
    {
        if (stat.isDead) return;

        if (Time.time - stat.LastAttackTime < stat.AttackMotionDelay)
        {
            rigid.linearVelocity = Vector2.zero;
            return;
        }

        if (Time.time - lastExploreTime > 0.2f)
        {
            scanner.ExploreTiles();
            lastExploreTime = Time.time;
        }

        if (state != null)
        {
            state.Execute();
        }
    }

    protected virtual void OnDestroy()
    {
        if (stat != null)
        {
            stat.OnDeath -= Death;
            stat.OnHit -= HandleHit;
        }
    }

    #endregion

    #region 4. 오버라이딩

    public abstract void AttackAction();
    public abstract void OnCombatBehaviour();

    #endregion

    #region 5. 함수

    // 초기화
    public void InitEnemy()
    {
        if (raceText == null)
        {
            raceText = GetComponentInChildren<TMP_Text>();
        }

        GetComponent<Collider2D>().enabled = true;
        rigid.bodyType = RigidbodyType2D.Dynamic;

        if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }

        if (stat != null)
        {
            stat.InitializeStats();
            if (scanner != null)
            {
                scanner.attackRange = stat.AttackRange;
                if (stat.classData.attackType == EnemyAttackType.Melee) scanner.scanRange = 3.0f;
                else scanner.scanRange = stat.AttackRange;

                scanner.nearestTarget = null;
            }

            if (sr != null && stat.classData != null && stat.classData.classShape != null)
            {
                sr.sprite = stat.classData.classShape;
            }

            if (raceText != null && stat.raceData != null)
            {
                raceText.text = stat.raceData.raceType.ToString();
            }

            if (anim != null && stat.classData != null && stat.raceData != null)
            {
                foreach (RaceInfo info in stat.classData.raceInfo)
                {
                    if (info.race == stat.raceData.raceType)
                    {
                        anim.runtimeAnimatorController = info.animController;
                        anim.Update(0f);
                        break;
                    }
                }
            }
        }

        currentPath?.Clear();

        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }

        if (sr != null && col != null && sr.sprite != null)
        {
            col.size = sr.sprite.bounds.size;
            col.offset = sr.sprite.bounds.center;
        }
        if (sr != null && healthBarTransform != null)
        {
            float topOfHead = sr.bounds.extents.y;
            healthBarTransform.localPosition = new Vector3(0, topOfHead + 0.2f, 0);
        }

        Transform emoTransform = transform.Find("Emoticon");
        if (emoTransform != null)
        {
            emoticonSR = emoTransform.GetComponent<SpriteRenderer>();

            detectionSprite = Resources.Load<Sprite>("Emoticon/Detection");
            painSprite = Resources.Load<Sprite>("Emoticon/Pain");

            if (sr != null && healthBarTransform != null)
            {
                float topOfHead = sr.bounds.extents.y;
                emoticonSR.transform.localPosition = new Vector3(0, topOfHead + 0.6f, 0);
                emoticonSR.gameObject.SetActive(false);
            }
        }

        ChangeState(explore);
    }

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
        float speed = stat.CurrentMoveSpeed;

        LookAt(targetPos);

        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector2.Distance(rigid.position, targetPos) < 0.1f)
        {
            currentPath.RemoveFirst();
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

    public virtual void HandleHit(Transform attacker)
    {
        ShowEmoticon("Pain");

        if (attacker == null) return;

        int attackerPriority = int.MaxValue;
        int currentPriority = int.MaxValue;

        Targetable attackTargetable = attacker.GetComponent<Targetable>();
        if (attackTargetable != null) { attackerPriority = attackTargetable.priority; }

        if (scanner.aggroTarget == attacker)
        {
            scanner.lastAggroTime = Time.time;
            return;
        }

        Targetable currentTargetable = (scanner.nearestTarget != null) ? scanner.nearestTarget.GetComponent<Targetable>() : null;
        if (currentTargetable != null) { currentPriority = currentTargetable.priority; }

        if (scanner.nearestTarget == null || currentPriority > attackerPriority)
        {
            scanner.aggroTarget = attacker;
            scanner.lastAggroTime = Time.time;
            scanner.nearestTarget = attacker;
            ChangeState(chase);
        }
    }

    public void ShowEmoticon(string type)
    {
        if (emoticonSR == null) return;

        if (emoticonCoroutine != null) StopCoroutine(emoticonCoroutine);

        if (type == "Detection" && detectionSprite != null)
        {
            emoticonSR.sprite = detectionSprite;
            emoticonSR.transform.localScale = new Vector3(48f / 56f, 40f / 45f, 1f);
        }
        else if (type == "Pain" && painSprite != null)
        {
            emoticonSR.sprite = painSprite;
            emoticonSR.transform.localScale = new Vector3(48f / 64f, 48f / 64f, 1f);
        }

        emoticonCoroutine = StartCoroutine(EmoticonRoutine());
    }

    private System.Collections.IEnumerator EmoticonRoutine()
    {
        Color c = emoticonSR.color;
        c.a = 1f;
        emoticonSR.color = c;
        emoticonSR.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        float fadeDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            emoticonSR.color = c;

            yield return null;
        }
        emoticonSR.gameObject.SetActive(false);
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (scanner != null && scanner.Explored != null)
        {
            Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
            foreach (Vector3Int tilePos in scanner.Explored)
            {
                Vector3 center = new Vector3(tilePos.x + 0.5f, tilePos.y + 0.5f, 0f);
                Gizmos.DrawCube(center, new Vector3(1f, 1f, 0.1f));
            }
        }

        if (currentPath != null && currentPath.Count > 0)
        {
            Gizmos.color = Color.yellow;
            Vector2 previousPos = transform.position;

            foreach (Vector2 pathNode in currentPath)
            {
                Gizmos.DrawSphere(pathNode, 0.1f);
                Gizmos.DrawLine(previousPos, pathNode);
                previousPos = pathNode;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(previousPos, 0.3f);
        }

        if (scanner != null && scanner.attackTarget != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, scanner.attackTarget.position);
        }
    }
#endif
}