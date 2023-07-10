using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    private TargetManager _targetManager;
    public bool isHit;
    
    private void Awake()
    {
        _targetManager = transform.parent.GetComponent<TargetManager>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Bullet") || isHit) return;
        _targetManager.IncrementTargetsShot();
        isHit = true;
    }
}
