using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory")] 
    [SerializeField] private InventorySlot inventorySlot;

    [SerializeField] private TextMeshProUGUI seedText;
    [SerializeField] private ParticleSystem seedParticles;

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

    private void Start()
    {
        SetCanvasCamera();
    }

    public void AddPermaSeed(PermaSeed permaSeed)
    {
        if (permaSeed == null) return;
        inventorySlot.SetPermaSeedImage(permaSeed);

        seedText.text = permaSeed.seedName;
    }

    public void RemovePermaSeed(bool playParticles = false)
    {
        inventorySlot.ClearImage();
        
        seedText.text = "";
        
        if (playParticles)
        {
            seedParticles.Play();
        }
    }
    
    public void HideInventory()
    {
        inventorySlot.HideInventory();
    }
    
    public void ShowInventory()
    {
        inventorySlot.ShowInventory();
    }

    public void SetCanvasCamera()
    {
        var canvas = GetComponent<Canvas>();

        if (HomeRoomController.Instance != null)
        {
            canvas.worldCamera = HomeRoomController.Instance.homeCamera;
            // Set the sorting layer to "UI"
            canvas.sortingLayerName = "UI";
        }
        else if (RoomController.Instance != null)
        {
            canvas.worldCamera = RoomController.Instance.forestCamera;
            // Set the sorting layer to "UI"
            canvas.sortingLayerName = "UI";
        }
        else
        {
            canvas.worldCamera = null;
        }
    }
}
