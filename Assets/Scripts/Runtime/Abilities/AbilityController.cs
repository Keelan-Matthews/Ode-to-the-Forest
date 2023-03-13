using System;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    public AbilityEffect abilityEffect;

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("AbilityController.OnTriggerEnter2D");
        Destroy(gameObject);
        abilityEffect.Apply(col.gameObject);
    }
}
