using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] prefebs;
    public List<GameObject>[] pools;

    void Start()
    {
        pools = new List<GameObject>[prefebs.Length];
        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;
        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                return select;
            }
        }

        if (!select)
        {
            select = Instantiate(prefebs[index], transform);
            pools[index].Add(select);
        }

        GameManager.instance.SpawnUnit(select);
        return select;
    }

    // 임시 코드
    public void EnemySpawn()
    {
        GameObject spawned = Get(1);
        if (spawned != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySfx(0);
        }
    }
}
