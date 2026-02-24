using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;

    public GameObject closePanel;
    public GameObject invenMonsters;
    public Button CloseBtn;
    
    public RectTransform popup;
    private TextMeshProUGUI requestDesc;
    private Button requestBtn;

    public RectTransform invenPopup;
    private TextMeshProUGUI unitDesc;
    private TextMeshProUGUI materialDesc;
    private Button MakeBtn;


    private Canvas rootCanvas;
    private int dataIndex;

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

        requestDesc = popup.GetComponentInChildren<TextMeshProUGUI>();
        requestBtn = popup.GetComponentInChildren<Button>();

        unitDesc = invenPopup.GetComponentsInChildren<TextMeshProUGUI>()[0];
        materialDesc = invenPopup.GetComponentsInChildren<TextMeshProUGUI>()[1];
        MakeBtn = invenPopup.GetComponentInChildren<Button>();
    }

    private void Start()
    {
        CloseBtn.onClick.AddListener(() => SceneChanger.instance.LoadBattleScene());
    }

    public void OpenPopup(UnitSo data, RectTransform targetRect, bool isInven)
    {
        this.requestDesc.text = data.requestDesc;
        this.unitDesc.text = data.unitDesc;
        this.materialDesc.text = data.materialDesc;
            
        dataIndex = data.unitNum;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, targetRect.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.GetComponent<RectTransform>(),
            screenPos,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out Vector2 localPos
            );

        if (isInven) 
        {
            invenPopup.localPosition = localPos + new Vector2(targetRect.sizeDelta.x, 0);
            invenPopup.gameObject.SetActive(true);
        }
        else
        {
            popup.localPosition = localPos + new Vector2(targetRect.sizeDelta.x, 0);
            popup.gameObject.SetActive(true);
        }
           
        closePanel.SetActive(true);
    }

    public void ClosePopup()
    {
        popup.gameObject.SetActive(false);
        invenPopup.gameObject.SetActive(false);
        closePanel.SetActive(false);
    }

    public void CompleteRequest()
    {
        Transform invenMonster = invenMonsters.transform.GetChild(dataIndex);

        invenMonster.GetComponent<InvenMonster>().Lock = false;
        invenMonster.GetComponent<Button>().interactable = true;
    }

    public void MakeUnit()
    {

    }
}
