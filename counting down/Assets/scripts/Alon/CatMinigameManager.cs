using UnityEngine;
using TMPro;

public class CatMinigameManager : MonoBehaviour
{
    [SerializeField] private int foodNeededToWin = 15;
    [SerializeField] private int maximumMisses = 3;

    [Header("UI")]
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text missesText;
    [SerializeField] private TMP_Text resultText;

    private int foodCaught;
    private int misses;
    private bool gameEnded;

    private void Start()
    {
        UpdateUI();

        if (resultText != null)
        {
            resultText.text = "";
        }
    }

    public void CatchFood()
    {
        if (gameEnded)
            return;

        foodCaught++;
        UpdateUI();

        Debug.Log("Food: " + foodCaught + " / " + foodNeededToWin);

        if (foodCaught >= foodNeededToWin)
        {
            EndGame(true);
        }
    }

    public void MissFood()
    {
        if (gameEnded)
            return;

        misses++;
        UpdateUI();

        Debug.Log("Misses: " + misses + " / " + maximumMisses);

        if (misses >= maximumMisses)
        {
            EndGame(false);
        }
    }

    private void UpdateUI()
    {
        if (foodText != null)
        {
            foodText.text =
                "Food: " + foodCaught + " / " + foodNeededToWin;
        }

        if (missesText != null)
        {
            missesText.text =
                "Misses: " + misses + " / " + maximumMisses;
        }
    }

    private void EndGame(bool playerWon)
    {
        gameEnded = true;

        FoodSpawner foodSpawner =
            FindFirstObjectByType<FoodSpawner>();

        if (foodSpawner != null)
        {
            foodSpawner.StopSpawning();
        }

        FallingItem[] remainingFood =
            FindObjectsByType<FallingItem>(
                FindObjectsSortMode.None
            );

        foreach (FallingItem food in remainingFood)
        {
            food.DropAfterWin();
        }

        if (playerWon)
        {
            resultText.text = "YOU WIN!";
            resultText.color = Color.green;
            Debug.Log("CAT MINIGAME WON!");
        }
        else
        {
            resultText.text = "YOU LOSE!";
            resultText.color = Color.red;
            Debug.Log("CAT MINIGAME LOST!");
        }
    }
}