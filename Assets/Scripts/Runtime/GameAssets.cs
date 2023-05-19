using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
        // if (Instance != null && Instance != this)
        // {
        //     Destroy(gameObject); // Destroy duplicate GameAssets instances
        //     return;
        // }
        //
        // Instance = this;
        // DontDestroyOnLoad(gameObject); // Persist across scene changes
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
