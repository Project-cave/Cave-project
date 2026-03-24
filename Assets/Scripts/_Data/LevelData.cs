using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
    public int dungeonGrade;
    public int stageNumber;

    [Header("종족 발생 확률")]
    public float humanProb;
    public float elfProb;
    public float dwarfProb;
    public float anthroProb;

    [Header("직업 발생 확률")]
    public float warriorProb;
    public float archerProb;
    public float knightProb;
    public float swordsManProb;
    public float assassinProb;
    public float wizardProb;
    public float magicianProb;
    public float paladinProb;

    [Header("등급별 소환 인원")]
    public int bronzeCount;
    public int silverCount;
    public int goldCount;
    public int platinumCount;

    /* [Header("성별 발생 확률 (%)")]
    public float maleProb;
    public float femaleProb;
    */
}