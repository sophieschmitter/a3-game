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

    /// <summary>
    /// This function is called when the mouse button clicks
    /// on this object.
    /// </summary>
    private void OnMouseDown()
    {
        Clicks += 1;  // add one point
        Pain += PainPerClick;
        TotalPain += PainPerClick;
    }

}
