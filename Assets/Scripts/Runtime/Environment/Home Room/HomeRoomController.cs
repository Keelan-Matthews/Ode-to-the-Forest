using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HomeRoomController : MonoBehaviour, IDataPersistence
{
    public TextMeshProUGUI homeEssenceText;
    public static HomeRoomController Instance;
    private int _homeEssence;
    
    [Header("Day/Night Cycle")]
    private int _day;
    // Text mesh pro for the day counter
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private GameObject newDayText;
    [SerializeField] private Light2D globalLight;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        var essence = PlayerController.Instance.GetEssence();
        _homeEssence += essence;
        homeEssenceText.enabled = false;
        homeEssenceText.text = _homeEssence.ToString();
        homeEssenceText.enabled = true;

        // Reset the player's essence
        PlayerController.Instance.ResetEssence();
        
        // Reset the player's abilities
        PlayerController.Instance.ResetAbilities();
        
        PlayerController.Instance.GoToSleep();
        
        GameManager.Instance.SetCursorDefault();
    }
    
    public int GetEssence()
    {
        return _homeEssence;
    }
    
    public void SpendEssence(int amount)
    {
        _homeEssence -= amount;
        homeEssenceText.enabled = false;
        homeEssenceText.text = _homeEssence.ToString();
        homeEssenceText.enabled = true;
    }
    
    public void NewDay()
    {
        _day++;
        dayText.text = "Day " + _day;

        // Play audio
        AudioManager.PlaySound(AudioManager.Sound.NewDay, transform.position);
        AudioManager.PlaySound(AudioManager.Sound.OdeRevive, transform.position);

        // Brighten the light
        StartCoroutine(BrightenLight());
        
        // Display new day in bold
        newDayText.SetActive(true);
        StartCoroutine(FadeInNewDayText());
        newDayText.GetComponentInChildren<TextMeshProUGUI>().text = "Day " + _day;
        StartCoroutine(FadeOutNewDayText());
        
        // Save the game
        DataPersistenceManager.Instance.SaveGame();
    }

    private IEnumerator BrightenLight()
    {
        // Increase the light intensity to 1 over 2 seconds
        // also change the color to white over 2 seconds
        var currentIntensity = globalLight.intensity;
        var currentColor = globalLight.color;
        
        var newColor = new Color(1f, 0.98f, 0.93f, 1f);
        
        var t = 0f;
        while (t < 2f)
        {
            t += Time.deltaTime;
            globalLight.intensity = Mathf.Lerp(currentIntensity, 1f, t / 2f);
            globalLight.color = Color.Lerp(currentColor, newColor, t / 2f);
            yield return null;
        }
    }

    private IEnumerator FadeInNewDayText()
    {
        var alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime;
            newDayText.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, alpha);
            newDayText.GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
    }
    
    private IEnumerator FadeOutNewDayText()
    {
        // Wait 2 seconds and then fade out the text
        yield return new WaitForSeconds(0.7f);
        
        var alpha = 2f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime;
            newDayText.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, alpha);
            newDayText.GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
        
        newDayText.SetActive(false);
    }
    
    public void LoadData(GameData data)
    {
        // Load the home essence
        _homeEssence = data.HomeEssence;
        _day = data.Day;
        dayText.text = "Day " + _day;
    }

    public void SaveData(GameData data)
    {
        // Save the home essence
        data.HomeEssence = _homeEssence;
        data.Day = _day;
    }

    public bool FirstLoad()
    {
        return true;
    }
}
