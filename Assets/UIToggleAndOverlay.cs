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

    /// <summary>
    /// Read-only property used by other scripts to check if we’re on cooldown.
    /// </summary>
    public bool IsOnCooldown => isOnCooldown;

    private void Start()
    {
        // Make sure the panel starts out hidden
        if (colorSelectionPanel != null)
        {
            colorSelectionPanel.SetActive(false);
        }

        // Optionally ensure the overlay starts at 0 fill
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0f;
        }
    }

    /// <summary>
    /// Toggling the panel on each click.
    /// Attach this script to the UI object you want clicked.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // If we’re on cooldown, do nothing.
        if (isOnCooldown) return;

        // Otherwise toggle the panel
        isPanelVisible = !isPanelVisible;
        if (colorSelectionPanel != null)
        {
            colorSelectionPanel.SetActive(isPanelVisible);
        }
    }


    /// <summary>
    /// Call this from DragArrow (or elsewhere) to start the cooldown fill from 1 -> 0.
    /// </summary>
    public void StartCooldown()
    {
        if (isOnCooldown) return;
        
        isOnCooldown = true;
        StartCoroutine(CooldownRoutine());
    }

    /// <summary>
    /// Hides the color selection panel (e.g., after a successful upgrade).
    /// </summary>
    public void CloseColorPanel()
    {
        if (colorSelectionPanel != null)
        {
            colorSelectionPanel.SetActive(false);
        }
        isPanelVisible = false;
    }

    /// <summary>
    /// Coroutine that fills `cooldownOverlay` from 1 down to 0 over `cooldownDuration`.
    /// Once it’s finished, we clear the cooldown.
    /// </summary>
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
