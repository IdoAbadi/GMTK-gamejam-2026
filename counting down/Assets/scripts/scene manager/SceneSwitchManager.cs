using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchManager : MonoBehaviour
{
    public static SceneSwitchManager Instance { get; private set; }

    [Header("Main Scene Audio")]
    [Tooltip("Drag the main scene's background AudioSources here.")]
    [SerializeField] private List<AudioSource> mainSceneAudioSources;

    // We store the original volumes so we can accurately restore them later
    private Dictionary<AudioSource, float> originalVolumes = new Dictionary<AudioSource, float>();
    private string activeMinigameScene;

    private void Awake()
    {
        // Simple Singleton pattern
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    // Call this method (e.g., from a button) to load your minigame
    public void StartMinigame(string minigameSceneName)
    {
        activeMinigameScene = minigameSceneName;
        StartCoroutine(LoadMinigameRoutine(minigameSceneName));
    }

    private IEnumerator LoadMinigameRoutine(string sceneName)
    {
        originalVolumes.Clear();

        // Lower the volume of all tracked main scene audio sources to 30%
        foreach (var audioSource in mainSceneAudioSources)
        {
            if (audioSource != null)
            {
                originalVolumes[audioSource] = audioSource.volume;
                audioSource.volume *= 0.3f;
            }
        }

        // Load Additively: The main scene stays active underneath
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    // This will be called by the Win Screen when it enables
    public void HandleMinigameWin()
    {
        StartCoroutine(CloseMinigameRoutine());
    }

    private IEnumerator CloseMinigameRoutine()
    {
        // Wait for 2 seconds while the win screen is visible
        yield return new WaitForSeconds(2f);

        // Unload the minigame scene
        if (!string.IsNullOrEmpty(activeMinigameScene))
        {
            yield return SceneManager.UnloadSceneAsync(activeMinigameScene);
            activeMinigameScene = null;
        }

        // Restore main scene audio to exactly what it was
        foreach (var kvp in originalVolumes)
        {
            if (kvp.Key != null)
            {
                kvp.Key.volume = kvp.Value;
            }
        }
    }
}