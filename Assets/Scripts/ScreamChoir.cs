using System.Collections.Generic;
using UnityEngine;

public class ScreamChoir : MonoBehaviour
{
    // The screams, in order: first = tamest, last = most unhinged.
    public AudioClip[] screams;

    // Clicks per second needed for ALL the unlocked screams to play at once.
    public float clicksForFullChoir = 10f;

    // How far back we look when measuring click speed (seconds).
    public float measureWindow = 1f;

    // How loud each scream is when it's playing (0 to 1).
    public float screamVolume = 0.8f;

    // How loud each scream gets when the full choir is playing. Max is 1.
    public float fullChoirVolume = 1f;

    // How quickly screams fade in and out (higher = snappier).
    public float fadeSpeed = 3f;

    [Header("Grows with Hell")]
    // Screams available before any rooms are bought.
    // Each room adds its own "Screams Added" (set on each upgrade button).
    public int startingScreams = 1;

    [Header("Blending")]
    // How much each repeat's pitch can vary (0.08 = up to 8% higher or lower).
    public float pitchVariation = 0.08f;
    
    // How different each voice's personal pitch is (0.1 = some voices up to 10% higher or lower, permanently).
    public float voiceSpread = 0.1f;

    // The next scream starts this many seconds before the current one ends.
    public float overlap = 0.3f;

    // One "voice" = two speakers that take turns, so screams can overlap.
    private class Voice
    {
        public AudioSource a;
        public AudioSource b;
        public AudioSource current;
        public float volume;
        public float basePitch = 1f;
    }

    private Voice[] voices;
    private List<float> clickTimes = new List<float>();
    private int lastClickCount;
    private UpgradeButton[] rooms;

    private void Start()
    {
        voices = new Voice[screams.Length];
        for (int i = 0; i < screams.Length; i++)
        {
            voices[i] = new Voice
            {
                a = MakeSpeaker(),
                b = MakeSpeaker(),
                basePitch = 1f + Random.Range(-voiceSpread, voiceSpread)
            };
        }

        // Find every room upgrade, even while the shop is closed.
        rooms = FindObjectsByType<UpgradeButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        lastClickCount = Clickable.Clicks;
    }

    private AudioSource MakeSpeaker()
    {
        AudioSource speaker = gameObject.AddComponent<AudioSource>();
        speaker.loop = false;       // we restart screams ourselves, with variety
        speaker.playOnAwake = false;
        speaker.volume = 0f;
        return speaker;
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
        float clicksPerSecond = clickTimes.Count / measureWindow;

        // 3. How many screams has the player unlocked? Starting amount + each bought room's bonus.
        int extraScreams = 0;
        foreach (UpgradeButton room in rooms)
        {
            if (room.HasBeenBought) extraScreams += room.screamsAdded;
        }
        int unlocked = Mathf.Clamp(startingScreams + extraScreams, 1, voices.Length);

        // 4. Click speed decides how many of the unlocked screams play.
        int howMany = Mathf.CeilToInt(clicksPerSecond / clicksForFullChoir * unlocked);
        howMany = Mathf.Clamp(howMany, 0, unlocked);

        // Full choir = every unlocked scream playing at once.
        float onVolume = (howMany == unlocked) ? fullChoirVolume : screamVolume;

        // 5. Update each voice.
        for (int i = 0; i < voices.Length; i++)
        {
            Voice v = voices[i];
            bool on = i < howMany;

            // Fade in or out.
            float target = on ? onVolume : 0f;
            v.volume = Mathf.MoveTowards(v.volume, target, fadeSpeed * Time.deltaTime);
            v.a.volume = v.volume;
            v.b.volume = v.volume;

            if (on)
            {
                if (v.current == null || !v.current.isPlaying)
                {
                    // Joining in: start with this voice's own scream.
                    StartScream(v, screams[i]);
                }
                else if (TimeLeft(v.current) < overlap)
                {
                    // About to end: start a different scream on the other speaker.
                    StartScream(v, screams[Random.Range(0, howMany)]);
                }
            }
            else if (v.volume <= 0f)
            {
                // Fully faded out: stop everything so it starts fresh next time.
                v.a.Stop();
                v.b.Stop();
                v.current = null;
            }
        }
    }

    private void StartScream(Voice v, AudioClip clip)
    {
        // Use whichever speaker isn't the current one, so the old scream can finish its tail.
        AudioSource next = (v.current == v.a) ? v.b : v.a;
        next.clip = clip;
        next.pitch = v.basePitch + Random.Range(-pitchVariation, pitchVariation);
        next.Play();
        v.current = next;
    }

    private float TimeLeft(AudioSource speaker)
    {
        // Seconds until this scream ends (higher pitch plays faster).
        return (speaker.clip.length - speaker.time) / speaker.pitch;
    }
}