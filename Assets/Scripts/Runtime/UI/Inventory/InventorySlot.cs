using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private Image _permaSeedImage;
    
    private void Awake()
    {
        _permaSeedImage = GetComponentInChildren<Image>();
    }
    
    //This method takes in a Perma Seed and sets the image of the Perma Seed to the image component
    public void SetPermaSeedImage(PermaSeed permaSeed)
    {
        _permaSeedImage.sprite = permaSeed.icon;
    }
    
    // this method clears the image component
    public void ClearPermaSeedImage()
    {
        _permaSeedImage.sprite = null;
    }
}
