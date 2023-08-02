using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

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
        
        // Display new day in bold
        newDayText.SetActive(true);
        StartCoroutine(FadeInNewDayText());
        newDayText.GetComponentInChildren<TextMeshProUGUI>().text = "Day " + _day;
        StartCoroutine(FadeOutNewDayText());
    }
    
    private IEnumerator FadeInNewDayText()
    {
        var alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime;
            newDayText.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
    }
    
    private IEnumerator FadeOutNewDayText()
    {
        // Wait 2 seconds and then fade out the text
        yield return new WaitForSeconds(2f);
        
        var alpha = 2f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime;
            newDayText.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, alpha);
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
