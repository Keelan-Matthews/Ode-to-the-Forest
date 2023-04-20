using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceScript : MonoBehaviour
{
    private int _essenceValue = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Add essence to the player
            other.gameObject.GetComponent<PlayerController>().AddEssence(_essenceValue);
            // Destroy the essence
            Destroy(gameObject);
        }
    }
}
