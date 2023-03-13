using System;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    public AbilityEffect abilityEffect;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            abilityEffect.Apply(col.gameObject);
        }
    }
}
