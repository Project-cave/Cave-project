using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerSaveData
{
    public int gold;
    public int level;
    public List<string> skillNames = new List<string>();
    public List<string> itemNames = new List<string>();
}
