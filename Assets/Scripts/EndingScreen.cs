using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class EndingScreen : MonoBehaviour, IPointerClickHandler
{
    // The final upgrade (the 11th one). The ending plays the first time it's bought.
    public UpgradeButton finalUpgrade;

    // The text object the message appears in.
    public TMP_Text endingText;

    // The message. {TIME} gets replaced with how long they've played.
    [TextArea(8, 12)]
    public string message =
        "This.\n" +
        "This is what you have been doing.\n" +
        "For the last {TIME}.\n" +
        "\n" +
        "We say hell is other people.\n" +
        "Thank you for proving us wrong.";

    // Shown at the bottom once the message has finished.
    public string continueHint = "<size=50%>(click to continue)</size>";

    [Header("Timing (seconds)")]
    public float waitBeforeEnding = 1.5f;   // lets the hotel scroll to the final floor first
    public float fadeTime = 2f;             // how long the screen takes to fade in/out
    public float lineDelay = 1.5f;          // pause between lines

    [Header("Debug")]
    public bool logDebug = true;            // untick once the ending works

    private CanvasGroup group;
    private bool started = false;
    private bool canClose = false;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0f;              // invisible...
        group.blocksRaycasts = false;  // ...and clicks pass straight through
        endingText.text = "";
    }

    private void Start()
    {
        if (!logDebug) return;

        if (finalUpgrade == null)
        {
            Debug.LogWarning("EndingScreen: Final Upgrade slot is empty.");
        }
        else
        {
            Debug.Log("EndingScreen: watching '" + finalUpgrade.name +
                      "'. If that is not your 11th upgrade, drag the right button into Final Upgrade.");
        }
    }

    private void Update()
    {
        if (started || finalUpgrade == null) return;

        if (finalUpgrade.HasBeenBought)
        {
            if (logDebug)
            {
                Debug.Log("EndingScreen: starting because '" + finalUpgrade.name + "' was bought.");
            }
            started = true;
            StartCoroutine(PlayEnding());
        }
    }

    private IEnumerator PlayEnding()
    {
        // Remember the play time right now, before the ending eats into it.
        string timePlayed = FormatTime(Time.timeSinceLevelLoad);

        yield return new WaitForSeconds(waitBeforeEnding);

        // Fade the screen in.
        group.blocksRaycasts = true;
        yield return Fade(0f, 1f);

        // Show the message one line at a time.
        string[] lines = message.Replace("{TIME}", timePlayed).Split('\n');
        string shown = "";
        foreach (string line in lines)
        {
            shown += line + "\n";
            endingText.text = shown;
            if (line.Trim() != "")
            {
                yield return new WaitForSeconds(lineDelay);
            }
        }

        endingText.text = shown + "\n" + continueHint;
        canClose = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canClose) return;
        canClose = false;
        StartCoroutine(Close());
    }

    private IEnumerator Close()
    {
        yield return Fade(1f, 0f);
        group.blocksRaycasts = false;   // back to the game
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeTime;
            group.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        group.alpha = to;
    }

    // Turns seconds into "1 hour, 4 minutes and 12 seconds".
    private string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        int hours = total / 3600;
        int minutes = (total % 3600) / 60;
        int secs = total % 60;

        List<string> parts = new List<string>();
        if (hours > 0) parts.Add(hours + (hours == 1 ? " hour" : " hours"));
        if (minutes > 0) parts.Add(minutes + (minutes == 1 ? " minute" : " minutes"));
        if (secs > 0 || parts.Count == 0) parts.Add(secs + (secs == 1 ? " second" : " seconds"));

        if (parts.Count == 1) return parts[0];
        return string.Join(", ", parts.GetRange(0, parts.Count - 1)) + " and " + parts[parts.Count - 1];
    }
}