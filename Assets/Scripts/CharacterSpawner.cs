using System.Collections;
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

    // How long the hotel takes to scroll to this floor when it opens (seconds).
    public float scrollTime = 0.6f;

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

            // Show the player their new floor.
            StartCoroutine(ScrollHotelToHere());
        }

        // Every purchase (including the first) adds a soul.
        SpawnCharacter();
    }

    private IEnumerator ScrollHotelToHere()
    {
        // The hotel's scroll area is somewhere above this floor in the Hierarchy.
        ScrollRect scroll = GetComponentInParent<ScrollRect>();
        if (scroll == null) yield break;

        RectTransform content = scroll.content;
        RectTransform viewport = scroll.viewport != null ? scroll.viewport : (RectTransform)scroll.transform;
        RectTransform me = (RectTransform)transform;

        float scrollable = content.rect.height - viewport.rect.height;
        if (scrollable <= 0f) yield break;   // everything already fits on screen

        // How far down from the top of the hotel this floor's middle is.
        Vector3 myCenter = me.TransformPoint(me.rect.center);
        float fromTop = content.rect.yMax - content.InverseTransformPoint(myCenter).y;

        // Scroll so this floor sits in the middle of the view.
        float offset = Mathf.Clamp(fromTop - viewport.rect.height / 2f, 0f, scrollable);
        float target = 1f - offset / scrollable;   // 1 = top of the hotel, 0 = bottom

        scroll.velocity = Vector2.zero;
        float start = scroll.verticalNormalizedPosition;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / scrollTime;
            scroll.verticalNormalizedPosition = Mathf.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t));
            yield return null;   // wait one frame, then continue
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