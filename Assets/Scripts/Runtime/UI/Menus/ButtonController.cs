using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public void OnButtonHover()
    {
        if (!GetComponent<Button>().interactable) return;
        AudioManager.PlaySound(AudioManager.Sound.ButtonHover, transform.position);
    }

    public void OnButtonClick()
    {
        AudioManager.PlaySound(AudioManager.Sound.ButtonClick, transform.position);
    }
}