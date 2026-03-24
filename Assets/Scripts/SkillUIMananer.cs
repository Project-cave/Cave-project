using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkillUIMananer : MonoBehaviour
{
    public Button CloseBtn;

    private void Start()
    {
        CloseBtn.onClick.AddListener(() => SceneChanger.instance.LoadBattleScene());
    }

}
