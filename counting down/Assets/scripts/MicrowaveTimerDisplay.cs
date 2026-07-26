using UnityEngine;
using TMPro; // Required for TextMeshPro UI elements

public class MicrowaveTimerDisplay : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag your Canvas TextMeshPro element here")]
    public TextMeshProUGUI timerText;

    // Reference to the target timer
    private Microwave_timer microwaveTimer;

    void Start()
    {
        // Locates the Microwave_timer in the scene on start
        // Note: FindFirstObjectByType is preferred in newer Unity versions over FindObjectOfType
        microwaveTimer = FindFirstObjectByType<Microwave_timer>();

        if (microwaveTimer == null)
        {
            Debug.LogError("No Microwave_timer found in the scene! Make sure one exists.");
        }
    }

    void FixedUpdate()
    {
        // Make sure we successfully found the timer and assigned the text
        if (microwaveTimer != null && timerText != null)
        {
            // Sample the remaining time using the API provided in the timer script
            float timeRemaining = microwaveTimer.TimeRemaining();

            // Update the Canvas text element, formatting to one decimal place
            timerText.text = timeRemaining.ToString("F1") + "s";
        }
    }
}