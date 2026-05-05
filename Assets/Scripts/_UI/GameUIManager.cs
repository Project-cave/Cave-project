using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;

    [SerializeField] Button MonsterBtn;
    [SerializeField] Button SkillBtn;
    [SerializeField] GameObject UnitSpawnPanel;

    //[Header("Monster Spawn")]
    //[SerializeField] GameObject MonsterPanel;
    //[SerializeField] GameObject content;

    public void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        MonsterBtn.onClick.AddListener(() => SceneController.instance.LoadMonsterScene());
        SkillBtn.onClick.AddListener(() => SceneController.instance.LoadSkillScene());
        //if (SceneController.instance.gameMonsterPanel != null)
        //{
        //    var monsterData = SceneController.instance.gameMonsterPanel;
        //    int index = 0;
        //    MonsterPanel.SetActive(true);

        //    foreach (var monsterSpawner in FacilityPlacementManager.Instance.placedFacilities)
        //    {
        //        if (monsterSpawner.Value == monsterData)
        //        {
        //            content.transform.GetChild(index).gameObject.SetActive(true);
        //            index++;
        //        }
        //    }

        //    SceneController.instance.gameMonsterPanel = null;
        //}

    }

    public void ToggleUnitSpawnPanel()
    {
        UnitSpawnPanel.SetActive(!UnitSpawnPanel.activeSelf);
    }
}
