using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeButton : MonoBehaviour, IPointerClickHandler
{
    // Name shown in Console messages. Set it per button.
    public string upgradeName = "Upgrade";

    // Pain needed to unlock this button, and the amount spent when clicked.
    public int cost = 100;

    // Solid color shown while locked.
    public Color lockedColor = Color.grey;

    // Image shown once unlocked.
    public Sprite unlockedSprite;

    // Optional: the Image to recolor. Leave empty to use this object's own Image.
    // If the visible picture is the child (sprite1), drag that child here.
    public Image target;

    // The tier this upgrade unlocks on its first purchase.
    public Image tier;

    private Sprite originalSprite;
    private int timesPurchased = 0;

    private bool IsUnlocked => Clickable.Pain >= cost;

    private void Awake()
    {
        if (target == null)
        {
            target = GetComponent<Image>();
        }
        originalSprite = target.sprite;
    }

    private void Update()
    {
        if (IsUnlocked)
        {
            if (unlockedSprite != null)
            {
                target.sprite = unlockedSprite;
            }
            target.color = Color.white;
        }
        else
        {
            target.sprite = originalSprite;
            target.color = lockedColor;
        }
    }

    // Replaces OnMouseDown for UI objects.
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked: " + upgradeName + " (cost " + cost + ")");
        if (!IsUnlocked)
        {
            return;
        }

        Clickable.Pain -= cost;
        timesPurchased += 1;

        if (timesPurchased == 1)
        {
            if (tier != null)
            {
                tier.color = Color.white;
            }
        }
        else
        {
            Debug.Log(upgradeName + " purchased again");
        }
    }
}