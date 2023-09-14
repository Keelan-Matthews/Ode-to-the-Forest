using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunlightDestroyer : MonoBehaviour
{
    public void DestroySunlight(float seconds)
    {
        StartCoroutine(DestroyAfterSeconds(seconds));
    }
    
    private IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
