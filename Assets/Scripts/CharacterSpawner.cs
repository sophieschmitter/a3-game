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
    Debug.Log("OnPurchased " + purchaseNumber + " on " + name + ", tier = " + (tier == null ? "EMPTY" : tier.name));

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

        // Pick a random spot inside the spawn area.
        Rect area = spawnArea.rect;
        float x = Random.Range(-area.width / 2f, area.width / 2f);
        float y = Random.Range(-area.height / 2f, area.height / 2f);

        GameObject character = Instantiate(characterPrefab, spawnArea);
        character.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
    }
}