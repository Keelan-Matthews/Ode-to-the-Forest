using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIPrefab : MonoBehaviour
{
    [SerializeField] private List<AbilityIcon> abilityIcons;
    
    private Image _abilityImage;
    
    private void Awake()
    {
        _abilityImage = GetComponent<Image>();
    }

    // Use the ability name to set the sprite
    public void SetAbility(string abilityName)
    {
        // Get the sprite from the list based on the ability name
        _abilityImage.sprite = abilityIcons.Find(x => x.abilityName == abilityName).abilitySprite;
        
        // If the sprite is null, log an error
        if (_abilityImage.sprite == null)
        {
            Debug.LogError($"No sprite found for {abilityName}");
        }
        
        // Set the sprite to the image
        _abilityImage.sprite = _abilityImage.sprite;
    }
}