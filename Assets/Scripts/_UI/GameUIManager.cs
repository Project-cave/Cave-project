using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] Button MonsterBtn;
    [SerializeField] Button SkillBtn;

    private void Start()
    {
        MonsterBtn.onClick.AddListener(() => SceneController.instance.LoadMonsterScene());
        SkillBtn.onClick.AddListener(() => SceneController.instance.LoadSkillScene());
    }
}
