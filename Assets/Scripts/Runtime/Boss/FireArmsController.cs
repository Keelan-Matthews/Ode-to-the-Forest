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
    
    private void Awake()
    {
        _aimPrefabRenderer = aimPrefab.GetComponent<SpriteRenderer>();
        _aimPrefabCollider = aimPrefab.GetComponent<CircleCollider2D>();
        
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
        aimPrefab.GetComponent<Animator>().SetTrigger("Die");
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
        _isFollowing = true;
        playerIsInsideAim = true;
        _timer = 0f;
        aimPrefab.SetActive(true);
        ShootArm();
        StartCoroutine(FollowPlayer());
    }
    
    private IEnumerator FollowPlayer()
    {
        while (_isFollowing)
        {
            _timer += Time.deltaTime;
            if (_timer >= timeToFollow)
            {
                _isFollowing = false;
                // _aimPrefabAnimator.SetBool(IsDamaging, true);
                _aimPrefabRenderer.color = Color.red;
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
        yield return new WaitForSeconds(damageDelay);
        
        arms[_currentArm].transform.localPosition = aimPrefab.transform.localPosition;

        // arms[_currentArm].GetComponent<Animator>().SetTrigger(Land);
        yield return new WaitForSeconds(0.2f);
        arms[_currentArm].GetComponent<Arm>().isExposed = true;
        CameraController.Instance.GetComponentInParent<CameraShake>().ShakeCamera(0.7f);
        if (playerIsInsideAim)
        {
            PlayerController.Instance.TakeDamage(damage);
        }
        
        _aimPrefabRenderer.color = Color.white;
        _aimPrefabCollider.enabled = false;
        aimPrefab.SetActive(false);

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
        
        // Reset the arm transform if it hasn't been destroyed
        if (_currentArm == -1) yield break;
        arms[_currentArm].transform.position = _initialArmPositions[_currentArm];
        arms[_currentArm].GetComponent<Animator>().SetTrigger(Return);
        arms[_currentArm].GetComponent<Arm>().isExposed = false;
    }

    private void ShootArm()
    {
        if (arms.Count == 0) return;
        var randomIndex = UnityEngine.Random.Range(0, arms.Count);
        var randomArm = arms[randomIndex];
        _currentArm = randomIndex;
        var armAnimator = randomArm.GetComponent<Animator>();
        armAnimator.SetTrigger(Shoot);
        aimPrefab.GetComponent<AimController>().SetArmState(randomArm.GetComponent<Arm>().stateNumber);
    }
}
