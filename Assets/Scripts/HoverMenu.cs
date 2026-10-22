using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(UpgradeButton))]
public class HoverMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Message shown when hovering over the unlocked button.
    [TextArea]
    public string description = "What this upgrade does.";

    // The tooltip panel and its text. One shared panel works for every button.
    public GameObject tooltipPanel;
    public TMP_Text tooltipText;

    private UpgradeButton button;
    private bool isHovered = false;

    private void Awake()
    {
        button = GetComponent<UpgradeButton>();
    }

    private void Update()
    {
        // Show only while hovering AND unlocked. Checking every frame
        // also hides it if the button re-locks.
        if (isHovered && button.IsUnlocked)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        Hide();
    }

    private void OnDisable()
    {
        // If the shop closes while hovering, don't leave the tooltip stuck on.
        isHovered = false;
        Hide();
    }

    private void Show()
    {
        if (tooltipPanel == null) return;

        if (tooltipText != null)
        {
            tooltipText.text = description;
        }
        tooltipPanel.SetActive(true);
    }

    private void Hide()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }
}