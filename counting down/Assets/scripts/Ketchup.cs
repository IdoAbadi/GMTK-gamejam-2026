using UnityEngine;

public class Ketchup : MonoBehaviour
{
    [SerializeField] private Canvas WinCanvas;

    private void Awake()
    {
        var go = GameObject.FindWithTag("win screen");
        WinCanvas = go.GetComponent<Canvas>();
        WinCanvas.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("kill"))
        {
            if (WinCanvas != null)
            {
                WinCanvas.gameObject.SetActive(true);
            }
            // add minigame won flag to main game manager

            // Destroy the prize object
            Destroy(this.gameObject);
        }
    }
}