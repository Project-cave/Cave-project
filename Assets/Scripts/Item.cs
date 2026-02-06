using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData data;
    public Weapon weapon;

    public void OnClick()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Range:
                GameObject newWeapon = new GameObject();
                weapon = newWeapon.AddComponent<Weapon>();
                weapon.Init(data);
                break;
        }
    }
}
