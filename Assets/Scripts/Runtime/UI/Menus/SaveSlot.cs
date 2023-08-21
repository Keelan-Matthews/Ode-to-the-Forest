using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("ProfileId")] 
    [SerializeField] private string profileId = "";
    
    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI profileIdText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private GameObject seedGrid;
    
    [Header("Clear Data Button")]
    [SerializeField] private Button clearDataButton;

    public bool hasData { get; private set; } = false;
    private Button _button;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void SetData(GameData data)
    {
        // check if data exists
        if (data == null)
        {
            hasData = false;
            // no data
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
            clearDataButton.gameObject.SetActive(false);
        }
        else
        {
            hasData = true;
            // has data
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);
            clearDataButton.gameObject.SetActive(true);
            
            // set profile id
            profileIdText.text = $"Day {data.Day}";
            var lastUpdated = data.LastUpdated;

            var overallLastUpdated = DataPersistenceManager.Instance.GetLastUpdated();

            // var dateTime = DateTime.FromBinary(lastUpdated);
            //
            // // convert to format 12/01/21 12:00:00
            
            if (lastUpdated == overallLastUpdated)
            {
                dateText.text = "Latest";
            }
            else
            {
                dateText.text = "";
            }

            // Get the active perma seeds
            var permaSeeds = data.SeedPlotSeeds;
            
            var seedSlots = seedGrid.GetComponentsInChildren<Image>();
            var count = 0;
            
            // Get the perma seed for each active perma seed
            foreach (var permaSeed in permaSeeds)
            {
                var seed = PermaSeedManager.Instance.GetSpecificPermaSeed(permaSeed);
                if (seed == null || seed.seedName == "Minimap") continue;
                
                // Set the sprite of the seed slot to the sprite of the perma seed
                seedSlots[count++].sprite = seed.icon;
            }
        }
    }
    
    public string GetProfileId()
    {
        return profileId;
    }
    
    public void SetInteractable(bool interactable)
    {
        _button.interactable = interactable;
        clearDataButton.interactable = interactable;
    }
}
