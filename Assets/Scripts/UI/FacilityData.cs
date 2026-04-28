using UnityEngine;

[CreateAssetMenu(fileName = "FacilityData", menuName = "Tower Defense/Facility Data")]
public class FacilityData : ScriptableObject
{
    [Header("기본 정보")]
    public string facilityName;  // 시설명
    public FacilityType facilityType;  // 종류
    public Sprite facilitySprite;  // 스프라이트
    public GameObject facilityPrefab;  // 프리팹 (있으면)
    
    [Header("건설 조건")]
    public int woodCost;   // 목재 비용
    public int scrapCost;  // 고철 비용
    public int stoneCost;  // 돌 비용
    
    [Header("기능")]
    [TextArea(3, 5)]
    public string description;  // 기능 설명
    
    [Header("생산 관련 (생산 시설인 경우)")]
    public bool isProductionFacility;  // 생산 시설 여부
    public CraftItemData[] producibleItems;  // 생산 가능한 아이템들
    public float productionTime;  // 생산 시간
    
    [Header("유닛 소환 관련 (소환진인 경우)")]
    public bool isSpawnFacility;  // 유닛 소환 시설 여부
    public UnitSo assignedUnit;  // 배치된 유닛 (런타임에 설정)
    
    [Header("함정 관련")]
    public int damagePerSecond;  // 함정 데미지 (함정인 경우)
    public float effectRadius;   // 효과 범위
    
    [Header("특수 효과")]
    public bool hasSpecialEffect;  // 특수 효과 여부
    [TextArea(2, 3)]
    public string specialEffectDesc;  // 특수 효과 설명
}

public enum FacilityType
{
    DungeonCore,  // 던전 코어
    Production,   // 생산 시설
    Trap,         // 함정
    Lure,         // 유인
    Spawn,        // 소환진
    Special       // 특수
}
