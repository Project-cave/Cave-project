using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvenMonster : MonoBehaviour
{
    public UnitSo unitData;
    public bool Lock;
    private RectTransform rectTransform;


    private void Start()
    {
        rectTransform = GetComponentsInChildren<RectTransform>()[1];
        Lock = true;
    }

    public void OnClickRequestBtn()
    {     
        PopupManager.instance.OpenPopup(unitData, rectTransform, true);
    }

}
