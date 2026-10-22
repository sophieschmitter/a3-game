using UnityEngine;
using UnityEngine.UI;

public class CharacterSpawner : MonoBehaviour
{
    // The tier that gets un-greyed on the first purchase.
    public Image tier;

    // Color the tier starts with, before it's unlocked.
    public Color lockedTierColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    // The "under construction" cover shown until this floor is bought.
    public GameObject construction;

    // The character prefab spawned on every purchase.
    public GameObject characterPrefab;

    // The area where characters appear. Leave empty to use this object.
    public RectTransform spawnArea;

    // How high above the bottom of the room the souls stand.
    public float floorY = 0;

    // Small random up/down offset so they don't all stand in a perfectly flat line.
    public float staggerY = 8f;

    // Keeps souls this far from the left and right walls.
    public float edgePadding = 60f;

    // Most souls shown in this room. Extra souls still earn pain, they're just hidden.
    public int maxVisibleSouls = 15;

    private int spawnedCount = 0;

    private void Awake()
    {
        // Start the tier greyed out.
        if (tier != null)
        {
            tier.color = lockedTierColor;
        }

        // Start with the construction cover showing.
        if (construction != null)
        {
            construction.SetActive(true);
        }
    }

    // Called by UpgradeButton each time this upgrade is bought.
    public void OnPurchased(int purchaseNumber)
    {
        if (purchaseNumber == 1)
        {
            // First purchase: un-grey the room and take down the construction cover.
            if (tier != null)
            {
                tier.color = Color.white;
            }
            if (construction != null)
            {
                construction.SetActive(false);
            }
        }

        // Every purchase (including the first) adds a soul.
        SpawnCharacter();
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

        GameObject character = Instantiate(characterPrefab, spawnArea);
        spawnedCount += 1;

        if (spawnedCount > maxVisibleSouls)
        {
            // Room is full: keep the soul (it still earns pain) but don't draw it.
            foreach (Graphic picture in character.GetComponentsInChildren<Graphic>())
            {
                picture.enabled = false;
            }
            return;
        }

        // Pick a random spot along the floor, staying away from the walls.
        float halfWidth = spawnArea.rect.width / 2f - edgePadding;
        float x = Random.Range(-halfWidth, halfWidth);
        float y = floorY + Random.Range(-staggerY, staggerY);

        character.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
    }
}