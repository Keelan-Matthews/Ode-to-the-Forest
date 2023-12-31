using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceScript : MonoBehaviour
{
    private const int EssenceValue = 1;
    private const float EssenceTravelSpeed = 10f;
    private const float HomeEssenceTravelSpeed = 50f;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the essence collides with the essence collector,
        // move the essence toward the player until it is destroyed
        if (other.gameObject.CompareTag("EssenceCollector"))
        {
            // If this is inactive, return
            if (!gameObject.activeSelf) return;
            StartCoroutine(MoveTowardsPlayer(other.gameObject));
        }

        if (other.gameObject.CompareTag("HomeEssenceCollector") && ScenesManager.Instance.currentSceneName.Equals("Home"))
        {
            if (!gameObject.activeSelf) return;

            StartCoroutine(MoveTowardsTarget(other.gameObject));
        }

        if (other.gameObject.CompareTag("Player") && !ScenesManager.Instance.currentSceneName.Equals("Home"))
        {
            // Add essence to the player
            other.gameObject.GetComponent<PlayerController>().AddEssence(EssenceValue);
            // Set essence to false
            gameObject.SetActive(false);
        }
        
        if (other.gameObject.CompareTag("AmountTarget"))
        {
            HomeRoomController.Instance.AddEssence(1);
            // Set essence to false
            gameObject.SetActive(false);
        }
    }

    private IEnumerator MoveTowardsPlayer(GameObject player)
    {
        while (gameObject.activeSelf)
        {
            // Move the essence toward the player
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, EssenceTravelSpeed * Time.deltaTime);
            yield return null;
        }
    }
    
    private IEnumerator MoveTowardsTarget(GameObject otherGameObject)
    {

        while (gameObject.activeSelf)
        {
            // Move the essence toward the player
            transform.position = Vector2.MoveTowards(transform.position, otherGameObject.transform.position, HomeEssenceTravelSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
