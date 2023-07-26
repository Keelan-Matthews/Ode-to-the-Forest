using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    private TargetManager _targetManager;
    public bool isHit;
    private Animator _targetAnimator;
    private static readonly int Shot = Animator.StringToHash("Shot");

    private void Awake()
    {
        _targetManager = transform.parent.parent.GetComponent<TargetManager>();
        
        // Get the parent's animator
        _targetAnimator = transform.parent.GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Bullet") || isHit) return;
        _targetManager.IncrementTargetsShot();
        _targetAnimator.SetTrigger(Shot);
        AudioManager.PlaySound(AudioManager.Sound.EnemyHit, transform.position);
        isHit = true;
    }
}
