using UnityEngine;

public class MinigameWinTrigger : MonoBehaviour
{
    private bool isFirstEnable = true;

    private void OnEnable()
    {
        // Ignore the first setup frame when the scene loads
        if (isFirstEnable)
        {
            isFirstEnable = false;
            return;
        }

        if (SceneSwitchManager.Instance != null)
        {
            // Pass 'true' to let the manager know the win condition was met
            SceneSwitchManager.Instance.HandleMinigameWin(true);
        }
        else
        {
            Debug.LogWarning("SceneSwitchManager not found. Are you running the minigame directly?");
        }
    }
}