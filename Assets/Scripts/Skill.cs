using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    public SkillSo skillData;
    Image Desc;
    TextMeshProUGUI text;

    private void Awake()
    {
        Desc = GetComponentsInChildren<Image>(true)[1];
        text = GetComponentInChildren<TextMeshProUGUI>(true);
        text.text = skillData.Desc;
    }

    public void OnDesc()
    {
        Desc.gameObject.SetActive(true);
    }

    public void OffDesc()
    {
        Desc.gameObject.SetActive(false);
    }

}
