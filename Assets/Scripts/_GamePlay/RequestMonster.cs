using UnityEngine;
using UnityEngine.UI;

public class RequestMonster : MonoBehaviour
{
    public UnitSo unitData;
    public GameObject[] connectLines;
    public Sprite solidLine;
    public RectTransform rectTransform;
    [SerializeField] private Button[] nextBtns;

    private void Start()
    {
        rectTransform = GetComponentsInChildren<RectTransform>()[1];
    }

    public void OnClickRequestBtn()
    {
        MonsterPopupManager.instance.OpenPopup(this);
    }

    public void OnUnlock()
    {
        foreach (var line in connectLines)
            line.GetComponent<Image>().sprite = solidLine;
        OnClickRequest();
    }

    public void OnClickRequest()
    {
        if(!UnitUnlockController.instance.IsUnlocked(unitData))
            UnitUnlockController.instance.Unlock(unitData);

        if (nextBtns == null) return;

        foreach(var btn in nextBtns)
        {
            btn.interactable = true;
        }
    }
}
