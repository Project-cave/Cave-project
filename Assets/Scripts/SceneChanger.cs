using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void LoadSkillScene()
    {
        SceneManager.LoadScene("Skill");
    }

    public void LoadBattleScene()
    {
        PlayerManager.instance.Save();
        SceneManager.LoadScene("GameScene");
    }

    public void LoadMonsterScene()
    {
        
        SceneManager.LoadScene("Monster");
    }
}
