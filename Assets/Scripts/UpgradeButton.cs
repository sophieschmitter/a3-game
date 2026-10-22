using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UpgradeButton : MonoBehaviour, IPointerClickHandler
{
    // Name shown in Console messages. Set it per button.
    public string upgradeName = "Upgrade";

    // Pain needed for the NEXT purchase. Goes up every time you buy.
    public int cost = 100;

    // How much the price grows after each purchase (1.15 = 15% more each time).
    public float priceGrowth = 1.15f;

    // Solid color shown while locked.
    public Color lockedColor = Color.grey;

    // Image shown once unlocked.
    public Sprite unlockedSprite;

    // The Image this button recolors (drag the child, e.g. sprite1).
    public Image target;

    // The spawner that reacts to purchases (drag the floor here).
    public CharacterSpawner spawner;

    // Optional: small text on the button showing the price and souls owned.
    public TMP_Text priceText;

    private Sprite originalSprite;
    private int timesPurchased = 0;

    // True when the player has enough pain to buy this.
    public bool IsUnlocked => Clickable.Pain >= cost;

    private void Awake()
    {
        if (spawner == null)
        {
            spawner = GetComponent<CharacterSpawner>();
        }

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

        UpdatePriceText();
    }

    private void UpdatePriceText()
    {
        if (priceText == null) return;

        if (timesPurchased == 0)
        {
            // Not bought yet: the first purchase unlocks the room.
            priceText.text = "Unlock\n" + cost + " pain";
        }
        else
        {
            // The first purchase only unlocked the room, so souls = purchases - 1.
            priceText.text = cost + " pain\n" + (timesPurchased - 1) + " souls";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsUnlocked)
        {
            return;
        }

        Clickable.Pain -= cost;
        timesPurchased += 1;

        // Raise the price for next time. CeilToInt rounds UP to a whole number.
        cost = Mathf.CeilToInt(cost * priceGrowth);

        if (spawner != null)
        {
            spawner.OnPurchased(timesPurchased);
        }
        else
        {
            Debug.LogWarning(upgradeName + ": no Spawner assigned.");
        }
    }
}