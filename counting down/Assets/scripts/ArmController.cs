using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmController : MonoBehaviour
{
    public enum State
    {
        Rotating,
        Extending,
        Retracting,
        Cooldown
    }

    [Header("References")]
    [SerializeField] private Grab_hand grabHand;

    [Header("Speeds")]
    [Tooltip("Speed of the sweeping rotation (higher = faster)")]
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float extendSpeed = 5f;
    [SerializeField] private float retractSpeed = 8f;

    [Header("Rotation Limits")]
    [SerializeField] private float maxAngle = 60f;

    [Header("Extension Limits")]
    [Tooltip("Maximum distance the arm will travel before automatically retracting.")]
    [SerializeField] private float maxExtensionDistance = 10f;

    private State state = State.Rotating;
    private Transform handTransform;

    [Header("Movement Target")]
    [Tooltip("If true, move the parent of this GameObject instead of the grab hand.")]
    [SerializeField] private bool moveParentInstead = false;

    private Transform movingTransform;
    private Vector3 initialPosition;
    private InputAction fireAction;

    private void Awake()
    {
        if (grabHand == null)
        {
            grabHand = GetComponentInChildren<Grab_hand>();
        }

        if (grabHand == null)
        {
            Debug.LogError("ArmController: GrabHand reference not assigned and no GrabHand found in children.");
            enabled = false;
            return;
        }

        handTransform = grabHand.transform;

        if (moveParentInstead && transform.parent != null)
        {
            movingTransform = transform.parent;
        }
        else
        {
            movingTransform = handTransform;
        }

        initialPosition = movingTransform.position;
    }

    private void Start()
    {
        state = State.Rotating;
        grabHand.enabled = false;
        if (grabHand != null) grabHand.grabFinished = false;
    }

    private void OnEnable()
    {
        if (fireAction == null)
        {
            fireAction = new InputAction("Fire", InputActionType.Button);
            fireAction.AddBinding("<Keyboard>/space");
            fireAction.AddBinding("<Gamepad>/buttonSouth");
            fireAction.performed += OnFire;
        }
        fireAction.Enable();
    }

    private void OnDisable()
    {
        if (fireAction != null)
            fireAction.Disable();
    }

    private void OnDestroy()
    {
        if (fireAction != null)
        {
            fireAction.performed -= OnFire;
            fireAction.Dispose();
            fireAction = null;
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.Rotating:
                UpdateRotating();
                break;

            case State.Extending:
                UpdateExtending();

                // Expect GrabHand to set grabFinished = true when it grabbed something
                if (grabHand != null && grabHand.grabFinished)
                {
                    state = State.Retracting;
                }
                break;

            case State.Retracting:
                UpdateRetracting();
                break;

            case State.Cooldown:
                // idle until coroutine finishes
                break;
        }
    }

    private void UpdateRotating()
    {
        float angle = Mathf.PingPong(Time.time * rotationSpeed, maxAngle * 2f) - maxAngle;
        transform.localEulerAngles = new Vector3(0f, 0f, angle);
    }

    private void UpdateExtending()
    {
        movingTransform.position += (Vector3)transform.up * extendSpeed * Time.deltaTime;

        // Safety Catch: If we miss everything, auto-retract once we hit the max distance
        if (Vector3.Distance(initialPosition, movingTransform.position) >= maxExtensionDistance)
        {
            state = State.Retracting;

            // Force the hand to close if it reached max distance
            if (grabHand != null)
            {
                grabHand.grabFinished = true;
            }
        }
    }

    private void UpdateRetracting()
    {
        movingTransform.position = Vector3.MoveTowards(movingTransform.position, initialPosition, retractSpeed * Time.deltaTime);
        if (Vector3.Distance(movingTransform.position, initialPosition) <= 0.001f)
        {
            movingTransform.position = initialPosition;
            state = State.Cooldown;
            StartCoroutine(CooldownRoutine());
        }
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        if (grabHand != null)
        {
            grabHand.state = Grab_hand.HandState.IdleOpen;
            grabHand.grabFinished = false;
            grabHand.enabled = false;
        }

        state = State.Rotating;
    }

    private void OnFire(InputAction.CallbackContext ctx)
    {
        if (state != State.Rotating) return;

        // RECORD NEW LAUNCH POSITION RIGHT BEFORE FIRING
        initialPosition = movingTransform.position;

        state = State.Extending;

        // Start the Coroutine to delay enabling the hand to prevent double-inputs
        StartCoroutine(EnableHandNextFrame());
    }

    private IEnumerator EnableHandNextFrame()
    {
        // Wait exactly one frame so the Spacebar input is cleared before the hand script turns on
        yield return null;

        if (grabHand != null)
        {
            grabHand.enabled = true;
            grabHand.grabFinished = false;
        }
    }
}