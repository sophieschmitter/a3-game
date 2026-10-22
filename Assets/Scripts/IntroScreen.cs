using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroScreen : MonoBehaviour
{
    // The giant mail button shown first.
    public Button mailButton;

    // The letter (a panel holding the text and the continue button).
    public GameObject letter;

    // The "(click to continue)" button at the bottom of the letter.
    public Button continueButton;

    // How long the intro takes to fade away (seconds).
    public float fadeTime = 1f;

    // How much the mail button gently pulses to invite a click (0 = no pulse).
    public float pulseAmount = 0.03f;

    private CanvasGroup group;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 1f;
        group.blocksRaycasts = true;   // nothing behind can be clicked yet

        mailButton.gameObject.SetActive(true);
        letter.SetActive(false);

        // Tell the buttons what to do when clicked (no On Click setup needed).
        mailButton.onClick.AddListener(OpenLetter);
        continueButton.onClick.AddListener(StartGame);
    }

    private void Update()
    {
        // Gentle "breathing" pulse on the mail button.
        if (mailButton.gameObject.activeSelf)
        {
            float scale = 1f + pulseAmount * Mathf.Sin(Time.time * 3f);
            mailButton.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    private void OpenLetter()
    {
        mailButton.gameObject.SetActive(false);
        letter.SetActive(true);
    }

    private void StartGame()
    {
        continueButton.interactable = false;   // no double-clicks
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeTime;
            group.alpha = 1f - t;
            yield return null;
        }
        group.blocksRaycasts = false;
        gameObject.SetActive(false);   // intro's done for good
    }
}