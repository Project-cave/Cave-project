using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    [Header("최상위 패널")]
    [SerializeField] private GameObject cutscenePanel;

    [Header("나레이션")]
    [SerializeField] private GameObject narrationGroup;
    [SerializeField] private TextMeshProUGUI txtNarrationContext;

    [Header("대사")]
    [SerializeField] private GameObject dialogueGroup;
    [SerializeField] private Image imgBackground;
    [SerializeField] private Image imgPortrait;
    [SerializeField] private Image speakerBackground;
    [SerializeField] private TextMeshProUGUI txtSpeaker;
    [SerializeField] private TextMeshProUGUI txtDialogueContext;

    private StoryData currentStory;
    private int currentLineIndex = 0;
    private Action onCutsceneComplete;

    public bool IsPlaying { get; private set; } = false;

    public void PlayStory(StoryData storyData, Action onComplete = null)
    {
        if (storyData == null) return;

        currentStory = storyData;
        currentLineIndex = 0;
        onCutsceneComplete = onComplete;
        IsPlaying = true;

        cutscenePanel.SetActive(true);
        RenderCurrentLine();
    }

    private void Update()
    {
        if (!IsPlaying) return;

        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            AdvanceLine();
        }
    }

    private void AdvanceLine()
    {
        currentLineIndex++;
        if (currentLineIndex < currentStory.LineCount)
        {
            RenderCurrentLine();
        }
        else
        {
            EndCutscene();
        }
    }

    private void RenderCurrentLine()
    {
        var line = currentStory.GetLine(currentLineIndex);

        if (line.lineType == StoryData.LineType.Narration)
        {
            narrationGroup.SetActive(true);
            dialogueGroup.SetActive(false);

            txtNarrationContext.text = line.context;
        }
        else if (line.lineType == StoryData.LineType.Dialogue)
        {
            narrationGroup.SetActive(false);
            dialogueGroup.SetActive(true);

            txtSpeaker.text = line.speakerName;

            if (line.speakerName == "") speakerBackground.gameObject.SetActive(false);
            else speakerBackground.gameObject.SetActive(true);

            txtDialogueContext.text = line.context;

            if (line.backgroundImage != null)
            {
                imgBackground.gameObject.SetActive(true);
                imgBackground.sprite = line.backgroundImage;
            }
            else
            {
                imgBackground.gameObject.SetActive(false);
            }

            if (imgPortrait != null)
            {
                imgPortrait.gameObject.SetActive(line.speakerPortrait != null);
                imgPortrait.sprite = line.speakerPortrait;
            }
        }
    }

    private void EndCutscene()
    {
        IsPlaying = false;
        cutscenePanel.SetActive(false);
        onCutsceneComplete?.Invoke();
    }
}