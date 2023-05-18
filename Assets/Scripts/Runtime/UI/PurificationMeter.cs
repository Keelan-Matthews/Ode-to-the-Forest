using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurificationMeter : MonoBehaviour
{

    public Slider slider;

    public void SetMaxPurification(float time)
    {
        // Set the slider value to the time
        slider.maxValue = time;
        slider.value = 0;
    }
    
    public void SetPurification(float time)
    {
        // Set the slider value to the time
        slider.value = time;
    }
    
    public float GetPurification()
    {
        // Return the slider value
        return slider.value;
    }
}
