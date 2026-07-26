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
    public string activeMinigameScene { get; private set; }

    private Vector2 originalTimerPosition;
    private Vector2 originalTimerAnchorMin;
    private Vector2 originalTimerAnchorMax;
    private Vector2 originalTimerPivot;

    public bool LastMinigameCompleted { get; private set; }

    // NEW: Tracks the total amount of minigame wins
    public int TotalWins { get; private set; } = 0;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void StartMinigame(string minigameSceneName)
    {
        if (!string.IsNullOrEmpty(activeMinigameScene)) return;

        activeMinigameScene = minigameSceneName;
        StartCoroutine(LoadMinigameRoutine(minigameSceneName));
    }

    private IEnumerator LoadMinigameRoutine(string sceneName)
    {
        originalVolumes.Clear();

        foreach (var audioSource in mainSceneAudioSources)
        {
            if (audioSource != null)
            {
                originalVolumes[audioSource] = audioSource.volume;
                audioSource.volume *= 0.3f;
            }
        }

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

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }

    public void HandleMinigameWin(bool isCompleted)
    {
        LastMinigameCompleted = isCompleted;

        // NEW: Increment total wins if the minigame was successfully completed
        if (isCompleted)
        {
            TotalWins++;
            Debug.Log($"Minigame Won! Total Wins: {TotalWins}");
        }

        StartCoroutine(CloseMinigameRoutine());
    }

    private IEnumerator CloseMinigameRoutine()
    {
        yield return new WaitForSeconds(2f);

        if (!string.IsNullOrEmpty(activeMinigameScene))
        {
            SceneManager.SetActiveScene(gameObject.scene);
            yield return SceneManager.UnloadSceneAsync(activeMinigameScene);
            activeMinigameScene = null;
        }

        foreach (var kvp in originalVolumes)
        {
            if (kvp.Key != null)
            {
                kvp.Key.volume = kvp.Value;
            }
        }

        if (timerRectTransform != null)
        {
            timerRectTransform.anchorMin = originalTimerAnchorMin;
            timerRectTransform.anchorMax = originalTimerAnchorMax;
            timerRectTransform.pivot = originalTimerPivot;
            timerRectTransform.anchoredPosition = originalTimerPosition;
        }
    }
}