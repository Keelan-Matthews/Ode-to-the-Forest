using System.Collections;
using System.Collections.Generic;
using Runtime.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrystalBallController : MonoBehaviour
{
    [SerializeField] private Interactable interactable;
    [SerializeField] private Animator animator;

    [SerializeField] private int cost;

    public bool used;
    private static readonly int Clairvoyance = Animator.StringToHash("Clairvoyance");
    private static readonly int GoodLuck = Animator.StringToHash("GoodLuck");
    private static readonly int Marker = Animator.StringToHash("Marker");
    
    [Header("Ability information references")]
    [SerializeField] private GameObject abilityInformation;
    [SerializeField] private TextMeshProUGUI abilityName;
    [SerializeField] private TextMeshProUGUI abilityDescription;
    [SerializeField] private Image abilityIcon;

    private int _abilityNum;

    private void Awake()
    {
        interactable.SetCost(cost);
    }

    public void Interact()
    {
        if (GameManager.Instance.activeDialogue || used) return;
        var homeEssence = HomeRoomController.Instance.GetEssence();
        if (homeEssence < cost)
        {
            // Trigger not enough essence
            interactable.TriggerCannotAfford();
            return;
        }
        
        // Remove the essence
        HomeRoomController.Instance.SpendEssence(cost);

        Predict();
            
        used = true;
        interactable.SetInteractable(false);
        interactable.HidePromptText();
    }
    

    private void Predict()
    {
        _abilityNum = Random.Range(0, 3);
        
        switch (_abilityNum)
        {
            case 0:
                animator.SetTrigger(Clairvoyance);
                break;
            case 1:
                animator.SetTrigger(GoodLuck);
                break;
            case 2:
                animator.SetTrigger(Marker);
                break;
        }
    }

    public void ShowAbilityInfo()
    {
        if (!used) return;
        abilityInformation.SetActive(false);
        abilityInformation.SetActive(true);
        
        AudioManager.PlaySound(AudioManager.Sound.ShowMenu, transform.position);

        var abilityEffect = AbilityManager.Instance.GetOracleAbility(_abilityNum);
        
        var seed = PermaSeedManager.Instance.GetSpecificPermaSeed(abilityEffect.abilityName);
        if (seed != null)
        {
            PermaSeedManager.Instance.AddActiveSeed(seed);
        }
        
        abilityName.text = abilityEffect.abilityName;
        
        abilityDescription.text = abilityEffect.description;
        
        abilityIcon.sprite = abilityEffect.icon;
        
        // Force canvas update
        Canvas.ForceUpdateCanvases();
        var layoutGroups = abilityInformation.GetComponentsInChildren<HorizontalLayoutGroup>();
        // Disable the layout groups and then re-enable them to force the text to wrap
        foreach (var layoutGroup in layoutGroups)
        {
            layoutGroup.enabled = false;
            layoutGroup.enabled = true;
        }
        
        abilityInformation.GetComponent<Animator>().SetTrigger("Show");
    }
}
