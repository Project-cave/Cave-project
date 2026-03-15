using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Object")]
    public GameObject spawnUnit;
    public PoolManager pool;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        AudioManager.instance.PlayBgm(AudioManager.Bgm.Battle, true);
    }

    public void SpawnUnit(GameObject spawnUnit)
    {
        this.spawnUnit = spawnUnit;
    }
}
