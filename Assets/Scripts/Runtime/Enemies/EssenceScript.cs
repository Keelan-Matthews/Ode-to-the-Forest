using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceScript : MonoBehaviour
{
    private int _essenceValue = 1;
    private float _essenceTravelSpeed = 10f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the essence collides with the essence collector,
        // move the essence toward the player until it is destroyed
        if (other.gameObject.CompareTag("EssenceCollector"))
        {
            StartCoroutine(MoveTowardsPlayer(other.gameObject));
        }
        
        if (other.gameObject.CompareTag("Player"))
        {
            // Add essence to the player
            other.gameObject.GetComponent<PlayerController>().AddEssence(_essenceValue);
            // Destroy the essence
            Destroy(gameObject);
        }
    }
    
    private IEnumerator MoveTowardsPlayer(GameObject player)
    {
        while (true)
        {
            // Move the essence toward the player
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, _essenceTravelSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
