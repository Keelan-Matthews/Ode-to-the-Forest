using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory")] [SerializeField] private InventorySlot inventorySlot;
    
    public static InventoryManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate GameManager instances
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scene changes
    }
    
    public void AddPermaSeed(PermaSeed permaSeed)
    {
        if (permaSeed == null) return;
        inventorySlot.SetPermaSeedImage(permaSeed);
    }
    
    public void RemovePermaSeed()
    {
        inventorySlot.ClearPermaSeedImage();
    }
}
