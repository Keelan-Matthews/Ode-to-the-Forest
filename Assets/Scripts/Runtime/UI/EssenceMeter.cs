using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EssenceMeter : MonoBehaviour
{
    public Slider slider;

    public void SetMaxEssenceFragment(float max)
    {
        // Set the slider value to the time
        slider.maxValue = max;
        slider.value = 0;
    }
    
    public void SetEssenceFragment(float essence)
    {
        // Set the slider value to the essence, but making it gradual and not instant
        var oldEssence = slider.value;
        StartCoroutine(ChangeEssence(oldEssence, essence));
    }
    
    private IEnumerator ChangeEssence(float oldEssence, float newEssence)
    {
        // Make the slider slide between the old and new essence
        const float duration = 0.3f;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            slider.value = Mathf.Lerp(oldEssence, newEssence, t);
            yield return null;
        }
    }
}
