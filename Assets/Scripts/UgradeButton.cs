using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    // Name shown in the Console messages. Set it per button.
    public string upgradeName = "Upgrade";

    // How much pain this upgrade costs. The button unlocks once
    // you have at least this much, and buying it spends it.
    public int cost = 100;

    // Color while locked and unlocked.
    public Color lockedColor = Color.grey;
    public Color unlockedColor = Color.white;

    // The tier sprite this upgrade unlocks on its first purchase.
    // Drag the matching tier (tier 1, tier 2...) into this slot.
    public SpriteRenderer tier;

    private SpriteRenderer spriteRenderer;

    // How many times this upgrade has been bought.
    private int timesPurchased = 0;

    // True once the player can afford the upgrade.
    private bool CanAfford => Clickable.Pain >= cost;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Grey until the player has enough pain.
        spriteRenderer.color = CanAfford ? unlockedColor : lockedColor;
    }

    private void OnMouseDown()
    {
        // Ignore clicks while the button is still locked.
        if (!CanAfford)
        {
            return;
        }

        // Spend the pain.
        Clickable.Pain -= cost;
        timesPurchased += 1;

        if (timesPurchased == 1)
        {
            // First purchase: unlock the tier. Swap in Carson's art later.
            tier.color = Color.white;
        }
        else
        {
            // Every purchase after the first: add a tortured soul. Add later.
            Debug.Log(upgradeName + " purchased again");
        }
    }
}
