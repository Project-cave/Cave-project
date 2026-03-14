using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequestPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI requestDesc;
    [SerializeField] private Button requestBtn;
    private RequestMonster currentNode;

    public void Open(RequestMonster node, Vector2 localPos)
    {
        currentNode = node;
        requestDesc.text = node.unitData.requestDesc;
        transform.localPosition = localPos + new Vector2(node.rectTransform.sizeDelta.x, 0);
        gameObject.SetActive(true);
    }

    public void CompleteRequest()
    {
        currentNode.OnUnlock();
        MonsterPopupManager.instance.ClosePopup();
    }
}
