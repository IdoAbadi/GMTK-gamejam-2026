using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class AutoDisableListener : MonoBehaviour
{
    private void Awake()
    {
        // Find all active AudioListeners across all loaded scenes
        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);

        // If there is more than one, disable this specific one
        if (listeners.Length > 1)
        {
            GetComponent<AudioListener>().enabled = false;
        }
    }
}