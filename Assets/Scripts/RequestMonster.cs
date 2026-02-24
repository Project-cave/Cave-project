using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequestMonster : MonoBehaviour
{
    public UnitSo unitData;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponentsInChildren<RectTransform>()[1];
    }

    public void OnClickRequestBtn()
    {
        PopupManager.instance.OpenPopup(unitData, rectTransform, false);
    }

}
