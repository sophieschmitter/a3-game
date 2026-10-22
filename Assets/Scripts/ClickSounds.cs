using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickSounds : MonoBehaviour
{
    // Played when clicking any button or clickable UI (shop, tiles, mail, continue...).
    public AudioClip buttonSound;

    // Played when clicking the red pain button. Leave empty for no extra sound (screams only).
    public AudioClip painButtonSound;

    // How loud each sound is (0 to 1).
    public float buttonVolume = 0.7f;
    public float painButtonVolume = 0.7f;

    private AudioSource speaker;
    private List<RaycastResult> hits = new List<RaycastResult>();

    private void Awake()
    {
        speaker = gameObject.AddComponent<AudioSource>();
        speaker.playOnAwake = false;
    }

    private void Update()
    {
        // Only on the frame the left mouse button goes down.
        if (!Input.GetMouseButtonDown(0)) return;

        if (MouseIsOnClickableUI())
        {
            Play(buttonSound, buttonVolume);
        }
        else if (MouseIsOnPainButton())
        {
            Play(painButtonSound, painButtonVolume);
        }
    }

    private void Play(AudioClip clip, float volume)
    {
        // PlayOneShot lets sounds overlap, so fast clicking never cuts one off.
        if (clip != null) speaker.PlayOneShot(clip, volume);
    }

    // Is the mouse on a UI thing that reacts to clicks (any Button, upgrade tile, etc.)?
    private bool MouseIsOnClickableUI()
    {
        if (EventSystem.current == null) return false;

        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;

        hits.Clear();
        EventSystem.current.RaycastAll(pointer, hits);
        if (hits.Count == 0) return false;

        // hits[0] is whatever is on top. Does it (or something it's inside) handle clicks?
        GameObject top = hits[0].gameObject;
        return ExecuteEvents.GetEventHandler<IPointerClickHandler>(top) != null
            || ExecuteEvents.GetEventHandler<IPointerDownHandler>(top) != null;
    }

    // Is the mouse on the red pain button (a world object, not UI)?
    private bool MouseIsOnPainButton()
    {
        // If UI is on top, the pain button can't be clicked anyway.
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return false;

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPoint);
        return hit != null && hit.GetComponent<Clickable>() != null;
    }
}