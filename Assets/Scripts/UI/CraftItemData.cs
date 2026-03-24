using UnityEngine;

[CreateAssetMenu(fileName = "NewCraftItem", menuName = "Tower Defense/Craft Item Data")]
public class CraftItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public string description;
}
