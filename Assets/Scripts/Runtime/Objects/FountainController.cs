using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainController : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private bool isActivated;
    [SerializeField] private int cost;
    private Animator _animator;
    private static readonly int Activate = Animator.StringToHash("Activate");

    public static event Action OnFountainActivated;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        StartCoroutine(UpdateShrine());
    }
    
    private void OnEnable()
    {
        GameManager.OnDiscount += SetCost;
    }
    
    private void OnDisable()
    {
        GameManager.OnDiscount -= SetCost;
    }
    
    private IEnumerator UpdateShrine()
    {
        yield return new WaitForSeconds(3f);
        var active = GameManager.Instance.fountainsActivated[index];
        if (active)
        {
            Interact(true);
        }
    }
    
    public void Interact(bool noCost = false)
    {
        if (isActivated) return;
        
        var interactable = GetComponentInChildren<Interactable>();

        if (!noCost)
        {
            if (GameManager.Instance.IsSellYourSoul)
            {
                if (PlayerController.Instance.GetHealth() <= 8)
                {
                    interactable.TriggerCannotAfford();
                    return;
                }
            }
            else
            {
                if (PlayerController.Instance.GetEssence() < cost)
                {
                    interactable.TriggerCannotAfford();
                    return;
                }
            }
        }

        isActivated = true;
        GameManager.Instance.fountainsActivated[index] = true;

        if (!noCost)
        {
            AudioManager.PlaySound(AudioManager.Sound.Fountain, transform.position);
        }
        
        OnFountainActivated?.Invoke();
        _animator.SetTrigger(Activate);

        if (!noCost)
        {
            if (GameManager.Instance.IsSellYourSoul)
            {
                // Decrease the player's health by 1
                PlayerController.Instance.GetComponent<Health>().TakeDamage(8);
            }
            else
            {
                // Remove the essence from the player
                PlayerController.Instance.SpendEssence(cost);
            }
        }

        // Make not interactable
        GetComponentInChildren<Interactable>().SetInteractable(false);
    }
    
    public void SetCost(int discount)
    {
        cost -= discount;
        var interactable = GetComponentInChildren<Interactable>();
        interactable.SetCost(cost);
    }
}
