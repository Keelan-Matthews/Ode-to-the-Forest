using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public void OnButtonHover()
    {
        AudioManager.PlaySound(AudioManager.Sound.ButtonHover, transform.position);
    }
    
    public void OnButtonClick()
    {
        AudioManager.PlaySound(AudioManager.Sound.ButtonClick, transform.position);
    }
    
    public void OnDisabledButtonClick()
    {
        AudioManager.PlaySound(AudioManager.Sound.DisabledButtonClick, transform.position);
    }
}
