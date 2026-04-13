using UnityEngine;

public class Targetable : MonoBehaviour
{
    public int priority = 10;

    [Header("건물 설정")]
    public bool isIndestructible = false;
    public EnemyRankType requiredRank = EnemyRankType.Bronze;

    public bool IsActive => gameObject.activeInHierarchy;
}