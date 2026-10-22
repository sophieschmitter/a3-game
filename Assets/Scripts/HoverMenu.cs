using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Message shown when hovering over this button.
    [TextArea]
    public string description = "What this upgrade does.";

    // The ONE shared tooltip panel and its text. Every button points at the same one.
    public GameObject tooltipPanel;
    public TMP_Text tooltipText;

    // Gap between the button and the tooltip (x = sideways, y = up/down).
    public Vector2 offset = new Vector2(20f, 0f);

    private bool isHovered = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (tooltipPanel == null) return;

        if (tooltipText != null) tooltipText.text = description;

        RectTransform tip = (RectTransform)tooltipPanel.transform;
        RectTransform me = (RectTransform)transform;

        // Find the middle of this button's right edge.
        Vector3[] corners = new Vector3[4];
        me.GetWorldCorners(corners);   // 0 = bottom-left, 1 = top-left, 2 = top-right, 3 = bottom-right
        Vector3 rightMiddle = (corners[2] + corners[3]) / 2f;

        // Hold the tooltip by its left-middle point, and put that point next to the button.
        tip.pivot = new Vector2(0f, 0.5f);
        tip.position = rightMiddle + new Vector3(offset.x, offset.y, 0f) * tip.lossyScale.x;

        // Draw it on top of everything else.
        tip.SetAsLastSibling();

        // Make the mouse ignore the tooltip, so it can't "steal" the hover and flicker.
        CanvasGroup group = tooltipPanel.GetComponent<CanvasGroup>();
        if (group == null) group = tooltipPanel.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;

        tooltipPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    private void OnDisable()
    {
        // If the shop closes while hovering, don't leave the tooltip stuck on.
        if (isHovered && tooltipPanel != null) tooltipPanel.SetActive(false);
        isHovered = false;
    }
}