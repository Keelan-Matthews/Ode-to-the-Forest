using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessControls : MonoBehaviour
{
    [SerializeField] private Volume postProcessVolume;
    
    [Header("Profiles")]
    [SerializeField] private VolumeProfile deathProfile;
    [SerializeField] private VolumeProfile lowHealthProfile;
    
    public bool isPulsing;

    public static PostProcessControls Instance;
    
    private void Awake()
    {
        Instance = this;
    }

    public void SetDeathProfile()
    {
        isPulsing = false;
        postProcessVolume.profile = deathProfile;
    }
    
    public void SetLowHealthProfile()
    {
        postProcessVolume.profile = lowHealthProfile;
    }
    
    public void RampUpWeightCoroutine()
    {
        StartCoroutine(RampUpWeight());
    }
    
    private IEnumerator RampUpWeight()
    {
        var volume = 0f;
        var duration = 0.2f;
        while (volume < 1f)
        {
            volume += Time.deltaTime / duration;
            postProcessVolume.weight = volume;
            yield return null;
        }
    }
    
    public void ResetWeightCoroutine()
    {
        StartCoroutine(ResetWeight());
    }
    
    private IEnumerator ResetWeight()
    {
        var volume = 1f;
        var duration = 1f;
        while (volume > 0f)
        {
            volume -= Time.deltaTime / duration;
            postProcessVolume.weight = volume;
            yield return null;
        }
    }
    
    private void Update()
    {
        if (isPulsing)
        {
            postProcessVolume.weight = Mathf.PingPong(Time.time, 1f);
        }
    }
}
