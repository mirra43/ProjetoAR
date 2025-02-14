using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

public class RotateAndScale : MonoBehaviour
{
    // Toggle to track if the animation is active
    private bool isAnimating = false;

    // Rotation speed (degrees per second)
    public Vector3 rotationSpeed = new Vector3(0f, 90f, 0f);

    // Scaling settings
    private Vector3 initialScale;
    public float scaleAmount = 0.2f;  // Maximum change from the initial scale
    public float scaleSpeed = 2f;     // Speed of the scaling animation

    void Start()
    {
        // Store the object's starting scale
        initialScale = transform.localScale;
    }

    void Update()
    {
        // Check for touch input using the new Input System
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            ToggleAnimation();
        }
        // Check for mouse input using the new Input System
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            ToggleAnimation();
        }

        // If animation is active, rotate and scale the object every frame
        if (isAnimating)
        {
            AnimateObject();
        }
    }

    // Toggle the animation state
    void ToggleAnimation()
    {
        isAnimating = !isAnimating;
        Debug.Log("Animation active: " + isAnimating);
    }

    // Rotate and scale the object
    void AnimateObject()
    {
        // Rotate the object
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // Calculate scale offset using PingPong for smooth oscillation
        float scaleOffset = Mathf.PingPong(Time.time * scaleSpeed, scaleAmount * 2) - scaleAmount;
        transform.localScale = initialScale + Vector3.one * scaleOffset;
    }
}
