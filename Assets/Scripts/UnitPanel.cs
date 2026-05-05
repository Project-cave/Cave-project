using UnityEngine;

public class UnitPanel : MonoBehaviour
{
    [SerializeField] private UnitPanelItem[] unitItems; // 전체 유닛 패널 아이템

    private void Start()
    {
        RefreshPanel();
    }

    private void RefreshPanel()
    {
        foreach (var item in unitItems)
        {
            item.Refresh();
        }
    }
}
