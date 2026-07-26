using UnityEngine;

public class controls : MonoBehaviour
{
    [Tooltip("Canvas (or GameObject) to disable when switching")]
    public GameObject canvasToDisable;

    [Tooltip("Canvas (or GameObject) to enable when switching")]
    public GameObject canvasToEnable;

    [Tooltip("If true, change Time.timeScale when switching canvases")]
    public bool changeTimeScale = false;

    [Tooltip("When changing time scale: set to true to use 1 (running), false to use 0 (paused)")]
    public bool timeScaleSetToOne = true;

    [Tooltip("If true, also pause/resume all audio when time scale changes")]
    public bool controlAudio = false;

    // Call from a UI Button OnClick to switch canvases
    public void SwitchCanvases()
    {
        if (canvasToDisable == null && canvasToEnable == null)
        {
            Debug.LogWarning("controls: both canvasToDisable and canvasToEnable are null.");
            return;
        }

        if (canvasToEnable != null)
        {
            canvasToEnable.SetActive(true);
        }

        if (canvasToDisable != null)
        {
            canvasToDisable.SetActive(false);
        }

        // Optionally change global time scale (0 = paused, 1 = normal)
        if (changeTimeScale)
        {
            Time.timeScale = timeScaleSetToOne ? 1f : 0f;

            // Pause or resume audio based on the timeScaleSetToOne value
            if (controlAudio)
            {
                // If timeScaleSetToOne is false (game is pausing), AudioListener.pause becomes true
                AudioListener.pause = !timeScaleSetToOne;
            }
        }
    }
}