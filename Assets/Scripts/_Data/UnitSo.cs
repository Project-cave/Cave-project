using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Scriptable Object/unit")]
public class UnitSo : ScriptableObject
{
    public enum Race { Goblin, Undead }
    public enum Rank { Normal }

    [Header("유닛 데이터")]
    public string unitName;
    public int unitNum;
    public Race race;
    public Rank rank;
    public int health;
    public bool isRanged;
    public int Damage;
    public int count;
    public int range;
    public float damageMult;
    public int bulletSpeed;
    public float speed;
    //public int defand;
    // 특수 기믹 안넣음

    [TextArea]
    public string unitDesc;

    [Header("해금 조건")]
    public int request;

    [TextArea]
    public string requestDesc;

    [Header("생산 조건")]
    public int material;

    [TextArea]
    public string materialDesc;

    public RuntimeAnimatorController animController;
    public GameObject DebuffPrefab;

}