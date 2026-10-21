using UnityEngine;
using UnityEngine.UI;

public class SoulBreathing : MonoBehaviour
{
    public Sprite[] frames;              // the breathing pictures, in order
    public float secondsPerFrame = 0.5f; // how long each picture shows

    private Image image;
    private float timer;
    private int currentFrame = 0;

    private void Awake()
    {
        image = GetComponent<Image>();
        // Start at a random point so souls don't all breathe in sync.
        timer = Random.Range(0f, secondsPerFrame);
    }

    private void Update()
    {
        if (frames.Length == 0) return;

        timer += Time.deltaTime;   // add the time since last frame
        if (timer >= secondsPerFrame)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            image.sprite = frames[currentFrame];
        }
    }
}