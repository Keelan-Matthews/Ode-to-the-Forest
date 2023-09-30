using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineOfYouthController : MonoBehaviour, IDataPersistence
{
    private int _numFountainsActivated;
    private Animator _animator;
    private static readonly int On1 = Animator.StringToHash("On1");
    private static readonly int On2 = Animator.StringToHash("On2");
    private static readonly int On3 = Animator.StringToHash("On3");
    private static readonly int On4 = Animator.StringToHash("On4");

    private void Awake()
    {
        FountainController.OnFountainActivated += IncrementFountainsActivated;
        _animator = GetComponent<Animator>();
        GetComponentInChildren<Interactable>().SetInteractable(false);
    }

    private void IncrementFountainsActivated()
    {
        _numFountainsActivated++;

        switch (_numFountainsActivated)
        {
            case 0:
                _animator.SetTrigger(On1);
                break;
            case 1:
                _animator.SetTrigger(On2);
                break;
            case 2:
                _animator.SetTrigger(On3);
                break;
            case 3:
                _animator.SetTrigger(On4);
                break;
            default:
                break;
        }
    }

    public void LoadData(GameData data)
    {
        // In data.fountainsActivated, count the number of true values
        foreach (var fountainActivated in data.fountainActivated)
        {
            if (fountainActivated)
            {
                IncrementFountainsActivated();
            }
        }
    }

    public void SaveData(GameData data)
    {
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
