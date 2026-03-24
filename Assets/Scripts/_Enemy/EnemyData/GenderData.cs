using UnityEngine;

[CreateAssetMenu(fileName = "New Class Data", menuName = "EnemyStats/Gender Data")]
public class GenderData : ScriptableObject
{
    [Header("기본 정보")]
    public EnemyGenderType genderType;
}