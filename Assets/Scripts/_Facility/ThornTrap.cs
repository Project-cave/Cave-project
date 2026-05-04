using UnityEngine;

public class ThornTrap : MonoBehaviour
{
    [SerializeField] private int trapDamage = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStatHandler stat = other.GetComponent<EnemyStatHandler>();

        if (stat != null && !stat.isDead) stat.TakeDamage(trapDamage, null);
    }
}
