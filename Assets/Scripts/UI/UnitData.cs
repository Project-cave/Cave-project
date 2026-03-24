using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "Tower Defense/Unit Data")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public Sprite unitSprite;
    public GameObject unitPrefab;
    public int cost;
}
