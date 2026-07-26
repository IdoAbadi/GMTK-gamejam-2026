using UnityEngine;
using UnityEngine.InputSystem;

public class scene_switch_interact : MonoBehaviour
{
    [Tooltip("Radius around this object where the player can toggle the timer using the E key")]
    public float activationRadius = 2f;
    // Name of the scene to load. Set this in the Inspector.
    public string sceneName;
    public GameObject player;

    // NEW: Reference to the timer to check if it's running
    [Tooltip("Assign the active Microwave_timer here.")]
    public Microwave_timer microwaveTimer;

    bool playerInRange = false;
    Keyboard kb;

    private void Start()
    {
        kb = Keyboard.current;
    }

    void Update()
    {
        playerInRange = false;
        if (player != null)
        {
            float d = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.transform.position.x, player.transform.position.y));
            playerInRange = d <= activationRadius;
        }

        if (playerInRange && !string.IsNullOrEmpty(sceneName) && kb.eKey.wasPressedThisFrame)
        {
            // NEW: Only allow minigame entry if the timer is actively running
            if (microwaveTimer != null && microwaveTimer.IsRunning())
            {
                if (SceneSwitchManager.Instance != null)
                {
                    SceneSwitchManager.Instance.StartMinigame(sceneName);
                }
                else
                {
                    Debug.LogError("SceneSwitchManager is missing from the main scene!");
                }
            }
            else
            {
                Debug.Log("Cannot enter minigame: The timer is not running.");
            }
        }
        else if (playerInRange && string.IsNullOrEmpty(sceneName) && kb.eKey.wasPressedThisFrame)
        {
            Debug.LogWarning("scene_switch_interact: sceneName is empty. Set the scene name in the Inspector.");
        }
    }
}