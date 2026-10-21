using UnityEngine;
using UnityEngine.UI;

public class CharacterSpawner : MonoBehaviour
{
    // The tier that gets un-greyed on the first purchase.
    public Image tier;

    // Color the tier starts with, before it's unlocked.
    public Color lockedTierColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    // The character prefab spawned on every purchase after the first.
    public GameObject characterPrefab;

    // The area where characters appear. Leave empty to use this object.
    public RectTransform spawnArea;

    // Where the line starts, relative to the spawn area's center.
    // Y is the "floor" height. X is where the first character stands.
    public float startX = -200f;
    public float floorY = 0;

    // Horizontal gap between characters.
    public float spacing = 60f;

    // Small random vertical offset so the line looks staggered (0 = perfectly flat).
    public float staggerY = 8f;

    // When the line runs out of room, start a new row this far below.
    public float rowHeight = 30f;

    private int spawnedCount = 0;

    private void Awake()
    {
        // Start the tier greyed out.
        if (tier != null)
        {
            tier.color = lockedTierColor;
        }
    }

    // Called by UpgradeButton each time this upgrade is bought.
    public void OnPurchased(int purchaseNumber)
    {
        if (purchaseNumber == 1)
        {
            // First purchase: un-grey the tier.
            if (tier != null)
            {
                tier.color = Color.white;
            }
        }
        else
        {
            // Every purchase after the first: add a character.
            SpawnCharacter();
        }
    }

    private void SpawnCharacter()
    {
        if (spawnArea == null)
        {
            spawnArea = GetComponent<RectTransform>();
        }

        if (characterPrefab == null)
        {
            Debug.LogWarning(name + ": assign Character Prefab.");
            return;
        }

        // How many characters fit in one row before wrapping.
        float usableWidth = spawnArea.rect.width / 2f - startX;
        int perRow = Mathf.Max(1, Mathf.FloorToInt(usableWidth / spacing));

        int column = spawnedCount % perRow;
        int row = spawnedCount / perRow;

        float x = startX + column * spacing;
        float y = floorY - row * rowHeight + Random.Range(-staggerY, staggerY);

        GameObject character = Instantiate(characterPrefab, spawnArea);
        character.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

        // Characters further down the screen draw in front of the ones behind them.
        character.transform.SetSiblingIndex(character.transform.parent.childCount - 1);

        spawnedCount += 1;
    }
}