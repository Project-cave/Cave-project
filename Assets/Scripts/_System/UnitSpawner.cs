using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public UnitSo[] unitData;

    public void SpawnSword()
    {
        GameObject select = GameManager.instance.pool.Get(0);
        GameManager.instance.spawnUnit = select;
        select.transform.position = new Vector2(21.5f, -13.2f);
        select.GetComponent<Unit>().InitUnit(unitData[0]);
        if (select != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySfx(0);
        }
    }


    public void SpawnWizard()
    {
        GameObject select = GameManager.instance.pool.Get(0);
        GameManager.instance.spawnUnit = select;
        select.transform.position = new Vector2(9.5f, -13.2f);
        select.GetComponent<Unit>().InitUnit(unitData[1]);
        if (select != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySfx(0);
        }
    }
}
