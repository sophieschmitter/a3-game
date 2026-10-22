using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UpgradeButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // Name shown in Console messages. Set it per button.
    public string upgradeName = "Upgrade";

    // The upgrade that must be bought before this one. Leave empty for the first one.
    public UpgradeButton requires;

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

    [Header("Frame")]
    // The tile's border picture. Leave empty to use this button's own Image.
    public Image frame;
    // The glowing border, shown while hovering an upgrade you can afford.
    public Sprite frameHoverSprite;

    private Sprite originalSprite;
    private Sprite frameNormalSprite;
    private int timesPurchased = 0;
    private bool isHovered = false;

    // True once this upgrade has been bought at least once.
    public bool HasBeenBought => timesPurchased > 0;

    // True when the previous upgrade has been bought (or there isn't one).
    public bool IsAvailable => requires == null || requires.HasBeenBought;

    // True when the player can buy this right now: available AND affordable.
    public bool IsUnlocked => IsAvailable && Clickable.Pain >= cost;

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

        if (frame == null)
        {
            frame = GetComponent<Image>();
        }
        // Remember the normal border (whatever is in the frame's Source Image).
        frameNormalSprite = frame.sprite;
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

        UpdateFrame();
        UpdatePriceText();
    }

    private void UpdateFrame()
    {
        if (frame == null || frame == target) return;

        // Glow only while hovering AND buyable.
        bool glow = isHovered && IsUnlocked && frameHoverSprite != null;
        frame.sprite = glow ? frameHoverSprite : frameNormalSprite;

        // Grey the frame too when you can't buy it.
        frame.color = IsUnlocked ? Color.white : lockedColor;
    }

    private void UpdatePriceText()
    {
        if (priceText == null) return;

        if (!IsAvailable)
        {
            // The previous upgrade hasn't been bought yet.
            priceText.text = "Locked";
        }
        else if (timesPurchased == 0)
        {
            // Not bought yet: the first purchase unlocks the room and its first soul.
            priceText.text = "Unlock\n" + cost + " pain";
        }
        else
        {
            // Every purchase adds one soul, so souls = purchases.
            priceText.text = cost + " pain\n" + timesPurchased + " souls";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
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