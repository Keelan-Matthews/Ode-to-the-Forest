using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableUICollider : MonoBehaviour
{
    public Canvas tutorialCanvas;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialCanvas.enabled = true;
        }
    }
}
