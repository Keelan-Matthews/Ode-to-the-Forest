using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public bool isShaking;
    public bool isSmall;
    public AnimationCurve curve;
    public AnimationCurve smallCurve;
    private AnimationCurve _usedCurve;
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
    
    public void ShakeCamera(float shakeDuration, bool isSmall = false)
    {
        duration = shakeDuration;
        if (isSmall)
        {
            _usedCurve = smallCurve;
        }
        else
        {
            _usedCurve = curve;
        }
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
            var strength = _usedCurve.Evaluate(elapsedTime / duration);
            transform.position = startPos + Random.insideUnitSphere * strength;
            // Reset the z position
            transform.position = new Vector3(transform.position.x, transform.position.y, startPos.z);
            yield return null;
        }
        
        transform.position = startPos;
    }
}
