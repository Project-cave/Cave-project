using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemCountText;
    [SerializeField] private Image slotBackground;  // 슬롯 배경 (선택사항)
    
    private CraftItemData itemData;
    private int count;
    
    public void SetItem(CraftItemData item, int itemCount)
    {
        itemData = item;
        count = itemCount;
        
        if (itemIcon != null)
        {
            itemIcon.sprite = item.itemSprite;
            itemIcon.enabled = true;
            // 아이콘 완전히 보이게
            Color iconColor = itemIcon.color;
            iconColor.a = 1f;
            itemIcon.color = iconColor;
        }
        
        if (itemCountText != null)
        {
            itemCountText.text = itemCount.ToString();
            itemCountText.enabled = true;
        }
    }
    
    public void ClearSlot()
    {
        itemData = null;
        count = 0;
        
        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }
        
        if (itemCountText != null)
        {
            itemCountText.text = "";
            itemCountText.enabled = false;
        }
    }
    
    // 호환성을 위한 메서드
    public void Clear()
    {
        ClearSlot();
    }
}