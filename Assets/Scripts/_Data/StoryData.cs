using UnityEngine;

[CreateAssetMenu(fileName = "NewStoryData", menuName = "Story Data")]
public class StoryData : ScriptableObject
{
    public enum LineType
    {
        Narration,  // 나레이션
        Dialogue    // 스프라이트 + 대사
    }

    [System.Serializable]
    public struct DialogueLine
    {
        public LineType lineType;

        [Header("Narration / Dialogue")]
        [TextArea(3, 5)] public string context;

        [Header("Dialogue")]
        public Sprite backgroundImage;
        public string speakerName;
        public Sprite speakerPortrait;
    }

    [Header("Story")]
    [SerializeField] private DialogueLine[] dialogueLines;

    public int LineCount => dialogueLines.Length;

    public DialogueLine GetLine(int index)
    {
        if (index < 0 || index >= dialogueLines.Length)
        {
            Debug.LogError($"[StoryData] 인덱스 {index}가 범위를 벗어났습니다.");
            return default;
        }
        return dialogueLines[index];
    }
}