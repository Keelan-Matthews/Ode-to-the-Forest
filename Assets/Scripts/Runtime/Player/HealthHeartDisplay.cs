using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHeartDisplay : MonoBehaviour
{
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Health health;
    
    List<HealthHeart> hearts = new List<HealthHeart>();

    private void OnEnable()
    {
        Health.OnPlayerDamaged += DrawHearts;
    }
    
    private void OnDisable()
    {
        Health.OnPlayerDamaged -= DrawHearts;
    }

    private void Start()
    {
        DrawHearts();
    }

    public void DrawHearts()
    {
        ClearHearts();
        
        // Determine how many total hearts to make
        // based off of the max health
        float maxHealthRemainder = health.MaxHealth % 2; //Odd or even?
        int totalHearts = (int)(health.MaxHealth / 2 + maxHealthRemainder);
        
        for (var i = 0; i < totalHearts; i++)
        {
            CreateEmptyHeart();
        }
        
        // Set the heart states based on the current health
        for (var i = 0; i < hearts.Count; i++)
        {
            var heartStatusRemainder = Mathf.Clamp(health.HealthValue - (i*2), 0, 2);
            hearts[i].SetHeartState((HeartState)heartStatusRemainder);
        }
    }
    
    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab, transform);
        HealthHeart heartComponent = newHeart.GetComponent<HealthHeart>();
        heartComponent.SetHeartState(HeartState.Empty);
        hearts.Add(heartComponent);
    }
    
    public void ClearHearts()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts = new List<HealthHeart>();
    }
}
