using UnityEngine;

public class FallingItem : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 3f;
    [SerializeField] private float destroyY = -6f;

    [Header("Win Drop")]
    [SerializeField] private float winGravityScale = 2.5f;
    [SerializeField] private float hitStrength = 2f;
    [SerializeField] private float spinStrength = 100f;

    private Rigidbody2D itemRigidbody;
    private CatMinigameManager gameManager;

    private bool itemResolved;
    private bool freeFalling;

    private void Awake()
    {
        itemRigidbody = GetComponent<Rigidbody2D>();
        gameManager = FindFirstObjectByType<CatMinigameManager>();
    }

    private void FixedUpdate()
    {
        if (!freeFalling)
        {
            Vector2 newPosition =
                itemRigidbody.position +
                Vector2.down * fallSpeed * Time.fixedDeltaTime;

            itemRigidbody.MovePosition(newPosition);
        }

        if (itemRigidbody.position.y < destroyY)
        {
            if (!itemResolved)
            {
                itemResolved = true;

                if (gameManager != null)
                {
                    gameManager.MissFood();
                }
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (itemResolved || freeFalling)
            return;

        if (other.GetComponent<BowlMovement>() != null)
        {
            itemResolved = true;

            if (gameManager != null)
            {
                gameManager.CatchFood();
            }

            Destroy(gameObject);
        }
    }

    public void DropAfterWin()
    {
        itemResolved = true;
        freeFalling = true;

        itemRigidbody.bodyType = RigidbodyType2D.Dynamic;
        itemRigidbody.gravityScale = winGravityScale;

        float randomHorizontal =
            UnityEngine.Random.Range(-hitStrength, hitStrength);

        float randomUpward =
            UnityEngine.Random.Range(0.5f, hitStrength);

        Vector2 hitDirection =
            new Vector2(randomHorizontal, randomUpward);

        itemRigidbody.AddForce(
            hitDirection,
            ForceMode2D.Impulse
        );

        float randomSpin =
            UnityEngine.Random.Range(-spinStrength, spinStrength);

        itemRigidbody.AddTorque(
            randomSpin,
            ForceMode2D.Impulse
        );
    }
}