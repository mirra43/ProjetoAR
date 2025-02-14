using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PinchAndSlideRotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Rotation sensitivity. Increase to rotate faster.")]
    public float rotationSpeed = 0.1f;
    private Vector2 lastDragPosition;
    private bool isDragging = false;

    [Header("Scaling Settings")]
    [Tooltip("Scale sensitivity. Lower values require a larger pinch to change scale.")]
    public float scaleSpeed = 0.001f;
    public float minScale = 0.1f;
    public float maxScale = 10f;

    void Update()
    {
        if (Touchscreen.current != null)
        {
            // Count active touches
            int activeTouchCount = 0;
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed)
                    activeTouchCount++;
            }

            if (activeTouchCount >= 2)
            {
                // If two or more touches are active, handle pinch scaling.
                HandlePinchScaling();
                // Cancel any ongoing rotation drag if a second finger is added.
                isDragging = false;
            }
            else if (activeTouchCount == 1)
            {
                // If exactly one touch is active, handle rotation.
                HandleRotation();
            }
        }
        else
        {
            // Fallback to mouse input (useful for testing in the Editor)
            HandleMouseInput();
        }
    }

    // Handles pinch-to-scale.
    void HandlePinchScaling()
    {
        // Retrieve two active touches.
        TouchControl touch1 = null;
        TouchControl touch2 = null;
        foreach (var touch in Touchscreen.current.touches)
        {
            if (touch.press.isPressed)
            {
                if (touch1 == null)
                    touch1 = touch;
                else
                {
                    touch2 = touch;
                    break;
                }
            }
        }
        if (touch1 == null || touch2 == null)
            return;

        // Current positions.
        Vector2 pos1 = touch1.position.ReadValue();
        Vector2 pos2 = touch2.position.ReadValue();

        // Previous positions.
        Vector2 prevPos1 = pos1 - touch1.delta.ReadValue();
        Vector2 prevPos2 = pos2 - touch2.delta.ReadValue();

        // Compute distances.
        float currentDistance = Vector2.Distance(pos1, pos2);
        float previousDistance = Vector2.Distance(prevPos1, prevPos2);
        float deltaDistance = currentDistance - previousDistance;

        // Adjust scale (lower scaleSpeed -> less sensitive).
        Vector3 newScale = transform.localScale + Vector3.one * deltaDistance * scaleSpeed;
        newScale = Vector3.Max(newScale, Vector3.one * minScale);
        newScale = Vector3.Min(newScale, Vector3.one * maxScale);
        transform.localScale = newScale;
    }

    // Handles one-finger horizontal drag to rotate the object around the Y axis.
    void HandleRotation()
    {
        var touch = Touchscreen.current.primaryTouch;
        if (touch == null)
            return;

        // Fully qualify TouchPhase to use the new Input System's enum.
        UnityEngine.InputSystem.TouchPhase phase = touch.phase.ReadValue();
        if (phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            // Record the starting position.
            lastDragPosition = touch.position.ReadValue();
            isDragging = true;
        }
        else if (phase == UnityEngine.InputSystem.TouchPhase.Moved && isDragging)
        {
            // Calculate horizontal movement.
            Vector2 currentPos = touch.position.ReadValue();
            Vector2 delta = currentPos - lastDragPosition;
            float rotationAmount = delta.x * rotationSpeed;

            // Rotate the object around the Y axis.
            transform.Rotate(0f, 0f, -rotationAmount, Space.Self);

            // Update last position.
            lastDragPosition = currentPos;
        }
        else if (phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                 phase == UnityEngine.InputSystem.TouchPhase.Canceled)
        {
            // End of drag.
            isDragging = false;
        }
    }

    // Fallback mouse controls for testing in the Editor.
    void HandleMouseInput()
    {
        // Left mouse button drag rotates the object.
        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            float rotationAmount = mouseDelta.x * rotationSpeed;
            transform.Rotate(0f, 0f, -rotationAmount, Space.Self);
        }

        // Right mouse button drag scales the object.
        if (Mouse.current.rightButton.isPressed)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            float scaleChange = mouseDelta.y * scaleSpeed;
            Vector3 newScale = transform.localScale + Vector3.one * scaleChange;
            newScale = Vector3.Max(newScale, Vector3.one * minScale);
            newScale = Vector3.Min(newScale, Vector3.one * maxScale);
            transform.localScale = newScale;
        }
    }
}
