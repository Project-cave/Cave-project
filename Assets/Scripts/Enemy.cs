using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum Status { Idle, Attack, Run, None }
    public Status status;
    public bool isMoving = false;
    public bool isAttack = false;
    public Rigidbody2D target;
    public float speed;
    
    float distance = 10000;
    Rigidbody2D rigid;
    Scanner scanner;

    // Start is called before the first frame update
    void Awake()
    {
        status = Status.Idle;
        rigid = GetComponent<Rigidbody2D>();
        scanner = GetComponent<Scanner>();
    }

    private void Update()
    {
        
    }


    // Update is called once per frame
    void FixedUpdate()
    {


        //if (status == Status.Idle)
        //{

        //}
        //else if (status == Status.Attack)
        //{

        //}
        //else if (status == Status.Run)
        //{

        //}

        if (!scanner.nearsetTarget)
            return;

        target = scanner.nearsetTarget.GetComponent<Rigidbody2D>();
        Vector2 dirVec;
        Vector2 nextVec;

        dirVec = target.position - rigid.position;
        distance = dirVec.magnitude;
        nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        if(distance > scanner.AttackRange)
        {
            rigid.MovePosition(rigid.position + nextVec);
        }
    }

}
