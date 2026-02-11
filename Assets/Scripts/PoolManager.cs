using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] prefebs;
    List<GameObject>[] pools;

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
        if (index < 0 || index >= pools.Length)
        {
            Debug.LogError($"PoolManager: 잘못된 인덱스 {index}");
            return null;
        }

        if (prefebs[index] == null)
        {
            Debug.LogError($"PoolManager: prefebs[{index}]가 null입니다!");
            return null;
        }

        GameObject select = null;
        for (int i = pools[index].Count - 1; i >= 0; i--)
        {
            GameObject item = pools[index][i];

            // 파괴된 오브젝트는 리스트에서 제거
            if (item == null)
            {
                pools[index].RemoveAt(i);
                
                continue;
            }

            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        if (select == null)
        {
            select = Instantiate(prefebs[index], transform);         
            pools[index].Add(select);
        }

        if (index == 0 && GameManager.instance != null)
        {
            GameManager.instance.spawnUnit = select;
            GameManager.instance.player = select.GetComponent<PlayerMovement>();
        }

        return select;
    }

    public void Run()
    {
        GameObject spawned = Get(0);

        if (spawned != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySfx(0);
        }
        
    }

    public void SpawnSword()
    {
        GameObject spawned = Get(1);
        if (spawned != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySfx(0);
        }
    }

    public void EnemySpawn()
    {
        GameObject spawned = Get(2);
        if (spawned != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySfx(0);
        }
    }
}
