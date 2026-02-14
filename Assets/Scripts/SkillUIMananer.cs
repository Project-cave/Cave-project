using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillUIMananer : MonoBehaviour
{
    GameObject Skill;
    GameObject Dungeon;
    GameObject DemonSkill;

    public void OnLoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

}
