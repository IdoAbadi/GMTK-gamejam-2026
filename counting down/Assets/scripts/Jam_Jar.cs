using UnityEngine;

public class Jam_Jar : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("kill"))
        {
            // Destroy the prize object
            Destroy(this.gameObject);
        }
    }
}
