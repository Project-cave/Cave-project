using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "item",menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Melee, Range }

    [Header("# Main Info")]
    public ItemType itemType;
    public int itemId;

    [Header("# Data")]
    public float baseDamage;
    public int baseCount;

    [Header("# Weapon")]
    public GameObject prejectile;
}
