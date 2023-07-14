using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private GameObject _permaSeedImage;
    
    private void Awake()
    {
        _permaSeedImage = transform.GetChild(0).gameObject;
    }
    
    //This method takes in a Perma Seed and sets the image of the Perma Seed to the image component
    public void SetPermaSeedImage(PermaSeed permaSeed)
    {
        _permaSeedImage.SetActive(false);
        _permaSeedImage.GetComponent<Image>().sprite = permaSeed.icon;
        _permaSeedImage.SetActive(true);
    }
    
    // this method clears the image component
    public void ClearPermaSeedImage()
    {
        _permaSeedImage.SetActive(false);
        _permaSeedImage.GetComponent<Image>().sprite = null;
        _permaSeedImage.SetActive(true);
    }
}
