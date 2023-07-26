using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour, IDataPersistence
{
    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private AudioMixer audioMixer;

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

    public void LoadData(GameData data)
    {
        masterVolumeSlider.value = data.MasterVolume;
        musicVolumeSlider.value = data.MusicVolume;
        sfxVolumeSlider.value = data.SfxVolume;
        
        SetMasterVolume();
        SetMusicVolume();
        SetSfxVolume();
    }

    public void SaveData(GameData data)
    {
        data.MasterVolume = masterVolumeSlider.value;
        data.MusicVolume = musicVolumeSlider.value;
        data.SfxVolume = sfxVolumeSlider.value;
    }
}
