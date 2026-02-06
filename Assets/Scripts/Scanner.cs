using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange;
    public float AttackRange;
    public LayerMask targetLayer;
    public RaycastHit2D[] targets;
    public Transform nearsetTarget;
    public Transform AttackTarget;
    PlayerMovement player;
    public bool inAttackRange;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
    }


    private void FixedUpdate()
    {
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearsetTarget = GetNearest();
        if(nearsetTarget != null )
            AttackTarget = GetAttackTarget();       
    }

    Transform GetNearest()
    {
        Transform result = null;
        float diff = 100;

        foreach(RaycastHit2D target in targets)
        {
            Vector3 mypos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(mypos, targetPos);

            if(curDiff < diff)
            {
                diff = curDiff;
                result = target.transform;
            }
        }
        return result;
    }

    Transform GetAttackTarget()
    {
        Transform result = null;
  
        Vector3 mypos = transform.position;
        Vector3 targetPos = nearsetTarget.position;
        float curDiff = Vector3.Distance(mypos,targetPos);

        if (curDiff < AttackRange)
        {
            inAttackRange = true;
            result = nearsetTarget;
        }
        else
        {
            inAttackRange = false;
        }

            return result;
    }

    
}
