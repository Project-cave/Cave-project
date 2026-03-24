using UnityEngine;

public class Targetable : MonoBehaviour
{
    public int priority = 10;

    public bool IsActive => gameObject.activeInHierarchy;
}