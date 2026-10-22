using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Top row (usual)")]
    public Sprite normal;
    public Sprite hover;
    public Sprite pressed;

    [Header("Bottom row (something new)")]
    public Sprite newNormal;
    public Sprite newHover;
    public Sprite newPressed;

    [Header("Setup")]
    // The shop panel. Opening it counts as "viewing" what's new.
    public GameObject shopPanel;
    // Seconds between flashes.
    public float flashSpeed = 0.4f;

    private Image image;
    private UpgradeButton[] upgrades;
    // Upgrades that were buyable the last time the shop was open.
    private HashSet<UpgradeButton> seen = new HashSet<UpgradeButton>();

    private bool isHovered = false;
    private bool isPressed = false;
    private bool flashOn = false;
    private float flashTimer = 0f;

    private void Awake()
    {
        image = GetComponent<Image>();
        // Find every upgrade in the scene, even while the shop is closed (inactive).
        upgrades = FindObjectsByType<UpgradeButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    private void Update()
    {
        bool shopOpen = shopPanel != null && shopPanel.activeInHierarchy;
        bool somethingNew = false;

        if (shopOpen)
        {
            // Shop is open: remember every unbought floor the player can unlock right now.
            foreach (UpgradeButton u in upgrades)
            {
                if (u.IsUnlocked && !u.HasBeenBought) seen.Add(u);
            }
        }
        else
        {
            // Shop is closed: is there a floor ready to unlock that the player hasn't seen yet?
            foreach (UpgradeButton u in upgrades)
            {
                if (u.IsUnlocked && !u.HasBeenBought && !seen.Contains(u))
                {
                    somethingNew = true;
                    break;
                }
            }
        }

        // Flash between the rows while something is new.
        if (somethingNew)
        {
            flashTimer += Time.deltaTime;
            if (flashTimer >= flashSpeed)
            {
                flashTimer = 0f;
                flashOn = !flashOn;
            }
        }
        else
        {
            flashOn = false;
            flashTimer = 0f;
        }

        // Pick the picture: which row (flash) and which column (mouse).
        Sprite wanted;
        if (isPressed) wanted = flashOn ? newPressed : pressed;
        else if (isHovered) wanted = flashOn ? newHover : hover;
        else wanted = flashOn ? newNormal : normal;

        if (wanted != null) image.sprite = wanted;
    }

    public void OnPointerEnter(PointerEventData eventData) { isHovered = true; }
    public void OnPointerExit(PointerEventData eventData) { isHovered = false; isPressed = false; }
    public void OnPointerDown(PointerEventData eventData) { isPressed = true; }
    public void OnPointerUp(PointerEventData eventData) { isPressed = false; }
}