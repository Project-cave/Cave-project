using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    
    [Header("Inventory Settings")]
    [SerializeField] private int maxSlots = 20;
    
    // 아이템 저장소 (아이템 타입별 개수)
    private Dictionary<CraftItemData, int> inventory = new Dictionary<CraftItemData, int>();
    
    // UI 업데이트를 위한 이벤트
    public event System.Action OnInventoryChanged;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    // 아이템 추가
    public bool AddItem(CraftItemData item, int count = 1)
    {
        if (item == null || count <= 0)
            return false;
        
        // 이미 있는 아이템이면 개수 증가
        if (inventory.ContainsKey(item))
        {
            inventory[item] += count;
        }
        else
        {
            // 새 아이템 추가
            if (inventory.Count >= maxSlots)
            {
                Debug.Log("인벤토리가 가득 찼습니다!");
                return false;
            }
            
            inventory.Add(item, count);
        }
        
        Debug.Log($"{item.itemName} x{count} 획득!");
        OnInventoryChanged?.Invoke();
        return true;
    }
    
    // 아이템 제거
    public bool RemoveItem(CraftItemData item, int count = 1)
    {
        if (item == null || count <= 0)
            return false;
        
        if (!inventory.ContainsKey(item))
        {
            Debug.Log($"{item.itemName}이(가) 인벤토리에 없습니다!");
            return false;
        }
        
        if (inventory[item] < count)
        {
            Debug.Log($"{item.itemName}이(가) 부족합니다! (필요: {count}, 보유: {inventory[item]})");
            return false;
        }
        
        inventory[item] -= count;
        
        // 개수가 0이 되면 딕셔너리에서 제거
        if (inventory[item] <= 0)
        {
            inventory.Remove(item);
        }
        
        Debug.Log($"{item.itemName} x{count} 소비!");
        OnInventoryChanged?.Invoke();
        return true;
    }
    
    // 아이템 개수 확인
    public int GetItemCount(CraftItemData item)
    {
        if (item == null || !inventory.ContainsKey(item))
            return 0;
        
        return inventory[item];
    }
    
    // 아이템이 충분한지 확인
    public bool HasItem(CraftItemData item, int count = 1)
    {
        if (item == null || count <= 0)
            return false;
        
        return GetItemCount(item) >= count;
    }
    
    // 여러 아이템이 충분한지 확인
    public bool HasItems(CraftItemData[] items, int[] counts)
    {
        if (items == null || counts == null || items.Length != counts.Length)
            return false;
        
        for (int i = 0; i < items.Length; i++)
        {
            if (!HasItem(items[i], counts[i]))
                return false;
        }
        
        return true;
    }
    
    // 여러 아이템 제거
    public bool RemoveItems(CraftItemData[] items, int[] counts)
    {
        if (items == null || counts == null || items.Length != counts.Length)
            return false;
        
        // 먼저 모두 있는지 확인
        if (!HasItems(items, counts))
            return false;
        
        // 모두 제거
        for (int i = 0; i < items.Length; i++)
        {
            RemoveItem(items[i], counts[i]);
        }
        
        return true;
    }
    
    // 전체 인벤토리 가져오기
    public Dictionary<CraftItemData, int> GetInventory()
    {
        return new Dictionary<CraftItemData, int>(inventory);
    }
    
    // 인벤토리 초기화
    public void ClearInventory()
    {
        inventory.Clear();
        OnInventoryChanged?.Invoke();
        Debug.Log("인벤토리가 초기화되었습니다!");
    }
}