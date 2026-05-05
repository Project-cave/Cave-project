using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawnFacility : MonoBehaviour
{
    public void OnMouseDown()
    {
        UnitSpawner.instance.OpenPanel(transform.position);
    }
}
