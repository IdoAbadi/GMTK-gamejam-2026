using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public class BroomLauncher : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] private float minimumX = -12f;
    [SerializeField] private float maximumX = 9f;

    [Header("Launch")]
    [SerializeField] private float minimumSpeed = 6f;
    [SerializeField] private float maximumSpeed = 30f;
    [SerializeField] private float maximumDragDistance = 5f;

    [Header("Aim Line")]
    [SerializeField] private float maximumLineLength = 6f;
    [SerializeField] private float lineWidth = 0.12f;
    [SerializeField] private Color weakColor = Color.yellow;
    [SerializeField] private Color strongColor = Color.red;

    [Header("Reset")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private float maximumFlightTime = 2f;

    private Rigidbody2D broomRigidbody;
    private LineRenderer aimLine;
    private Camera mainCamera;

    private Vector2 dragStart;

    private bool isAiming;
    private bool wasLaunched;
    private bool isResetting;

    private float flightTimer;
    private float selectedStartX;

    private Quaternion startingRotation;

    private void Awake()
    {
        broomRigidbody = GetComponent<Rigidbody2D>();
        aimLine = GetComponent<LineRenderer>();
        mainCamera = Camera.main;

        selectedStartX = transform.position.x;
        startingRotation = transform.rotation;

        aimLine.positionCount = 2;
        aimLine.startWidth = lineWidth;
        aimLine.endWidth = lineWidth * 0.4f;
        aimLine.useWorldSpace = true;
        aimLine.enabled = false;
    }

    private void Update()
    {
        if (!wasLaunched && !isAiming && !isResetting)
        {
            MoveAlongSofaStrip();
        }

        if (!wasLaunched &&
            !isResetting &&
            Input.GetMouseButtonDown(0))
        {
            StartAiming();
        }

        if (!wasLaunched && isAiming)
        {
            UpdateAimIndicator();

            if (Input.GetMouseButtonUp(0))
            {
                Launch();
            }
        }

        if (wasLaunched && !isResetting)
        {
            flightTimer += Time.deltaTime;

            if (flightTimer >= maximumFlightTime)
            {
                ResetBroom();
            }
        }
    }

    private void MoveAlongSofaStrip()
    {
        Vector2 mousePosition = GetMouseWorldPosition();

        selectedStartX = Mathf.Clamp(
            mousePosition.x,
            minimumX,
            maximumX
        );

        transform.position = new Vector3(
            selectedStartX,
            startPoint.position.y,
            transform.position.z
        );

        broomRigidbody.linearVelocity = Vector2.zero;
        broomRigidbody.angularVelocity = 0f;
    }

    private void StartAiming()
    {
        isAiming = true;
        dragStart = GetMouseWorldPosition();
        aimLine.enabled = true;
    }

    private void UpdateAimIndicator()
    {
        Vector2 currentMousePosition = GetMouseWorldPosition();
        Vector2 dragVector = dragStart - currentMousePosition;

        float dragDistance = Mathf.Min(
            dragVector.magnitude,
            maximumDragDistance
        );

        float powerPercent = dragDistance / maximumDragDistance;

        Vector2 direction = dragVector.normalized;

        Vector2 lineStart = transform.position;

        Vector2 lineEnd =
            lineStart +
            direction * maximumLineLength * powerPercent;

        aimLine.SetPosition(0, lineStart);
        aimLine.SetPosition(1, lineEnd);

        Color currentColor = Color.Lerp(
            weakColor,
            strongColor,
            powerPercent
        );

        aimLine.startColor = currentColor;
        aimLine.endColor = currentColor;

        if (direction.sqrMagnitude > 0.01f)
        {
            float angle =
                Mathf.Atan2(direction.y, direction.x) *
                Mathf.Rad2Deg - 90f;

            transform.rotation =
                Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void Launch()
    {
        Vector2 dragEnd = GetMouseWorldPosition();
        Vector2 dragVector = dragStart - dragEnd;

        float dragDistance = Mathf.Min(
            dragVector.magnitude,
            maximumDragDistance
        );

        float powerPercent =
            dragDistance / maximumDragDistance;

        isAiming = false;
        aimLine.enabled = false;

        if (powerPercent < 0.05f)
        {
            transform.rotation = startingRotation;
            return;
        }

        Vector2 direction = dragVector.normalized;

        float launchSpeed = Mathf.Lerp(
            minimumSpeed,
            maximumSpeed,
            powerPercent
        );

        broomRigidbody.linearVelocity =
            direction * launchSpeed;

        flightTimer = 0f;
        wasLaunched = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!wasLaunched || isResetting)
        {
            return;
        }

        if (collision.gameObject.CompareTag("CatToy"))
        {
            StartCoroutine(ResetAfterToyCollision());
        }
    }

    private IEnumerator ResetAfterToyCollision()
    {
        isResetting = true;

        yield return new WaitForFixedUpdate();

        ResetBroom();
    }

    private void ResetBroom()
    {
        broomRigidbody.linearVelocity = Vector2.zero;
        broomRigidbody.angularVelocity = 0f;

        transform.position = new Vector3(
            selectedStartX,
            startPoint.position.y,
            startPoint.position.z
        );

        transform.rotation = startingRotation;

        aimLine.enabled = false;

        flightTimer = 0f;
        isAiming = false;
        wasLaunched = false;
        isResetting = false;
    }

    private Vector2 GetMouseWorldPosition()
    {
        Vector3 mousePosition =
            mainCamera.ScreenToWorldPoint(Input.mousePosition);

        return new Vector2(
            mousePosition.x,
            mousePosition.y
        );
    }
}