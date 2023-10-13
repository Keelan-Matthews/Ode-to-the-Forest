using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossPortalController : MonoBehaviour
{
    private static readonly int Spawn1 = Animator.StringToHash("Spawn");

    private void Start()
    {
        // Get the interactable child
        var interactable = transform.GetComponentInChildren<Interactable>();
        interactable.SetCost(0);
    }

    public void Interact()
    {
        DeathScreenController.Instance.TriggerScreen(true);
        // DataPersistenceManager.Instance.SaveGame();
        // PermaSeedManager.Instance.UnapplyPermaSeedEffects();
        // PlayerController.Instance.UnapplyAllAbilities();
        // GameManager.OnGameContinue();
    }

    public void Spawn()
    {
        GetComponent<Animator>().SetTrigger(Spawn1);
        
        StartCoroutine(IncreaseLightIntensity());
    }
    
    private IEnumerator IncreaseLightIntensity()
    {
        var light = GetComponentInChildren<Light2D>();
        var originalIntensity = light.intensity;
        light.intensity = 0f;
        var duration = 0.5f;
        
        while (light.intensity < originalIntensity)
        {
            light.intensity += Time.deltaTime / duration;
            yield return null;
        }
    }
}
