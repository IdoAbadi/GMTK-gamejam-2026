using UnityEngine;

public class feeding_winscreen : MonoBehaviour
{
    private void OnEnable()
    {
        if (SceneSwitchManager.Instance != null)
        {
            Debug.Log("feeding_winscreen: Minigame win detected.");
            // Pass 'true' to let the manager know the win condition was met
            SceneSwitchManager.Instance.HandleMinigameWin(true);
        }
        else
        {
            Debug.LogWarning("SceneSwitchManager not found. Are you running the minigame directly?");
        }
    }
}