using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }
    
    [Header("UI References")]
    [SerializeField] private Transform slotsContainer;  // Grid Layout이 있는 부모 오브젝트
    [SerializeField] private GameObject slotPrefab;     // 슬롯 프리팹 (선택사항)
    
    [Header("Settings")]
    [SerializeField] private int maxSlots = 20;
    
    private List<InventorySlot> slots = new List<InventorySlot>();
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        Debug.Log("===== InventoryUI 시작 =====");
        
        // 슬롯 생성
        CreateSlots();
        
        // InventoryManager 이벤트 구독
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += UpdateUI;
            Debug.Log("InventoryUI: InventoryManager 이벤트 구독 완료!");
        }
        else
        {
            Debug.LogError("InventoryUI: InventoryManager가 없습니다!");
        }
        
        // 초기 UI 업데이트
        UpdateUI();
    }
    
    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= UpdateUI;
        }
    }
    
    void CreateSlots()
    {
        if (slotsContainer == null)
        {
            Debug.LogError("InventoryUI: slotsContainer가 연결되지 않았습니다!");
            return;
        }
        
        Debug.Log($"슬롯 생성 시작... (최대 {maxSlots}개)");
        
        // 기존 슬롯 정리
        foreach (Transform child in slotsContainer)
        {
            Destroy(child.gameObject);
        }
        slots.Clear();
        
        // 슬롯 생성
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slotObj;
            
            if (slotPrefab != null)
            {
                // 프리팹이 있으면 사용
                slotObj = Instantiate(slotPrefab, slotsContainer);
            }
            else
            {
                // 프리팹이 없으면 자동 생성
                slotObj = CreateSlotManually(i);
            }
            
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            if (slot != null)
            {
                slots.Add(slot);
            }
            else
            {
                Debug.LogError($"슬롯 {i}에 InventorySlot 컴포넌트가 없습니다!");
            }
        }
        
        Debug.Log($"슬롯 {slots.Count}개 생성 완료!");
    }
    
    GameObject CreateSlotManually(int index)
    {
        // 프리팹 없을 때 자동으로 슬롯 생성
        GameObject slotObj = new GameObject($"Slot_{index}");
        slotObj.transform.SetParent(slotsContainer);
        slotObj.transform.localScale = Vector3.one;
        
        // InventorySlot 컴포넌트 추가
        InventorySlot slot = slotObj.AddComponent<InventorySlot>();
        
        // 배경 이미지
        UnityEngine.UI.Image bg = slotObj.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        RectTransform rt = slotObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80, 80);
        
        // 아이템 이미지
        GameObject imgObj = new GameObject("ItemIcon");
        imgObj.transform.SetParent(slotObj.transform);
        imgObj.transform.localScale = Vector3.one;
        imgObj.transform.localPosition = Vector3.zero;
        
        UnityEngine.UI.Image itemImage = imgObj.AddComponent<UnityEngine.UI.Image>();
        itemImage.enabled = false;
        
        RectTransform imgRt = imgObj.GetComponent<RectTransform>();
        imgRt.anchorMin = new Vector2(0.5f, 0.5f);
        imgRt.anchorMax = new Vector2(0.5f, 0.5f);
        imgRt.sizeDelta = new Vector2(60, 60);
        
        // 개수 텍스트
        GameObject textObj = new GameObject("ItemCountText");
        textObj.transform.SetParent(slotObj.transform);
        textObj.transform.localScale = Vector3.one;
        
        TMPro.TextMeshProUGUI countText = textObj.AddComponent<TMPro.TextMeshProUGUI>();
        countText.text = "";
        countText.fontSize = 18;
        countText.color = Color.white;
        countText.alignment = TMPro.TextAlignmentOptions.BottomRight;
        countText.enabled = false;
        
        RectTransform textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0, 0);
        textRt.anchorMax = new Vector2(1, 1);
        textRt.sizeDelta = Vector2.zero;
        textRt.anchoredPosition = new Vector2(-5, 5);
        
        // InventorySlot에 참조 연결 (리플렉션 사용)
        var itemIconField = typeof(InventorySlot).GetField("itemIcon", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var itemCountTextField = typeof(InventorySlot).GetField("itemCountText", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (itemIconField != null)
            itemIconField.SetValue(slot, itemImage);
        if (itemCountTextField != null)
            itemCountTextField.SetValue(slot, countText);
        
        return slotObj;
    }
    
    public void UpdateUI()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("UpdateUI: InventoryManager가 없습니다!");
            return;
        }
        
        Debug.Log("===== 인벤토리 UI 업데이트 시작 =====");
        
        // 모든 슬롯 비우기
        foreach (var slot in slots)
        {
            if (slot != null)
                slot.ClearSlot();
        }
        
        // 인벤토리 데이터 가져오기
        Dictionary<CraftItemData, int> inventory = InventoryManager.Instance.GetInventory();
        
        Debug.Log($"표시할 아이템: {inventory.Count}개");
        
        // 각 아이템을 슬롯에 표시
        int slotIndex = 0;
        foreach (var kvp in inventory)
        {
            if (slotIndex >= slots.Count)
            {
                Debug.LogWarning("슬롯이 부족합니다!");
                break;
            }
            
            CraftItemData item = kvp.Key;
            int count = kvp.Value;
            
            Debug.Log($"슬롯 {slotIndex}: {item.itemName} x{count}");
            
            slots[slotIndex].SetItem(item, count);
            slotIndex++;
        }
        
        Debug.Log("===== 인벤토리 UI 업데이트 완료 =====");
    }
}
