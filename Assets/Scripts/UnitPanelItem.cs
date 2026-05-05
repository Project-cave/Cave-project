using UnityEngine;
using UnityEngine.UI;

public class UnitPanelItem : MonoBehaviour
{
    public UnitSo unitData;
    private Button btn;
    [SerializeField] private UnitSpawner spawner;

    private void Awake()
    {
        btn = GetComponent<Button>();
    }

    private void Start()
    {
        btn.onClick.AddListener(() =>
            spawner.Spawn(unitData)
        );
    }

    public void Refresh()
    {
        btn.interactable = UnitUnlockController.instance.IsUnlocked(unitData);
    }

}
