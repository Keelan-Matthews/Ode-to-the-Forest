using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlmanacController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject canvasBook;
    [SerializeField] private GameObject abilityInformation;
    [SerializeField] private TextMeshProUGUI abilityName;
    [SerializeField] private TextMeshProUGUI abilityDescription;
    [SerializeField] private Image abilityIcon;
    private bool _open = false;
    
    // Cooldown for the book opening animation
    private float _cooldown = 0.5f;
    
    private void Awake()
    {
        // Get the Interactable component
        var interactable = GetComponentInChildren<Interactable>();
        
        // Set the prompt text
        interactable.SetCost(0);
    }
    
    public void Interact()
    {
        if (_open) return;
        canvasBook.SetActive(true);
        PlayerController.Instance.canMove = false;
        AudioManager.PlaySound(AudioManager.Sound.ShowMenu, transform.position);
        StartCoroutine(OpenCooldown());
    }

    private IEnumerator OpenCooldown()
    {
        yield return new WaitForSeconds(_cooldown);
        _open = true;
    }
    
    private IEnumerator CloseCooldown()
    {
        yield return new WaitForSeconds(_cooldown);
        _open = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
        {
            if (!_open) return;
            canvasBook.SetActive(false);
            PlayerController.Instance.canMove = true;
            StartCoroutine(CloseCooldown());
        }
    }
    
    public void OpenBook()
    {
        animator.SetTrigger("OpenBook");
        GameManager.Instance.AlmanacOpen = true;
    }
    
    public void CloseBook()
    {
        animator.SetTrigger("CloseBook");
        GameManager.Instance.AlmanacOpen = false;
        if (abilityInformation.activeSelf)
        {
            DisableAbilityInformation();
        }
    }
    
    public void DisplayAbilityStats(AbilityEffect abilityEffect)
    {
        abilityInformation.SetActive(false);
        abilityInformation.SetActive(true);
        AudioManager.PlaySound(AudioManager.Sound.ShowMenu, transform.position);

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
    
    public void DisableAbilityInformation()
    {
        abilityInformation.SetActive(false);
        abilityInformation.GetComponent<Animator>().SetTrigger("Hide");
    }
}
