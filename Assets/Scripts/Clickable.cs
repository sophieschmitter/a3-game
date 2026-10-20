using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{

    // This variable is marked as `static`, which means it is
    // accessible by any script, anywhere! They can just write
    // Clickable.Clicks to read/write to this value.
    //
    // See an example in `ClickTracker.cs`, which is on a UI
    // object titled `Tracker Text (TMP)`.
    public static int Clicks = 0;        // every click ever (for Stats)
    public static int Pain = 0;          // pain you have right now (gets spent in the shop and thus will go down, which is why it must be separated from clicks)
    public static int TotalPain = 0;     // all pain ever made (for Stats)
    public static int PainPerClick = 1;  // how much pain one click gives (upgrades will raise this)

    // larger when hovered (1.1 = 10% larger).
    public float hoverScale = 1.1f;
    // smaller when pressed (0.9 = 10% smaller).
    public float pressScale = 0.9f;
    // How fast the button grows/shrinks, in scale units per second.
    public float scaleSpeed = 0.5f;

    private Vector3 originalScale;
    private bool isHovered = false;
    private bool isPressed = false;

    private void Awake()
    {
        // Remember the size the object started with.
        originalScale = transform.localScale;
    }

    private void Update()
    {
        // Pressed takes priority over hovered, which takes priority over normal.
        Vector3 targetScale = originalScale;
        if (isPressed)
        {
            targetScale = originalScale * pressScale;
        }
        else if (isHovered)
        {
            targetScale = originalScale * hoverScale;
        }

        transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    /// <summary>
    /// This function is called when the mouse button clicks
    /// on this object.
    /// </summary>
    private void OnMouseDown()
    {
        Clicks += 1;  // add one point
        Pain += PainPerClick;
        TotalPain += PainPerClick;
        isPressed = true;
    }

    // release press
    private void OnMouseUp()
    {
        isPressed = false;
    }

    // hover
    private void OnMouseEnter()
    {
        isHovered = true;
    }

    // unhover
    private void OnMouseExit()
    {
        isHovered = false;
    }

}