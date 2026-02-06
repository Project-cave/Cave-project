using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float m_movementSpeed = 10;

    Vector2 m_start;
    Vector2 m_goal;
    //LinkedList<Vector2> path = null;
    LinkedList<Vector2> m_fasterPath = new();

    public enum Status { Attack, Move, idle }
    public Status status = Status.idle;

    BoxCollider2D m_boxCollider;
    Animator anim;
    PathFinder m_pathFinder; 
    SpriteRenderer sr;
    public Scanner scanner;
    Weapon weapon;

    
    public bool moveable = false;

    private void Awake()
    {
        m_pathFinder = GetComponent<PathFinder>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        sr = GetComponent<SpriteRenderer>();
        weapon = GetComponentInChildren<Weapon>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && moveable)
        {
            
            m_start = transform.position;
            m_goal = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            status = Status.Move;
            

            long jpsElapsedMS;
            long jpsElapsedTick;
            m_fasterPath.Clear();
            {
                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                {
                    m_fasterPath = m_pathFinder.getShortestPath(m_start, m_goal);
                }
                watch.Stop();
                jpsElapsedMS = watch.ElapsedMilliseconds;
                jpsElapsedTick = watch.ElapsedTicks;

                Debug.Log(
                    "JPS: " + jpsElapsedMS + "ms" + "(" + jpsElapsedTick + "ticks)"
                );
            }
        }
        
        if (m_fasterPath != null && m_fasterPath.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, m_fasterPath.First.Value, m_movementSpeed * Time.deltaTime);
            if (m_fasterPath.First.Value.x >= transform.position.x)
            {
                sr.flipX = false;
            }
            else
            {
                sr.flipX = true;
            }
            if ((Vector2)transform.position == m_fasterPath.First.Value)
            {
                m_fasterPath.RemoveFirst();
            }
        }
        if (m_fasterPath.Count == 0 && status != Status.Attack)
        {
            if (scanner.inAttackRange)
                status = Status.Attack;
            else
                status = Status.idle;
        }

        if (status == Status.Attack)
        {
            if(scanner.AttackTarget.position.x >= transform.position.x)
            {
                if(weapon != null)
                    weapon.transform.localPosition = new Vector3(0.5f, 0, 0);
                sr.flipX = false;
            }
            else if(scanner.AttackTarget.position.x < transform.position.x)
            {
                if(weapon != null)
                    weapon.transform.localPosition = new Vector3(-0.5f, 0, 0);
                sr.flipX = true;
            }
        }
    }


    private void LateUpdate()
    {
        if (status == Status.idle)
        {
            anim.SetBool("IdleBool", true);
            anim.SetBool("RunBool",false);
            anim.SetBool("AttackBool", false);
        }
        if(status == Status.Move)
        {
            anim.SetBool("IdleBool", false);
            anim.SetBool("RunBool", true);
            anim.SetBool("AttackBool", false);
        }
        if(status == Status.Attack)
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
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Masic);
        weapon.Fire();
    }

    public void SwordAttack()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Sword);
    }
    


    private void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying)
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
