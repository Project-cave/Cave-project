using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("# Game Control")]
    public bool isLive;
    //public float gameTime;

    //[Header("# player Info")]
    //public int playerId;

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

    public void SpawnUnit(GameObject spawnUnit)
    {
        this.spawnUnit = spawnUnit;
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayBgm(AudioManager.Bgm.Battle ,true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
