using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null) Debug.Log("Animator 실종");
        if (spriteRenderer == null) Debug.Log("SR 실종");
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

    // === 스프라이트 반전 (이동) ===
    public void FaceDirection(Vector2 direction)
    {
        if (direction.x > 0)
            spriteRenderer.flipX = false;
        else if (direction.x < 0)
            spriteRenderer.flipX = true;
    }

    // === 스프라이트 반전 (공격) ===
    public void FaceTarget(Transform target)
    {
        if (target == null) return;

        if (target.position.x >= transform.position.x)
            spriteRenderer.flipX = false;
        else
            spriteRenderer.flipX = true;
    }
}
