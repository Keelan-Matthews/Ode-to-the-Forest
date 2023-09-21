using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private GameObject _image;
    
    private void Awake()
    {
        _image = transform.GetChild(0).gameObject;
        
        // Set the image color to transparent
        _image.GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }
    
    //This method takes in a Perma Seed and sets the image of the Perma Seed to the image component
    public void SetPermaSeedImage(PermaSeed permaSeed)
    {
        _image.SetActive(false);
        _image.GetComponent<Image>().sprite = permaSeed.icon;
        _image.SetActive(true);
        
        // Set the image color to opaque
        _image.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    // this method clears the image component
    public void ClearImage()
    {
        _image.SetActive(false);
        _image.GetComponent<Image>().sprite = null;
        _image.SetActive(true);
        
        // Set the image color to transparent
        _image.GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }
}
