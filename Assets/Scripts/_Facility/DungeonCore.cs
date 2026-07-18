using UnityEngine;

[RequireComponent(typeof(FacilityStatHandler))]
public class DungeonCore : MonoBehaviour
{
    private FacilityStatHandler statHandler;

    private void Awake()
    {
        statHandler = GetComponent<FacilityStatHandler>();
    }

    private void Start()
    {
        statHandler.OnDeath += TriggerGameOver;
    }

    private void OnDestroy()
    {
        if (statHandler != null) statHandler.OnDeath -= TriggerGameOver;
    }

    private void TriggerGameOver()
    {
        Debug.Log("[DungeonCore] 던전 코어 파괴");
        if (StageManager.instance != null)
        {
            StageManager.instance.OnWaveFailed();
        }
    }
}