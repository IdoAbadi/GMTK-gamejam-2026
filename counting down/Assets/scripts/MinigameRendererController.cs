using UnityEngine;

public class MinigameRendererController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag your Microwave_timer object here")]
    public Microwave_timer microwaveTimer;

    [Tooltip("Drag the parent GameObject containing your sprites, UI, or Canvas here")]
    public GameObject targetVisuals;

    [Header("Win Conditions")]
    [Tooltip("Amount of TotalWins needed for these visuals to turn ON when the timer starts.")]
    public int requiredWinsToAppear = 0;

    [Tooltip("Amount of TotalWins that will cause these visuals to turn OFF.")]
    public int requiredWinsToDisappear = 1;

    void Awake()
    {
        // Ensure the visuals start turned off
        if (targetVisuals != null)
        {
            targetVisuals.SetActive(false);
        }
        else
        {
            Debug.LogWarning("MinigameRendererController: No Target Visuals assigned!");
        }
    }

    void Update()
    {
        if (microwaveTimer == null || targetVisuals == null || SceneSwitchManager.Instance == null) return;

        // Get the current amount of wins from the manager
        int currentWins = SceneSwitchManager.Instance.TotalWins;

        // The visuals should be visible IF:
        // 1. The microwave timer is currently running
        // 2. We have enough wins to make it appear
        // 3. We haven't reached the amount of wins that makes it disappear
        bool shouldBeVisible = microwaveTimer.IsRunning() &&
                               currentWins >= requiredWinsToAppear &&
                               currentWins < requiredWinsToDisappear;

        // Apply the visual state (Checking activeSelf prevents Unity from overworking)
        if (shouldBeVisible && !targetVisuals.activeSelf)
        {
            targetVisuals.SetActive(true);
        }
        else if (!shouldBeVisible && targetVisuals.activeSelf)
        {
            targetVisuals.SetActive(false);
        }
    }
}