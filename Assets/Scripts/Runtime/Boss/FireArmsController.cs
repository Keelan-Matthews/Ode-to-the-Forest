using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArmsController : MonoBehaviour
{
    [SerializeField] private GameObject aimPrefab;
    [SerializeField] private float timeToFollow;
    [SerializeField] private float damageDelay;
    [SerializeField] private int damage;
    [SerializeField] private GameObject[] arms;

    private Vector2[] _initialArmPositions;
    
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
    private static readonly int Land = Animator.StringToHash("Land");
    
    private void Awake()
    {
        _aimPrefabRenderer = aimPrefab.GetComponent<SpriteRenderer>();
        _aimPrefabCollider = aimPrefab.GetComponent<CircleCollider2D>();
        
        _initialArmPositions = new Vector2[arms.Length];

        for (var i = 0; i < arms.Length; i++)
        {
            _initialArmPositions[i] = arms[i].transform.position;
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
        
        // Set the current arm to the same transform as where the aimPrefab is
        arms[_currentArm].transform.position = aimPrefab.transform.position;
        
        arms[_currentArm].GetComponent<Animator>().SetTrigger(Land);
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

        yield return new WaitForSeconds(4f);
        
        // Reset the arm transform
        arms[_currentArm].transform.position = _initialArmPositions[_currentArm];
        arms[_currentArm].GetComponent<Animator>().SetTrigger(Return);
        arms[_currentArm].GetComponent<Arm>().isExposed = false;
    }

    private void ShootArm()
    {
        // var randomIndex = Random.Range(0, arms.Length);
        var randomIndex = 0;
        var randomArm = arms[randomIndex];
        _currentArm = randomIndex;
        var armAnimator = randomArm.GetComponent<Animator>();
        armAnimator.SetTrigger(Shoot);
    }
}
