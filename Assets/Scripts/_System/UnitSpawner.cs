using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public static UnitSpawner instance;
    [SerializeField] private UnitPanel unitPanel;
    public Vector2 spawnPosition;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void OpenPanel(Vector2 pos)
    {
        spawnPosition = pos;
        GameUIManager.instance.ToggleUnitSpawnPanel();
    }

    public void Spawn(UnitSo unitData)
    {
        int index = unitData.info.unitNum;
        GameObject select = GameManager.instance.pool.Get(index);
        GameManager.instance.spawnUnit = select;
        select.transform.position = new Vector2(21.5f, -13.2f);
        select.GetComponent<Unit>().InitUnit(unitData);

        if (select != null && AudioManager.instance != null)
            AudioManager.instance.PlaySfx(0);
    }
}
