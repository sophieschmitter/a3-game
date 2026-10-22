using System.Collections.Generic;
using UnityEngine;

public class ScreamChoir : MonoBehaviour
{
    // The screams, in order: first = tamest, last = most unhinged.
    public AudioClip[] screams;

    // Clicks per second needed for ALL the screams to play at once.
    public float clicksForFullChoir = 10f;

    // How far back we look when measuring click speed (seconds).
    public float measureWindow = 1f;

    // How loud each scream is when it's playing (0 to 1).
    public float screamVolume = 0.6f;

    // How quickly screams fade in and out (higher = snappier).
    public float fadeSpeed = 3f;

    private AudioSource[] voices;
    private List<float> clickTimes = new List<float>();
    private int lastClickCount;

    private void Start()
    {
        // Make one "speaker" (AudioSource) per scream.
        voices = new AudioSource[screams.Length];
        for (int i = 0; i < screams.Length; i++)
        {
            AudioSource voice = gameObject.AddComponent<AudioSource>();
            voice.clip = screams[i];
            voice.loop = true;          // keeps screaming while the player keeps clicking
            voice.playOnAwake = false;
            voice.volume = 0f;
            voices[i] = voice;
        }

        lastClickCount = Clickable.Clicks;
    }

    private void Update()
    {
        // 1. Notice any new clicks since last frame, and write down when they happened.
        int newClicks = Clickable.Clicks - lastClickCount;
        lastClickCount = Clickable.Clicks;
        for (int i = 0; i < newClicks; i++)
        {
            clickTimes.Add(Time.time);
        }

        // 2. Forget clicks that are older than the measuring window.
        clickTimes.RemoveAll(t => Time.time - t > measureWindow);

        // 3. Work out how fast the player is clicking, and how many screams that earns.
        float clicksPerSecond = clickTimes.Count / measureWindow;
        int howMany = Mathf.CeilToInt(clicksPerSecond / clicksForFullChoir * voices.Length);
        howMany = Mathf.Clamp(howMany, 0, voices.Length);

        // 4. Fade each scream in or out.
        for (int i = 0; i < voices.Length; i++)
        {
            AudioSource voice = voices[i];
            float target = (i < howMany) ? screamVolume : 0f;

            // Start a scream from the beginning when it joins in.
            if (target > 0f && !voice.isPlaying)
            {
                voice.Play();
            }

            voice.volume = Mathf.MoveTowards(voice.volume, target, fadeSpeed * Time.deltaTime);

            // Fully faded out? Stop it, so it starts fresh next time.
            if (target == 0f && voice.volume <= 0f)
            {
                voice.Stop();
            }
        }
    }
}