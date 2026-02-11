using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float m_movementSpeed = 10;
    [SerializeField] float arrivalThreshold = 0.2f;

    Vector2 m_start;
    Vector2 m_goal;
    LinkedList<Vector2> m_fasterPath = new LinkedList<Vector2>();

    public enum Status { Attack, Move, idle }
    public Status status = Status.idle;

    Animator anim;
    PathFinder m_pathFinder;
    SpriteRenderer sr;
    public Scanner scanner;
    Weapon weapon;
    BoxCollider2D boxCollider;

    public bool moveable = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        sr = GetComponent<SpriteRenderer>();
        weapon = GetComponentInChildren<Weapon>();
        boxCollider = GetComponent<BoxCollider2D>();

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
        m_goal = targetPosition;
        status = Status.Move;

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

        if (m_fasterPath.Count == 0 && status != Status.Attack)
        {
            if (scanner != null && scanner.inAttackRange)
                status = Status.Attack;
            else
                status = Status.idle;
        }

        if (status == Status.Attack)
        {
            if (scanner != null && scanner.AttackTarget != null)
            {
                if (scanner.AttackTarget.position.x >= transform.position.x)
                {
                    if (weapon != null)
                        weapon.transform.localPosition = new Vector3(0.5f, 0, 0);
                    sr.flipX = false;
                }
                else if (scanner.AttackTarget.position.x < transform.position.x)
                {
                    if (weapon != null)
                        weapon.transform.localPosition = new Vector3(-0.5f, 0, 0);
                    sr.flipX = true;
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (anim == null) return;

        if (status == Status.idle)
        {
            anim.SetBool("IdleBool", true);
            anim.SetBool("RunBool", false);
            anim.SetBool("AttackBool", false);
        }
        if (status == Status.Move)
        {
            anim.SetBool("IdleBool", false);
            anim.SetBool("RunBool", true);
            anim.SetBool("AttackBool", false);
        }
        if (status == Status.Attack)
        {
            anim.SetBool("IdleBool", false);
            anim.SetBool("RunBool", false);
            anim.SetBool("AttackBool", true);
        }
    }

    public void SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
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