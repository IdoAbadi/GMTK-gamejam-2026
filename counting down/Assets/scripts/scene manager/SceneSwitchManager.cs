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

    [Header("Main Scene UI")]
    [Tooltip("Drag the Timer's RectTransform here.")]
    [SerializeField] private RectTransform timerRectTransform;
    [Tooltip("Padding from the top-right corner during a minigame.")]
    [SerializeField] private Vector2 minigameTimerOffset = new Vector2(-20f, -20f);

    private Dictionary<AudioSource, float> originalVolumes = new Dictionary<AudioSource, float>();
    private string activeMinigameScene;

    // Variables to store the original layout of the timer
    private Vector2 originalTimerPosition;
    private Vector2 originalTimerAnchorMin;
    private Vector2 originalTimerAnchorMax;
    private Vector2 originalTimerPivot;

    // Add this line to store the completion status so other scripts can read it
    public bool LastMinigameCompleted { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

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

        // Move Timer to Top-Right
        if (timerRectTransform != null)
        {
            originalTimerPosition = timerRectTransform.anchoredPosition;
            originalTimerAnchorMin = timerRectTransform.anchorMin;
            originalTimerAnchorMax = timerRectTransform.anchorMax;
            originalTimerPivot = timerRectTransform.pivot;

            timerRectTransform.anchorMin = new Vector2(1, 1);
            timerRectTransform.anchorMax = new Vector2(1, 1);
            timerRectTransform.pivot = new Vector2(1, 1);

            timerRectTransform.anchoredPosition = minigameTimerOffset;
        }

        // Load Additively: The main scene stays active underneath
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public void HandleMinigameWin(bool isCompleted)
    {
        LastMinigameCompleted = isCompleted;
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

        // Restore main scene audio
        foreach (var kvp in originalVolumes)
        {
            if (kvp.Key != null)
            {
                kvp.Key.volume = kvp.Value;
            }
        }

        // Restore Timer to its exact original position
        if (timerRectTransform != null)
        {
            timerRectTransform.anchorMin = originalTimerAnchorMin;
            timerRectTransform.anchorMax = originalTimerAnchorMax;
            timerRectTransform.pivot = originalTimerPivot;
            timerRectTransform.anchoredPosition = originalTimerPosition;
        }
    }
}