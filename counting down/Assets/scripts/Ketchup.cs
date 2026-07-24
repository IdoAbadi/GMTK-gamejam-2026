using UnityEngine;

public class Ketchup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("kill"))
        {
            //add enable to win canvas 

            // add minigame won flag to main game manager


            // Destroy the prize object
            Destroy(this.gameObject);
        }
    }
}
