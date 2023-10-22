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
        
        // Call Dim on SunlightCOntroller
        var sunlightController = GetComponentInChildren<SunlightController>();
        sunlightController.Dim();

        yield return new WaitForSeconds(1f);
        sunlightController.StopCoroutines();
        Destroy(gameObject);
    }
}
