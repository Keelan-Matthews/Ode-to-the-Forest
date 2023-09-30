using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainController : MonoBehaviour, IDataPersistence
{
    [SerializeField] private int index;
    [SerializeField] private bool isActivated;
    private Animator _animator;
    private static readonly int Activate = Animator.StringToHash("Activate");

    public static event Action OnFountainActivated;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    public void Interact()
    {
        if (isActivated) return;
        
        isActivated = true;
        // AudioManager.PlaySound(AudioManager.Sound.Fountain, transform.position);
        OnFountainActivated?.Invoke();
        _animator.SetTrigger(Activate);
        
        // Make not interactable
        GetComponentInChildren<Interactable>().SetInteractable(false);
    }


    public void LoadData(GameData data)
    {
        var active = data.fountainActivated[index];
        if (active)
        {
            Interact();
        }
    }

    public void SaveData(GameData data)
    {
        data.fountainActivated[index] = isActivated;
    }

    public bool FirstLoad()
    {
        return true;
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}
