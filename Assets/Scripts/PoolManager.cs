using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    public GameObject[] prefebs;
    List<GameObject>[] pools;

    // Start is called before the first frame update
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
                break;
            }
        }

        if (!select)
        {
            select = Instantiate(prefebs[index], transform);
            pools[index].Add(select);
        }

        if (index == 0)
        {
            GameManager.instance.spawnUnit = select;
            GameManager.instance.player = select.GetComponent<PlayerMovement>();
        }
            

        return select;
    }

    public void Run()
    {
        Get(0);
        AudioManager.instance.PlaySfx(0);
    }

    public void SpawnSword()
    {
        Get(1);
        AudioManager.instance.PlaySfx(0);
    }

    public void EnemySpawn()
    {
        Get(2);
        AudioManager.instance.PlaySfx(0);
    }

}
