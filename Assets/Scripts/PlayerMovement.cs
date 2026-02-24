using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class PlayerMovement : MonoBehaviour
{    
    enum Status { Attack, Move, idle }

    [SerializeField] float m_movementSpeed = 10;
    public float Movement { set => m_movementSpeed = value; }
    [SerializeField] float arrivalThreshold = 0.2f;
      
    Status status = Status.idle;
    Vector2 m_start;
    Vector2 m_goal;

    Vector2Int m_startCenter;
    Vector2 m_goalCenter;
    LinkedList<Vector2> m_fasterPath = new LinkedList<Vector2>();

    Animator anim;
    PathFinder m_pathFinder;
    SpriteRenderer sr;
    Scanner scanner;
    Weapon weapon;
    BoxCollider2D boxCollider;

    // 스탯 관리
    StatHandler stat;

    public bool moveable = false;
    public bool tileCenterMode = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        sr = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        // StatHandler 가져오기
        stat = GetComponent<StatHandler>();

        if (boxCollider == null)
        {
            Debug.LogWarning($"{gameObject.name}: BoxCollider2D가 없습니다!");
        }
    }

    private void Start()
    {
        m_pathFinder = PathFinder.instance;
        if (m_pathFinder == null)
        {
            Debug.LogError($"{gameObject.name}: PathFinder instance를 찾을 수 없습니다!");
        }
    }

    public void SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public void MoveToPosition(Vector3 targetPosition)
    {
        if (!moveable)
        {
            Debug.LogWarning($"[{gameObject.name}] moveable이 false입니다!");
            return;
        }

        if (m_pathFinder == null)
        {
            Debug.LogError($"[{gameObject.name}] PathFinder가 null입니다!");
            return;
        }

        m_start = transform.position;

        if (tileCenterMode) m_goal = new Vector2((int)(targetPosition.x) + 0.5f,(int)(targetPosition.y) + 0.5f);
        else m_goal = targetPosition;
       
        ChangerStatus(Status.Move);

        m_fasterPath.Clear();

        if (boxCollider != null)
        {
            m_fasterPath = m_pathFinder.getShortestPath(m_start, m_goal, boxCollider);
        }
        else
        {
            m_fasterPath = m_pathFinder.getShortestPath(m_start, m_goal, new Vector2(1f, 1f));
        }
    }

    void Update()
    {
        if (m_fasterPath != null && m_fasterPath.Count > 0)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                m_fasterPath.First.Value,
                m_movementSpeed * Time.deltaTime
            );

            if (m_fasterPath.First.Value.x >= transform.position.x)
            {
                sr.flipX = false;
            }
            else
            {
                sr.flipX = true;
            }

            if (Vector2.Distance(transform.position, m_fasterPath.First.Value) < arrivalThreshold)
            {
                m_fasterPath.RemoveFirst();
            }
        }

        if (m_fasterPath.Count == 0 && status == Status.Move)
        {
            if (scanner != null && scanner.inAttackRange)
            {
                ChangerStatus(Status.Attack);
            }
            else
            {
                ChangerStatus(Status.idle);
            }
                
        }

        if (status == Status.Attack)
        {
            if (scanner != null && scanner.attackTarget != null)
            {
                if (scanner.attackTarget.position.x >= transform.position.x)
                {
                    if (weapon != null)
                        weapon.transform.localPosition = new Vector3(0.5f, 0, 0);
                    sr.flipX = false;
                }
                else if (scanner.attackTarget.position.x < transform.position.x)
                {
                    if (weapon != null)
                        weapon.transform.localPosition = new Vector3(-0.5f, 0, 0);
                    sr.flipX = true;
                }
            }
        }
    }
    
    void ChangerStatus(Status newStatus)
    {
        if (status == newStatus) return;

        status = newStatus;

        anim.SetBool("Run", status == Status.Move);
        anim.SetBool("Attack", status == Status.Attack);
    }



    public void WizardAttack()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Masic);
        if (weapon != null)
            weapon.Fire();
    }

    public void SwordAttack()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Sword);
    }


    // 사망 처리 함수
    public void Death()
    {
        // 사망 로직
    }



    private void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying && m_fasterPath != null)
        {
            Color originalColor = Gizmos.color;

            if (m_fasterPath.Count > 0)
            {
                Gizmos.color = Color.green;

                foreach (var loc in m_fasterPath)
                    Gizmos.DrawCube(new Vector3(loc.x, loc.y, 0), new Vector3(0.5f, 0.5f, 0.5f));

                Gizmos.DrawLine(transform.position, m_fasterPath.First.Value);

                for (LinkedListNode<Vector2> iter = m_fasterPath.First; iter.Next != null; iter = iter.Next)
                {
                    Vector3 from = iter.Value;
                    Vector3 to = iter.Next.Value;
                    Gizmos.DrawLine(from, to);
                }
            }

            Gizmos.color = originalColor;
        }
    }
}