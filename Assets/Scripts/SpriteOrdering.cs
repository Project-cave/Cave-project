using UnityEngine;

public class SpriteOrdering : MonoBehaviour
{
    SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        sr.sortingOrder = Mathf.RoundToInt(1000 + (-29 - transform.position.y) * 100);
    }
}