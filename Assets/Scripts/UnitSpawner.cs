using System;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public UnitSo[] unitData;

    public void SpawnSword()
    {
        GameObject select = GameManager.instance.pool.Get(0);   
        select.GetComponent<Unit>().InitUnit(unitData[0]);
        if (select != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySfx(0);
        }
    }


    public void SpawnWizard()
    {
        GameObject select = GameManager.instance.pool.Get(0);
        select.GetComponent<Unit>().InitUnit(unitData[1]);
        if (select != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySfx(0);
        }
    }
}
