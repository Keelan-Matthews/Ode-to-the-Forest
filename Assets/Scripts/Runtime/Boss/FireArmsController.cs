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
    // private static readonly int IsDamaging = Animator.StringToHash("isDamaging");
    
    private void Awake()
    {
        _aimPrefabRenderer = aimPrefab.GetComponent<SpriteRenderer>();
        _aimPrefabCollider = aimPrefab.GetComponent<CircleCollider2D>();
        // _aimPrefabAnimator = aimPrefab.GetComponent<Animator>();
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
        
        arms[_currentArm].GetComponent<Animator>().SetTrigger("Land");
        yield return new WaitForSeconds(0.2f);
        CameraController.Instance.GetComponentInParent<CameraShake>().ShakeCamera(0.7f);
        if (playerIsInsideAim)
        {
            PlayerController.Instance.TakeDamage(damage);
        }
        
        _aimPrefabRenderer.color = Color.white;
        _aimPrefabCollider.enabled = false;
        aimPrefab.SetActive(false);

        yield return new WaitForSeconds(1f);
        arms[_currentArm].GetComponent<Animator>().SetTrigger("Return");
    }

    private void ShootArm()
    {
        var randomIndex = Random.Range(0, arms.Length);
        var randomArm = arms[randomIndex];
        _currentArm = randomIndex;
        var armAnimator = randomArm.GetComponent<Animator>();
        armAnimator.SetTrigger("Shoot");
    }
}
