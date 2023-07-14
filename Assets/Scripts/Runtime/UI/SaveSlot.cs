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
    
    [Header("Clear Data Button")]
    [SerializeField] private Button clearDataButton;
    
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
            // no data
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
            clearDataButton.gameObject.SetActive(false);
        }
        else
        {
            // has data
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);
            clearDataButton.gameObject.SetActive(true);
            
            // set profile id
            profileIdText.text = $"SAVE {profileId}";
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
