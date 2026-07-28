using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    //private void Awake()
    //{
    //    animator = GetComponent<Animator>();
    //    spriteRenderer = GetComponent<SpriteRenderer>();

    //    if (animator == null) Debug.Log("Animator ����");
    //    if (spriteRenderer == null) Debug.Log("SR ����");
    //}

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null) Debug.Log("Animator ����");
        if (spriteRenderer == null) Debug.Log("SR ����");
    }

    public void SetAnimator(RuntimeAnimatorController controller)
    {
        if (animator != null)
        {
            animator.runtimeAnimatorController = controller;
        }
    }

    public void PlayIdle()
    {
        if (animator == null) { Debug.LogError("animator null! " + gameObject.name); return; }
        animator.SetBool("Run", false);
    }

    public void PlayMove()
    {
        animator.SetBool("Run", true);
    }

    public void PlayAttack()
    {
        animator.SetBool("Run", false);
    }

    public void PlayAttackMotion()
    {
        animator.SetTrigger("AttackTrigger");
    }

    // === ��������Ʈ ���� (�̵�) ===
    public void FaceDirection(Vector2 direction)
    {
        if (direction.x > 0)
            spriteRenderer.flipX = true;
        else if (direction.x < 0)
            spriteRenderer.flipX = false;
    }

    // === ��������Ʈ ���� (����) ===
    public void FaceTarget(Transform target)
    {
        if (target == null) return;

        if (target.position.x >= transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;
    }
}
