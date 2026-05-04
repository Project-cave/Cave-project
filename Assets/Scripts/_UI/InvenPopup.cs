using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvenPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI unitDesc;
    [SerializeField] private TextMeshProUGUI materialDesc;
    [SerializeField] private Button MakeBtn;
    private InvenMonster currentNode;

    public void Open(InvenMonster node, Vector2 localPos)
    {
        currentNode = node;
        unitDesc.text = node.unitData.info.unitDesc;
        materialDesc.text = node.unitData.productionData.materialDesc;
        transform.localPosition = localPos + new Vector2(node.rectTransform.sizeDelta.x, 0);
        gameObject.SetActive(true);
    }

    public void MakeRequest()
    {
        SceneController.instance.gameMonsterPanel = currentNode.unitData;
        MonsterPopupManager.instance.ClosePopup();
        SceneController.instance.LoadGameScene();     
    }
}
