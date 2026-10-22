using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomLog : MonoBehaviour
{
    // The lore for this room. Entry 1 unlocks on the 1st purchase, 2 on the 2nd, 3 on the 3rd.
    [TextArea(3, 6)]
    public string[] entries = new string[3];

    // What locked entries look like.
    public string lockedEntry = "<i>???</i>";

    [Header("Icons")]
    public Sprite normalIcon;   // log button, nothing new
    public Sprite newIcon;      // log button with the red dot

    [Header("Parts (found automatically by name if left empty)")]
    public Button logButton;    // child named "LogButton"
    public GameObject logPanel; // child named "LogPanel"
    public TMP_Text logText;    // the text inside LogPanel

    private int unlockedCount = 0;
    private bool hasUnread = false;

    private void Awake()
    {
        // Find the parts by name so each floor doesn't need dragging.
        if (logButton == null)
        {
            Transform t = transform.Find("LogButton");
            if (t != null) logButton = t.GetComponent<Button>();
        }
        if (logPanel == null)
        {
            Transform t = transform.Find("LogPanel");
            if (t != null) logPanel = t.gameObject;
        }
        if (logText == null && logPanel != null)
        {
            logText = logPanel.GetComponentInChildren<TMP_Text>(true);
        }

        // Clicking the log button opens/closes. Clicking the panel itself closes it.
        logButton.onClick.AddListener(Toggle);
        Button panelButton = logPanel.GetComponent<Button>();
        if (panelButton != null) panelButton.onClick.AddListener(Close);

        // Hidden until the room's first purchase.
        logButton.gameObject.SetActive(false);
        logPanel.SetActive(false);
    }

    // Called by this floor's CharacterSpawner on every purchase.
    public void OnPurchased(int purchaseNumber)
    {
        if (purchaseNumber <= entries.Length && purchaseNumber > unlockedCount)
        {
            unlockedCount = purchaseNumber;
            hasUnread = true;
            logButton.gameObject.SetActive(true);
            Refresh();
        }
    }

    private void Toggle()
    {
        if (logPanel.activeSelf) Close();
        else Open();
    }

    private void Open()
    {
        hasUnread = false;
        logPanel.SetActive(true);
        Refresh();
    }

    private void Close()
    {
        logPanel.SetActive(false);
    }

    private void Refresh()
    {
        // Red dot if there's something new to read.
        Sprite icon = hasUnread ? newIcon : normalIcon;
        if (icon != null) logButton.image.sprite = icon;

        // Build the text: unlocked entries, then ??? for the rest.
        string text = "";
        for (int i = 0; i < entries.Length; i++)
        {
            if (i > 0) text += "\n\n";
            text += (i < unlockedCount) ? entries[i] : lockedEntry;
        }
        logText.text = text;

        // Souls get added to the room over time; keep the panel and button drawn above them.
        logPanel.transform.SetAsLastSibling();
        logButton.transform.SetAsLastSibling();
    }
}