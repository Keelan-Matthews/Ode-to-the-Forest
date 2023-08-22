using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeEssenceGatherController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        HomeRoomController.Instance.AddEssence(1);
        
        // Destroy the essence
        Destroy(gameObject);
    }
}
