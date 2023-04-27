using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceScript : MonoBehaviour
{
    private const int EssenceValue = 1;
    private const float EssenceTravelSpeed = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the essence collides with the essence collector,
        // move the essence toward the player until it is destroyed
        if (other.gameObject.CompareTag("EssenceCollector"))
        {
            StartCoroutine(MoveTowardsPlayer(other.gameObject));
        }

        if (!other.gameObject.CompareTag("Player")) return;
        // Add essence to the player
        other.gameObject.GetComponent<PlayerController>().AddEssence(EssenceValue);
            
        // Set essence to false
        gameObject.SetActive(false);
    }

    private IEnumerator MoveTowardsPlayer(GameObject player)
    {
        while (true)
        {
            // Move the essence toward the player
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, EssenceTravelSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
