using UnityEngine;

public class PassiveIncome : MonoBehaviour
{
    // How much pain this character produces each time.
    public int amount = 10;

    // Seconds between payouts.
    public float interval = 10f;

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            // Subtract instead of resetting to 0, so no time is lost.
            timer -= interval;

            Clickable.Pain += amount;
            Clickable.TotalPain += amount;
        }
    }
}