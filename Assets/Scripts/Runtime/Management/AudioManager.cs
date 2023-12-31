using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioManager
{
    public enum Sound
    {
        PlayerShoot,
        PlayerHit,
        PlayerWalk,
        EnemyHit,
        EnemyDeath,
        EnemySpawn,
        EnemyAttack,
        EnemiesWalk,
        HomeBackgroundMusic,
        ForestBackgroundMusic,
        ObeliskUseGood,
        ObeliskUseBad,
        WaveStart,
        WaveEnd,
        BushRustle,
        ButtonClick,
        ButtonHover,
        DisabledButtonClick,
        OdeRevive,
        NewDay,
        SeedPlanted,
        SeedGrown,
        OpenDialogue,
        SeedPickup,
        SeedUproot,
        PlotUnlocked,
        EnterTrader,
        EnterObelisk,
        EnterPortal,
        InteractPortal,
        OdeDeath,
        ShowMenu,
        ArmStrike,
        BossEnrage,
        BossRoomStart,
        BossShoot,
        CoreHit,
        CoreDeath,
        TrackOde,
        LockOnOde,
        Fountain,
        ShrineOfYouthAmbience,
        EnterShrineOfYouth,
        EnterCollector,
        Heartbeat,
        ObstacleBreak,
        ObeliskDecide,
        Predict,
        OracleOneShot
    }

    private static Dictionary<Sound, float> _soundTimerDictionary;
    
    public static void Initialize()
    {
        _soundTimerDictionary = new Dictionary<Sound, float>();
        _soundTimerDictionary[Sound.PlayerWalk] = 0f;
    }

    public static void PlaySound(Sound sound, Vector3 position)
    {
        if (!CanPlaySound(sound)) return;

        // Limit the number of simultaneous instances of the same sound
        var maxInstances = 1;  // Maximum allowed instances for the same sound
        var currentInstances = GameObject.FindGameObjectsWithTag(sound.ToString()).Length;

        if (currentInstances >= maxInstances)
        {
            // Destroy the oldest sound instance
            var oldestSoundInstance = GameObject.FindGameObjectsWithTag(sound.ToString())
                .OrderBy(go => go.GetComponent<AudioSource>().time).FirstOrDefault();

            if (oldestSoundInstance != null)
            {
                Object.Destroy(oldestSoundInstance);
            }
        }

        var soundGameObject = new GameObject(sound.ToString());
        soundGameObject.transform.position = position;
        soundGameObject.tag = sound.ToString();

        var audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = GetAudioClip(sound);
        audioSource.outputAudioMixerGroup = GameAssets.Instance.AudioMixer.FindMatchingGroups("SFX").First();

        // Play the sound
        audioSource.Play();

        // Destroy the sound object after the sound has played
        Object.Destroy(soundGameObject, audioSource.clip.length);
    }


    // public static void PlaySound(Sound sound)
    // {
    //     if (!CanPlaySound(sound)) return;
    //     var soundGameObject = new GameObject(sound.ToString());
    //     var audioSource = soundGameObject.AddComponent<AudioSource>();
    //     audioSource.PlayOneShot(GetAudioClip(sound));
    //     
    //     // Destroy the sound object after the sound has played
    //     Object.Destroy(soundGameObject, audioSource.clip.length);
    // }
    
    private static bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            case Sound.PlayerShoot:
            case Sound.PlayerHit:
            case Sound.EnemyHit:
            case Sound.EnemyDeath:
            case Sound.EnemySpawn:
            case Sound.EnemyAttack:
            case Sound.ForestBackgroundMusic:
            case Sound.HomeBackgroundMusic:
            case Sound.ObeliskUseGood:
            case Sound.ObeliskUseBad:
            case Sound.WaveStart:
            case Sound.BushRustle:
            case Sound.ButtonClick: 
            case Sound.ButtonHover:
            case Sound.DisabledButtonClick:
            case Sound.OdeRevive:
            case Sound.NewDay:
            case Sound.SeedPlanted:
            case Sound.SeedGrown:
            case Sound.OpenDialogue:
            case Sound.SeedPickup:
            case Sound.SeedUproot:
            case Sound.PlotUnlocked:
            case Sound.EnterTrader:
            case Sound.EnterObelisk:
            case Sound.EnterPortal:
            case Sound.InteractPortal:
            case Sound.OdeDeath:
            case Sound.ShowMenu:
            case Sound.ArmStrike:
            case Sound.BossEnrage:
            case Sound.BossRoomStart:
            case Sound.BossShoot:
            case Sound.CoreHit:
            case Sound.CoreDeath:
            case Sound.TrackOde:
            case Sound.LockOnOde:
            case Sound.Fountain:
            case Sound.ShrineOfYouthAmbience:
            case Sound.EnterShrineOfYouth:
            case Sound.EnterCollector:
            case Sound.ObstacleBreak:
            case Sound.ObeliskDecide:
            case Sound.Predict:
            case Sound.OracleOneShot:
            default:
                return true;
            case Sound.PlayerWalk:
                if (!_soundTimerDictionary.ContainsKey(sound)) return true;
                var lastTimePlayed = _soundTimerDictionary[sound];
                const float playerWalkTimerMax = 0.2f;
                if (!(lastTimePlayed + playerWalkTimerMax < Time.time)) return false;
                _soundTimerDictionary[sound] = Time.time;
                return true;
            case Sound.WaveEnd:
                if (!_soundTimerDictionary.ContainsKey(sound)) return true;
                var lastTimePlayed1 = _soundTimerDictionary[sound];
                const float waveEndTimerMax = 5f;
                if (!(lastTimePlayed1 + waveEndTimerMax < Time.time)) return false;
                _soundTimerDictionary[sound] = Time.time;
                return true;
            case Sound.EnemiesWalk:
                if (!_soundTimerDictionary.ContainsKey(sound)) return true;
                var lastTimePlayed2 = _soundTimerDictionary[sound];
                const float enemiesWalkTimerMax = 0.5f;
                if (!(lastTimePlayed2 + enemiesWalkTimerMax < Time.time)) return false;
                _soundTimerDictionary[sound] = Time.time;
                return true;
            case Sound.Heartbeat:
                if (!_soundTimerDictionary.ContainsKey(sound)) return true;
                var lastTimePlayed3 = _soundTimerDictionary[sound];
                const float heartbeatTimerMax = 4f;
                if (!(lastTimePlayed3 + heartbeatTimerMax < Time.time)) return false;
                _soundTimerDictionary[sound] = Time.time;
                return true;
        }
    }
    
    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (var audio in GameAssets.Instance.audioList.Where(audio => audio.sound == sound))
        {
            return audio.clip;
        }

        Debug.LogError("Sound " + sound + " not found!");
        return null;
    }
    
    // Functions for playing background music and looping it
    public static void PlayBackgroundMusic(Sound sound)
    {
        var soundGameObject = new GameObject(sound.ToString());
        var audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = GetAudioClip(sound);
        audioSource.loop = true;
        audioSource.Play();
    }
    
    public static void StopBackgroundMusic(Sound sound)
    {
        var audioSource = GameObject.Find(sound.ToString()).GetComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = GameAssets.Instance.AudioMixer.FindMatchingGroups("Music").First();
        audioSource.Stop();
    }
}
