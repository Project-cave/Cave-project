using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;

    [SerializeField] Button MonsterBtn;
    [SerializeField] Button SkillBtn;
    [SerializeField] GameObject UnitSpawnPanel;

    public void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        MonsterBtn.onClick.AddListener(() => SceneController.instance.LoadMonsterScene());
        SkillBtn.onClick.AddListener(() => SceneController.instance.LoadSkillScene());
    }

    public void ToggleUnitSpawnPanel()
    {
        UnitSpawnPanel.SetActive(!UnitSpawnPanel.activeSelf);
    }

    public void DisableSceneButton()
    {
        SkillBtn.interactable = false;
        MonsterBtn.interactable = false;
    }

    public void AbleSceneButton()
    {
        SkillBtn.interactable = true;
        MonsterBtn.interactable = true;
    }
}
