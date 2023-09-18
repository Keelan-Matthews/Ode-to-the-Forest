using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public bool isShaking;
    public AnimationCurve curve;
    public float duration = 1f;

    // Update is called once per frame
    void Update()
    {
        if (isShaking)
        {
            isShaking = false;
            StartCoroutine(Shake());
        }
    }
    
    public void ShakeCamera(float shakeDuration)
    {
        duration = shakeDuration;
        isShaking = true;
    }
    
    public IEnumerator Shake()
    {
        var startPos = transform.position;
        var elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            startPos = transform.position;
            elapsedTime += Time.deltaTime;
            var strength = curve.Evaluate(elapsedTime / duration);
            transform.position = startPos + Random.insideUnitSphere * strength;
            // Reset the z position
            transform.position = new Vector3(transform.position.x, transform.position.y, startPos.z);
            yield return null;
        }
        
        transform.position = startPos;
    }
}
