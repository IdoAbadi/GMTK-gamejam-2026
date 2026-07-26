using UnityEngine;
using UnityEngine.InputSystem;

public class scene_switch_interact : MonoBehaviour
{
    [Tooltip("Radius around this object where the player can toggle the timer using the E key")]
    public float activationRadius = 2f;
    // Name of the scene to load. Set this in the Inspector.
    public string sceneName;
    public GameObject player;
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
            // Use the SceneSwitchManager to load the minigame additively
            if (SceneSwitchManager.Instance != null)
            {
                SceneSwitchManager.Instance.StartMinigame(sceneName);
            }
            else
            {
                Debug.LogError("SceneSwitchManager is missing from the main scene!");
            }
        }
        else if (playerInRange && string.IsNullOrEmpty(sceneName) && kb.eKey.wasPressedThisFrame)
        {
            Debug.LogWarning("scene_switch_interact: sceneName is empty. Set the scene name in the Inspector.");
        }
    }
}