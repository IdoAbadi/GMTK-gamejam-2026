using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class BowlMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float leftLimit = -8f;
    [SerializeField] private float rightLimit = 8f;

    private void Update()
    {
        float direction = 0f;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed ||
                Keyboard.current.aKey.isPressed)
            {
                direction = -1f;
            }

            if (Keyboard.current.rightArrowKey.isPressed ||
                Keyboard.current.dKey.isPressed)
            {
                direction = 1f;
            }
        }
#else
        direction = Input.GetAxisRaw("Horizontal");
#endif

        Vector3 newPosition = transform.position;

        newPosition.x += direction * moveSpeed * Time.deltaTime;
        newPosition.x = Mathf.Clamp(
            newPosition.x,
            leftLimit,
            rightLimit
        );

        transform.position = newPosition;
    }
}