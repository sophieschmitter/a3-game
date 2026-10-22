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

    // The Image this button recolors (drag the child, e.g. sprite1).
    public Image target;

    // The spawner that reacts to purchases (drag the floor here).
    public CharacterSpawner spawner;

    private Sprite originalSprite;
    private int timesPurchased = 0;

    // Public so UpgradeTooltip can read it.
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
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsUnlocked)
        {
            return;
        }

        Clickable.Pain -= cost;
        timesPurchased += 1;

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