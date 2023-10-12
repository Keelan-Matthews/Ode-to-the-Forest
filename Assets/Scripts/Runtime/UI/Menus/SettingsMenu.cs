using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        // Initially, set the slider values to the current volume
        masterVolumeSlider.value = audioMixer.GetFloat("MasterVolume", out var masterVolume) ? Mathf.Pow(10, masterVolume / 20) : 1;
        musicVolumeSlider.value = audioMixer.GetFloat("MusicVolume", out var musicVolume) ? Mathf.Pow(10, musicVolume / 20) : 1;
        sfxVolumeSlider.value = audioMixer.GetFloat("SfxVolume", out var sfxVolume) ? Mathf.Pow(10, sfxVolume / 20) : 1;
    }

    public void SetMasterVolume()
    {
        var volume = masterVolumeSlider.value;
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }
    
    public void SetMusicVolume()
    {
        var volume = musicVolumeSlider.value;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }
    
    public void SetSfxVolume()
    {
        var volume = sfxVolumeSlider.value;
        audioMixer.SetFloat("SfxVolume", Mathf.Log10(volume) * 20);
    }
}
