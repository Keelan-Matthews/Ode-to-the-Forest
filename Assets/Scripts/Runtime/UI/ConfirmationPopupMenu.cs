using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ConfirmationPopupMenu : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI displayBoxText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    
    public void ActivateMenu(string displayText, UnityAction confirmAction, UnityAction cancelAction)
    {
        gameObject.SetActive(true);
        displayBoxText.text = displayText;
        
        // remove all listeners
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        
        confirmButton.onClick.AddListener(() =>
        {
            DeactivateMenu();
            confirmAction();
        });
        
        cancelButton.onClick.AddListener(() =>
        {
            DeactivateMenu();
            cancelAction();
        });
    }
    
    public void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }
}
