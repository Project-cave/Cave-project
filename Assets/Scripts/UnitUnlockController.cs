using System.Collections.Generic;
using UnityEngine;

public class UnitUnlockController : MonoBehaviour
{
    public static UnitUnlockController instance;

    private HashSet<UnitSo> unlockedUnits = new HashSet<UnitSo>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Unlock(UnitSo unit)
    {
        Debug.Log(unit.ToString());
        unlockedUnits.Add(unit);
    }

    public bool IsUnlocked(UnitSo unit)
    {
        return unlockedUnits.Contains(unit);
    }
}
