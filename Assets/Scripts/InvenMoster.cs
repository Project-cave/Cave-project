using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvenMonster : MonoBehaviour
{
    public UnitSo unitData;
    public bool Lock;
    public RectTransform rectTransform;


    private void Start()
    {
        rectTransform = GetComponentsInChildren<RectTransform>()[1];
    }

    public void OnClickRequestBtn()
    {
        MonsterPopupManager.instance.OpenPopup(this);
    }

}
