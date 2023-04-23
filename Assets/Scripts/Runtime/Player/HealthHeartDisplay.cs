using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHeartDisplay : MonoBehaviour
{
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Health health;
    private int _healthPerHeart = 4;
    
    List<HealthHeart> hearts = new ();

    private void OnEnable()
    {
        Health.OnPlayerDamaged += DrawHearts;
        Health.OnPlayerHealed += DrawHearts;
        Health.OnAddedHeart += DrawHearts;
    }
    
    private void OnDisable()
    {
        Health.OnPlayerDamaged -= DrawHearts;
        Health.OnPlayerHealed -= DrawHearts;
        Health.OnAddedHeart -= DrawHearts;
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
        float maxHealthRemainder = health.MaxHealth % _healthPerHeart; //Odd or even?
        int totalHearts = (int)(health.MaxHealth / _healthPerHeart + maxHealthRemainder);
        
        for (var i = 0; i < totalHearts; i++)
        {
            CreateEmptyHeart();
        }
        
        // Set the heart states based on the current health
        for (var i = 0; i < hearts.Count; i++)
        {
            var heartStatusRemainder = Mathf.Clamp(health.HealthValue - (i*_healthPerHeart), 0, _healthPerHeart);
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
