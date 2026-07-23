using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Timer_text_updater : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject microwave;
    public TextMeshProUGUI timerText;
    private int timeRemaining = -1;
    private Microwave_timer microwaveTimer;
    // Update is called once per frame

    void Start()
    {
        if (microwave == null)
        {
            Debug.LogError("Microwave GameObject is not assigned in the inspector.");
            return;
        }
        else
        {
            microwaveTimer = microwave.GetComponent<Microwave_timer>();
            // Check if the microwave has a Microwave_timer component
            if (microwaveTimer == null)
            {
                Debug.LogError("Microwave GameObject does not have a Microwave_timer component.");
                return;
            }
        }
    }
    private void FixedUpdate()
    {
        // Read the microwave timer's float timeRemaining and store as int
        timeRemaining = (int)microwaveTimer.timeRemaining;
    }

    void Update()
    {
        // Update the timerText with the timeRemaining
        if (timeRemaining >= 0)
        {
            timerText.text = timeRemaining.ToString();
        }
        else
        {
            timerText.text = "0";
        }
    }
}
