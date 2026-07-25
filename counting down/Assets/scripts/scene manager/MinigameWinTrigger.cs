using UnityEngine;

public class MinigameWinTrigger : MonoBehaviour
{
    private void OnEnable()
    {
        // When this Canvas is enabled, tell the Manager in the main scene
        if (SceneSwitchManager.Instance != null)
        {
            SceneSwitchManager.Instance.HandleMinigameWin();
        }
        else
        {
            Debug.LogWarning("SceneSwitchManager not found. Are you running the minigame directly?");
        }
    }
}