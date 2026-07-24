using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class Grab_hand : MonoBehaviour
{
    public enum HandState
    {
        IdleOpen,
        Closed
    }

    public HandState state = HandState.IdleOpen;

    [SerializeField] private Animator animator;
    [SerializeField] private string grabTriggerName = "Grab";

    public bool grabFinished = false;

    public List<GameObject> overlappingObjects = new List<GameObject>();
    private GameObject grabbedObject;

    void Update()
    {
        // 1. Check for missing/destroyed objects and clear them instantly
        // This is necessary because your external script will Destroy() objects at the grab box
        overlappingObjects.RemoveAll(item => item == null);
        if (grabbedObject == null) grabFinished = false; // Reset if our held object was destroyed

        if (IsSpacePressed() && state == HandState.IdleOpen)
        {
            state = HandState.Closed;
            grabFinished = true; // Signal arm to retract

            if (animator != null)
            {
                animator.SetTrigger(grabTriggerName);
            }

            if (overlappingObjects.Count > 0)
            {
                // choose the top-most object (highest sortingOrder) if any
                grabbedObject = SelectTopMostObject();

                // Attach to hand if it's a prize or can
                if (grabbedObject != null && (grabbedObject.CompareTag("prize") || grabbedObject.CompareTag("can")))
                {
                    AttachGrabbedToHand();
                }
                else
                {
                    grabbedObject = null;
                }
            }
        }
    }

    private bool IsSpacePressed()
    {
        var kb = Keyboard.current;
        if (kb != null && kb.spaceKey.wasPressedThisFrame)
        {
            return true;
        }

        var gp = Gamepad.current;
        if (gp != null && gp.buttonSouth.wasPressedThisFrame)
        {
            return true;
        }

        return false;
    }

    private GameObject SelectTopMostObject()
    {
        GameObject best = null;
        int bestOrder = int.MinValue;

        foreach (var go in overlappingObjects)
        {
            if (go == null) continue;

            var sr = go.GetComponent<SpriteRenderer>();
            int order = sr != null ? sr.sortingOrder : int.MinValue;

            if (best == null || order > bestOrder)
            {
                best = go;
                bestOrder = order;
            }
        }

        return best;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore the grab box so it doesn't get added to the overlap list
        if (other.attachedRigidbody != null &&
            other.name != "grab box" &&
            !overlappingObjects.Contains(other.gameObject))
        {
            overlappingObjects.Add(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // NOTE: Make sure your screen edges have the tag "Edge" in the Unity Editor!
        // This stops the claw from instantly bouncing back if it bumps something random.
        if (collision.gameObject.CompareTag("Edge"))
        {
            if (animator != null)
            {
                animator.speed = 3f;
                animator.SetTrigger(grabTriggerName);
                grabbedObject = null;
            }

            grabFinished = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) return; // Safeguard for destroyed objects

        overlappingObjects.Remove(other.gameObject);

        if (grabbedObject == other.gameObject)
        {
            grabbedObject = null;
        }
    }

    public void OnGrabAnimationComplete()
    {
        if (animator != null)
        {
            animator.speed = 1f;
        }
        state = HandState.Closed;
        grabFinished = true;
    }

    public GameObject GrabbedObject => grabbedObject;

    public IReadOnlyList<GameObject> OverlappingObjects => overlappingObjects.AsReadOnly();

    private void AttachGrabbedToHand()
    {
        if (grabbedObject == null) return;

        // Use modern parenting to keep scale/position stable
        grabbedObject.transform.SetParent(this.transform, true);

        // Make kinematic to prevent Unity physics from freaking out while the arm drags it
        var rb = grabbedObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}