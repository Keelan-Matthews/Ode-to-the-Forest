using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArmsController : MonoBehaviour
{
    [SerializeField] private GameObject aimPrefab;
    [SerializeField] private float timeToFollow;
    [SerializeField] private float damageDelay;
    [SerializeField] private int damage;
    [SerializeField] private List<GameObject> arms;

    public static event Action OnArmDestroyed;

    private List<Vector2> _initialArmPositions;
    
    private int _currentArm;

    // This class will make the aimPrefab follow the player for timeToFollow seconds,
    // Then it will stop wherever the player was when the timer ended.
    // After that, it will damage the player after the damageDelay if the player is still colliding with the aimPrefab.
    
    private bool _isFollowing;
    public bool playerIsInsideAim;
    private bool _isDone;
    private float _timer;
    private SpriteRenderer _aimPrefabRenderer;
    private CircleCollider2D _aimPrefabCollider;
    private Animator _aimPrefabAnimator;

    private static readonly int Return = Animator.StringToHash("Return");
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int Strike = Animator.StringToHash("Strike");
    private static readonly int LockOn = Animator.StringToHash("LockOn");
    private static readonly int Expose = Animator.StringToHash("Expose");

    private void Awake()
    {
        _aimPrefabRenderer = aimPrefab.GetComponent<SpriteRenderer>();
        _aimPrefabCollider = aimPrefab.GetComponent<CircleCollider2D>();
        _aimPrefabAnimator = aimPrefab.GetComponent<Animator>();
        
        _initialArmPositions = new List<Vector2>();

        for (var i = 0; i < arms.Count; i++)
        {
            _initialArmPositions.Add(arms[i].transform.position);
        }
        
        // Subscribe to on arm destroy
        OnArmDestroyed += KillAimSprite;
    }

    private void KillAimSprite()
    {
        // Make sure it is the last state
        aimPrefab.GetComponent<AimController>().SetArmState(3);
        _aimPrefabAnimator.SetTrigger(Die);
        
        // Reset the alpha of the aimPrefab after 0.5 seconds
        StartCoroutine(ResetAlpha());
    }

    private IEnumerator ResetAlpha()
    {
        yield return new WaitForSeconds(4f);
        
        _aimPrefabRenderer.color = new Color(1, 1, 1, 1);
    }

    public int GetTotalHealth()
    {
        var totalHealth = 0;
        for (var i = 0; i < arms.Count; i++)
        {
            totalHealth += arms[i].GetComponent<Arm>().armHitPoints;
        }
        return totalHealth;
    }

    public void RemoveArm(int armIndex)
    {
        _aimPrefabAnimator.SetTrigger(Die);
        
        // find the arm in the list with .index == armIndex
        for (var i = 0; i < arms.Count; i++)
        {
            if (arms[i].GetComponent<Arm>().index != armIndex) continue;
            if (arms.Count == 1)
            {
                armIndex = 0;
            }
            arms.RemoveAt(armIndex);
            OnArmDestroyed?.Invoke();
        
            // Reset the current arm to 0
            _currentArm = -1;
        }
    }
    
    public void StartFollowing()
    {
        if (PlayerController.Instance.IsDead()) return;
        // Reset triggers if they need to be
        _aimPrefabAnimator.ResetTrigger(Return);
        _aimPrefabAnimator.ResetTrigger(Shoot);
        _aimPrefabAnimator.ResetTrigger(Die);
        _aimPrefabAnimator.ResetTrigger(Strike);
        _aimPrefabAnimator.ResetTrigger(LockOn);
        
        // Get the animator on each arm and reset the shoot and return triggers
        for (var i = 0; i < arms.Count; i++)
        {
            arms[i].GetComponent<Animator>().ResetTrigger(Shoot);
            arms[i].GetComponent<Animator>().ResetTrigger(Return);
        }

        _isFollowing = true;
        playerIsInsideAim = true;
        _timer = 0f;
        aimPrefab.SetActive(true);
        ShootArm();
        StartCoroutine(FollowPlayer());
    }
    
    private IEnumerator FollowPlayer()
    {
        AudioManager.PlaySound(AudioManager.Sound.TrackOde, transform.position);
        
        // Change the sort layer of the aimPrefab to the Floor, with a value of 5
        _aimPrefabRenderer.sortingLayerName = "Floor";
        _aimPrefabRenderer.sortingOrder = 5;
        
        while (_isFollowing)
        {
            _timer += Time.deltaTime;
            if (_timer >= timeToFollow)
            {
                _isFollowing = false;
                _aimPrefabCollider.enabled = true;
                
                // Start the damage timer
                StartCoroutine(DamagePlayer());
            }
            else
            {
                aimPrefab.transform.position = PlayerController.Instance.transform.position;
            }
            yield return null;
        }
    }
    
    private IEnumerator DamagePlayer()
    {
        // Wait for the damage delay seconds
        _aimPrefabAnimator.SetTrigger(LockOn);
        AudioManager.PlaySound(AudioManager.Sound.LockOnOde, transform.position);
        yield return new WaitForSeconds(damageDelay);
        _aimPrefabAnimator.SetTrigger(Strike);
        aimPrefab.GetComponent<AimController>().EnableCollider(true);
        yield return new WaitForSeconds(0.2f);
        AudioManager.PlaySound(AudioManager.Sound.ArmStrike, transform.position);
        
        arms[_currentArm].GetComponent<Arm>().isExposed = true;
        _aimPrefabAnimator.SetTrigger(Expose);
        _aimPrefabRenderer.sortingLayerName = "Player";
        _aimPrefabRenderer.sortingOrder = 0;
        CameraController.Instance.GetComponentInParent<CameraShake>().ShakeCamera(0.7f);
        if (playerIsInsideAim)
        {
            PlayerController.Instance.TakeDamage(damage);
        }
        
        _aimPrefabCollider.enabled = false;

        var elapsedTime = 0f;
        var maxWaitTime = 4f;

        while (elapsedTime < maxWaitTime)
        {
            // Check if the boss has died
            if (BossController.Instance.isDead) break;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Yield each frame before checking again
            yield return null;
        }
        aimPrefab.GetComponent<AimController>().EnableCollider(false);
        if (!BossController.Instance.isDead)
        {
            _aimPrefabAnimator.SetTrigger(Return);  
        }
        yield return new WaitForSeconds(0.5f);
        aimPrefab.SetActive(false);

        if (_currentArm == -1) yield break;
        arms[_currentArm].transform.position = _initialArmPositions[_currentArm];
        if (_currentArm == -1) yield break;
        arms[_currentArm].GetComponent<Arm>().UpdateState();
        arms[_currentArm].GetComponent<Animator>().SetTrigger(Return);
        if (_currentArm == -1) yield break;
        arms[_currentArm].GetComponent<Arm>().isExposed = false;
    }

    public void TakeDamage(int damage)
    {
        if (_currentArm == -1) return;
        if (arms.Count == 0) return;
        arms[_currentArm].GetComponent<Arm>().TakeDamage(damage);
    }

    private void ShootArm()
    {
        if (arms.Count == 0) return;
        var randomIndex = arms.Count - 1;
        var randomArm = arms[randomIndex];
        _currentArm = randomIndex;
        var armAnimator = randomArm.GetComponent<Animator>();
        armAnimator.SetTrigger(Shoot);
        aimPrefab.GetComponent<AimController>().SetArmState(randomArm.GetComponent<Arm>().stateNumber);
    }
}
