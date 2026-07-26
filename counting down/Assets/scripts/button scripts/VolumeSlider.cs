using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    [Header("Mixer Settings")]
    [Tooltip("Drag your Audio Mixer here")]
    public AudioMixer audioMixer;

    [Tooltip("The exact name of the exposed parameter in the Audio Mixer (e.g., 'MasterVolume')")]
    public string mixerParameter = "MasterVolume";

    private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();
    }

    private void Start()
    {
        // Load saved volume (default to 1, which is max volume)
        float savedVolume = PlayerPrefs.GetFloat(mixerParameter, 1f);

        // Ensure the slider UI matches the loaded value
        volumeSlider.value = savedVolume;

        // Apply the volume to the mixer immediately
        SetVolume(savedVolume);

        // Listen for slider changes
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    private void SetVolume(float sliderValue)
    {
        // Clamp to avoid Log10(0) which returns negative infinity
        float clampedValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);

        // Convert linear slider value to logarithmic decibels (-80dB to 0dB)
        float volumeInDecibels = Mathf.Log10(clampedValue) * 20f;

        // Apply to the Audio Mixer
        audioMixer.SetFloat(mixerParameter, volumeInDecibels);

        // Save for the next time the game is played
        PlayerPrefs.SetFloat(mixerParameter, sliderValue);
    }

    private void OnDestroy()
    {
        // Prevent memory leaks when the UI is destroyed
        volumeSlider.onValueChanged.RemoveListener(SetVolume);
    }
}