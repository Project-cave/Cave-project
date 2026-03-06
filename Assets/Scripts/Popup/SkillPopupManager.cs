using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPopupManager : MonoBehaviour
{
    public static SkillPopupManager instance;
    public GameObject closePanel;
    public RequestPopup requestPopup;
    public InvenPopup invenPopup;
    public Button CloseBtn;

    private Canvas rootCanvas;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        rootCanvas = FindFirstObjectByType<Canvas>().rootCanvas;
    }

    private void Start()
    {
        CloseBtn.onClick.AddListener(() => SceneChanger.instance.LoadBattleScene());
    }

    public void OpenPopup(RequestMonster node)
    {
        requestPopup.Open(node, GetLocalPos(node.rectTransform));           
        closePanel.SetActive(true);
    }

    public void OpenPopup(InvenMonster node)
    {
        invenPopup.Open(node, GetLocalPos(node.rectTransform));
        closePanel.SetActive(true);
    }

    public void ClosePopup()
    {
        requestPopup.gameObject.SetActive(false);
        invenPopup.gameObject.SetActive(false);
        closePanel.SetActive(false);
    }

    private Vector2 GetLocalPos(RectTransform targetRect)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, targetRect.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.GetComponent<RectTransform>(),
            screenPos,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out Vector2 localPos
            );
        return localPos;
    }
}
