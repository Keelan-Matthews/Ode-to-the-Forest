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

    [Header("Day/Night Cycle")] private int _day;

    // Text mesh pro for the day counter
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private GameObject newDayText;
    [SerializeField] private Light2D globalLight;

    private int _odeEssence;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
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

        StartCoroutine(EssenceToMother(_odeEssence, PlayerController.Instance.transform.position));
    }

    public void AddEssence(int amount)
    {
        _homeEssence += amount;
        homeEssenceText.enabled = false;
        homeEssenceText.text = _homeEssence.ToString();
        homeEssenceText.enabled = true;

        StartCoroutine(BounceText());
    }

    private IEnumerator BounceText()
    {
        // Scale the home essence text up and down over 0.1 seconds
        var t = 0f;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            var scale = Mathf.Lerp(1f, 1.1f, t / 0.1f);
            homeEssenceText.transform.localScale = new Vector3(scale, scale, 0);
            yield return null;
        }
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
        yield return new WaitForSeconds(0.2f);

        var alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime;
            newDayText.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, alpha);
            newDayText.GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        newDayText.SetActive(false);
    }

    private IEnumerator EssenceToMother(int amount, Vector2 playerPosition)
    {
        for (var i = 0; i < amount; i++)
        {
            var essence = EssencePooler.Instance.GetPooledObject();

            if (essence == null) continue;
            essence.transform.position = playerPosition;
            // Disable all colliders except the trigger
            essence.GetComponents<Collider2D>()[1].enabled = false;

            essence.SetActive(true);

            // Apply a force to it in the top left direction
            var rb = essence.GetComponent<Rigidbody2D>();

            var forceDirection = new Vector2(-1f, 1f); // Adjust values as needed
            forceDirection.Normalize(); // Normalize the vector to ensure consistent speed

            // Apply the force
            var forceMagnitude = 10f; // Adjust the force strength as needed
            rb.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);


            // Wait for 0.02 seconds before spawning the next essence
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void LoadData(GameData data)
    {
        // Load the home essence
        _homeEssence = data.HomeEssence;
        _odeEssence = data.Essence;
        _day = data.Day;
        dayText.text = "Day " + _day;

        homeEssenceText.enabled = false;
        homeEssenceText.text = _homeEssence.ToString();
        homeEssenceText.enabled = true;
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