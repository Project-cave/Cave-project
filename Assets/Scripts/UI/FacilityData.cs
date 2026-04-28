using UnityEngine;

[CreateAssetMenu(fileName = "FacilityData", menuName = "Tower Defense/Facility Data")]
public class FacilityData : ScriptableObject
{
    [Header("기본 정보")]
    public string facilityName;
    public FacilityType facilityType;
    public Sprite facilitySprite;
    public GameObject facilityPrefab;
    
    [Header("건설 조건")]
    public int woodCost;
    public int scrapCost;
    public int stoneCost;

    [Header("HP")]
    public int baseHP;  // 팀원 추가
    
    [Header("기능")]
    [TextArea(3, 5)]
    public string description;
    
    [Header("생산 관련 (생산 시설인 경우)")]
    public bool isProductionFacility;
    public CraftItemData[] producibleItems;
    public float productionTime;
    
    [Header("유닛 소환 관련 (소환진인 경우)")]
    public bool isSpawnFacility;
    public UnitSo assignedUnit;
    
    [Header("함정 관련")]
    public int damagePerSecond;
    public float effectRadius;
    
    [Header("특수 효과")]
    public bool hasSpecialEffect;
    [TextArea(2, 3)]
    public string specialEffectDesc;
}

public enum FacilityType
{
    DungeonCore,
    Production,
    Trap,
    Lure,
    Spawn,
    Special
}