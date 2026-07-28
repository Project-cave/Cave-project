using UnityEngine;

public class TempInit : MonoBehaviour
{
    Enemy enemy;
    Unit unit;

    private void Awake()
    {
        enemy = GetComponentInChildren<Enemy>();
        unit = GetComponent<Unit>();
    }

    private void Start()
    {
        if (enemy != null)
        {
            enemy.InitEnemy();
        }
        if (unit != null)
        {

            unit.InitUnit(unit.unitData);
        }
    }
}