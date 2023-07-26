using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class GameAssets : MonoBehaviour
{
    public static GameAssets Instance { get; private set; }
    public AudioMixer AudioMixer;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate GameManager instances
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scene changes
        
        AudioManager.Initialize();
    }
    
    public List<Audio> audioList;
    public PurificationMeter purificationMeter;
    public Camera mainCamera;
    public TextMeshProUGUI essenceText;

    [System.Serializable]
    public class Audio
    {
        public AudioClip clip;
        public AudioManager.Sound sound;
    }
}
