using UnityEngine;
using UnityEngine.UI;

public class UnitPanelItem : MonoBehaviour
{
    public UnitSo unitData;
    private Button btn;

    public void Refresh()
    {
        if (btn == null)
        {
            btn = GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
                UnitSpawner.instance.Spawn(unitData)
            );
        }
        if (unitData == null) return;
        btn.interactable = UnitUnlockController.instance.IsUnlocked(unitData);
    }

}
