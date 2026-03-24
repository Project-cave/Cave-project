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
    public Button monsterBtn;
    public Button skillBtn;

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
        monsterBtn.onClick.AddListener(() => SceneChanger.instance.LoadMonsterScene());
        skillBtn.onClick.AddListener(() => SceneChanger.instance.LoadSkillScene());
    }

    public void SpawnUnit(GameObject spawnUnit)
    {
        this.spawnUnit = spawnUnit;
    }
}
