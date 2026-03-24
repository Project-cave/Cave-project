using System;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public UnitSo[] unitData;

    public void SpawnSword()
    {
        GameObject select = GameManager.instance.pool.Get(0);
        UnitStatHandler statHandler = select.GetComponent<UnitStatHandler>();
        if (statHandler != null)
        {
            statHandler.unitData = unitData[0];
        }
        select.GetComponent<Unit>().InitUnit();
        if (select != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySfx(0);
        }
    }


    public void SpawnWizard()
    {
        GameObject select = GameManager.instance.pool.Get(0);
        UnitStatHandler statHandler = select.GetComponent<UnitStatHandler>();
        if (statHandler != null)
        {
            statHandler.unitData = unitData[1];
        }
        select.GetComponent<Unit>().InitUnit();
        if (select != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySfx(0);
        }
    }
}