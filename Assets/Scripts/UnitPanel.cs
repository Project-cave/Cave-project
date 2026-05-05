using UnityEngine;

public class UnitPanel : MonoBehaviour
{
    [SerializeField] private UnitPanelItem[] unitItems;

    private void OnEnable()
    {
        RefreshPanel();
    }

    public void RefreshPanel()
    {
        foreach (var item in unitItems)
            item.Refresh();
    }
}
