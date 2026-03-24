using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    
    [Header("UI References")]
    [SerializeField] private Transform inventoryContent;
    [SerializeField] private GameObject inventorySlotPrefab;
    
    [Header("Settings")]
    [SerializeField] private int maxSlots = 20;  // 5x4 = 20
    
    // 아이템별 개수를 저장 (스택)
    private Dictionary<CraftItemData, int> itemStacks = new Dictionary<CraftItemData, int>();
    private List<InventorySlot> slotInstances = new List<InventorySlot>();
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        InitializeSlots();
    }
    
    // 처음에 빈 슬롯 20개 생성
    void InitializeSlots()
    {
        if (inventoryContent == null || inventorySlotPrefab == null)
            return;
        
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, inventoryContent);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            
            if (slot != null)
            {
                slot.Clear();  // 빈 슬롯으로 초기화
                slotInstances.Add(slot);
            }
        }
    }
    
    public void AddItem(CraftItemData item)
    {
        // 이미 있는 아이템이면 개수 증가
        if (itemStacks.ContainsKey(item))
        {
            itemStacks[item]++;
        }
        else
        {
            itemStacks[item] = 1;
        }
        
        Debug.Log($"인벤토리에 추가: {item.itemName} x{itemStacks[item]}");
        UpdateInventoryUI();
    }
    
    public void RemoveItem(CraftItemData item, int amount = 1)
    {
        if (!itemStacks.ContainsKey(item))
            return;
        
        itemStacks[item] -= amount;
        
        // 개수가 0이 되면 딕셔너리에서 제거
        if (itemStacks[item] <= 0)
        {
            itemStacks.Remove(item);
        }
        
        UpdateInventoryUI();
    }
    
    public Dictionary<CraftItemData, int> GetItemStacks()
    {
        return itemStacks;
    }
    
    public int GetTotalItemCount()
    {
        int total = 0;
        foreach (var count in itemStacks.Values)
        {
            total += count;
        }
        return total;
    }
    
    // 슬롯을 새로 생성하지 않고 기존 슬롯 내용만 업데이트
    void UpdateInventoryUI()
    {
        // 모든 슬롯 초기화
        foreach (var slot in slotInstances)
        {
            if (slot != null)
                slot.Clear();
        }
        
        // 아이템을 순서대로 슬롯에 배치
        int slotIndex = 0;
        foreach (var itemStack in itemStacks)
        {
            if (slotIndex >= slotInstances.Count)
                break;  // 슬롯 초과 시 중단
            
            if (slotInstances[slotIndex] != null)
            {
                slotInstances[slotIndex].SetItem(itemStack.Key, itemStack.Value);
            }
            
            slotIndex++;
        }
    }
}
