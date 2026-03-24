using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    
    [SerializeField] private int startingResources = 100;
    private int currentResources;
    
    public event Action<int> OnResourceChanged;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        currentResources = startingResources;
        OnResourceChanged?.Invoke(currentResources);
    }
    
    public int GetCurrentResources()
    {
        return currentResources;
    }
    
    public bool CanAfford(int cost)
    {
        return currentResources >= cost;
    }
    
    public bool SpendResources(int amount)
    {
        if (CanAfford(amount))
        {
            currentResources -= amount;
            OnResourceChanged?.Invoke(currentResources);
            return true;
        }
        return false;
    }
    
    public void AddResources(int amount)
    {
        currentResources += amount;
        OnResourceChanged?.Invoke(currentResources);
    }
}
