using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CombinedRotationAndScale : MonoBehaviour
{
    public float dragRotationSpeed = 0.1f;
    private Vector2 lastDragPosition;
    private bool isDragging = false;

    public float pinchScaleSpeed = 0.001f;
    public float minScale = 0.1f;
    public float maxScale = 10f;

    public Vector3 animationRotationSpeed = new Vector3(0f, 90f, 0f);
    public float animationScaleAmount = 0.2f;
    public float animationScaleSpeed = 2f;
    private Vector3 animationInitialScale;
    private bool isAnimating = false;

    void Start()
    {
        animationInitialScale = transform.localScale;
    }

    void Update()
    {
        if (Touchscreen.current != null)
        {
            int activeTouchCount = 0;
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed)
                    activeTouchCount++;
            }

            if (activeTouchCount >= 3)
            {
                isAnimating = true;
                isDragging = false;
            }
            else
            {
                isAnimating = false;

                if (activeTouchCount == 2)
                {
                    HandlePinchScaling();
                    isDragging = false;
                }
                else if (activeTouchCount == 1)
                {
                    HandleRotation();
                }
            }
        }

        if (isAnimating)
        {
            AnimateObject();
        }
    }

    void HandleRotation()
    {
        var touch = Touchscreen.current.primaryTouch;
        if (touch == null)
            return;

        UnityEngine.InputSystem.TouchPhase phase = touch.phase.ReadValue();
        if (phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            lastDragPosition = touch.position.ReadValue();
            isDragging = true;
        }
        else if (phase == UnityEngine.InputSystem.TouchPhase.Moved && isDragging)
        {
            Vector2 currentPos = touch.position.ReadValue();
            Vector2 delta = currentPos - lastDragPosition;
            float rotationAmount = delta.x * dragRotationSpeed;

            transform.Rotate(0f, -rotationAmount, 0f, Space.World);

            lastDragPosition = currentPos;
        }
        else if (phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                 phase == UnityEngine.InputSystem.TouchPhase.Canceled)
        {
            isDragging = false;
        }
    }

    void HandlePinchScaling()
    {
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

        Vector2 pos1 = touch1.position.ReadValue();
        Vector2 pos2 = touch2.position.ReadValue();

        Vector2 prevPos1 = pos1 - touch1.delta.ReadValue();
        Vector2 prevPos2 = pos2 - touch2.delta.ReadValue();

        float currentDistance = Vector2.Distance(pos1, pos2);
        float previousDistance = Vector2.Distance(prevPos1, prevPos2);
        float deltaDistance = currentDistance - previousDistance;

        Vector3 newScale = transform.localScale + Vector3.one * deltaDistance * pinchScaleSpeed;
        newScale = Vector3.Max(newScale, Vector3.one * minScale);
        newScale = Vector3.Min(newScale, Vector3.one * maxScale);
        transform.localScale = newScale;
    }

    void AnimateObject()
    {
        transform.Rotate(animationRotationSpeed * Time.deltaTime, Space.World);

        float scaleOffset = Mathf.PingPong(Time.time * animationScaleSpeed, animationScaleAmount * 2) - animationScaleAmount;
        transform.localScale = animationInitialScale + Vector3.one * scaleOffset;
    }
}
