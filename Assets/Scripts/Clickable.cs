using UnityEngine;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour
{
    // This variable is marked as `static`, which means it is
    // accessible by any script, anywhere!
    public static int Clicks = 0;        // every click ever (for Stats)
    public static int Pain = 0;          // pain you have right now (gets spent in the shop)
    public static int TotalPain = 0;     // all pain ever made (for Stats)
    public static int PainPerClick = 1;  // how much pain one click gives (upgrades will raise this)

    [Header("Button pictures")]
    public Sprite normalSprite;    // regular button
    public Sprite hoverSprite;     // regular + yellow ring
    public Sprite pressedSprite;   // pushed down + yellow ring

    [Header("Feel")]
    // larger when hovered (1.03 = 3% larger).
    public float hoverScale = 1.03f;
    // smaller when pressed (0.97 = 3% smaller).
    public float pressScale = 0.97f;
    // How fast the button grows/shrinks.
    public float scaleSpeed = 10f;
    // The pushed-down picture shows for at least this long, even on a super-quick click.
    public float minPressTime = 0.08f;

    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;
    private bool isHovered = false;
    private bool isHeld = false;
    private float pressTimer = 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        // Count down the "keep showing pressed" timer.
        if (pressTimer > 0f) pressTimer -= Time.deltaTime;

        bool showPressed = isHeld || pressTimer > 0f;

        // Pick the picture.
        Sprite wanted = normalSprite;
        if (showPressed) wanted = pressedSprite;
        else if (isHovered) wanted = hoverSprite;
        if (wanted != null) spriteRenderer.sprite = wanted;

        // Pick the size.
        Vector3 targetScale = originalScale;
        if (showPressed) targetScale = originalScale * pressScale;
        else if (isHovered) targetScale = originalScale * hoverScale;

        transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    // True when the mouse is on top of a UI thing (shop, hotel, buttons...).
    private bool MouseIsOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    // Called when the mouse button clicks on this object.
    private void OnMouseDown()
    {
        if (MouseIsOverUI()) return;   // the click was meant for the UI, not us

        Clicks += 1;
        Pain += PainPerClick;
        TotalPain += PainPerClick;

        isHeld = true;
        pressTimer = minPressTime;
        transform.localScale = originalScale * pressScale;   // snap down instantly for a punchy feel
    }

    // Mouse button released.
    private void OnMouseUp()
    {
        isHeld = false;
    }

    // Runs every frame while the mouse is over this object.
    private void OnMouseOver()
    {
        isHovered = !MouseIsOverUI();
    }

    // Mouse left the button.
    private void OnMouseExit()
    {
        isHovered = false;
    }
}