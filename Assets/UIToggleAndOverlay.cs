using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIToggleAndOverlay : MonoBehaviour, IPointerClickHandler
{
    [Header("Color Selection Panel (Initially Hidden)")]
    [SerializeField] private GameObject colorSelectionPanel;

    [Header("Cooldown Overlay Image (Filled)")]
    [SerializeField] private Image cooldownOverlay;
    
    [Header("Cooldown Settings")]
    [SerializeField] private float cooldownDuration = 60f;

    private bool isOnCooldown = false;
    private bool isPanelVisible = false;

    public bool IsOnCooldown => isOnCooldown;

    private void Start()
    {
        if (colorSelectionPanel != null)
        {
            colorSelectionPanel.SetActive(false);
        }

        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0f;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isOnCooldown) return;

        isPanelVisible = !isPanelVisible;
        if (colorSelectionPanel != null)
        {
            colorSelectionPanel.SetActive(isPanelVisible);
        }
    }


    public void StartCooldown()
    {
        if (isOnCooldown) return;
        
        isOnCooldown = true;
        StartCoroutine(CooldownRoutine());
    }

    public void CloseColorPanel()
    {
        if (colorSelectionPanel != null)
        {
            colorSelectionPanel.SetActive(false);
        }
        isPanelVisible = false;
    }


    private IEnumerator CooldownRoutine()
    {
        // Fill at 100% to indicate the start of cooldown
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 1f;
        }

        float remainingTime = cooldownDuration;
        while (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            if (cooldownOverlay != null)
            {
                // Update fill to reflect how much time remains
                cooldownOverlay.fillAmount = remainingTime / cooldownDuration;
            }
            yield return null;
        }

        // Cooldown finished
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0f;
        }
        isOnCooldown = false;
    }
}
