using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitType", menuName = "Tower Defense/Unit Type Data")]
public class UnitTypeData : ScriptableObject
{
    [Header("Unit Info")]
    public string unitTypeName;  // 고블린, 스켈레톤 등
    public Sprite unitIcon;      // 유닛 아이콘
    
    [Header("Spawn Facility")]
    public Sprite facilitySprite;  // 생산 시설 스프라이트 (포탈 이미지)
    
    [Header("Description")]
    public string description;   // 유닛 설명
}
