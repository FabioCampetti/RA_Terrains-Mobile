using UnityEngine;

public class ObjectViewer : MonoBehaviour {
    private bool isRotating = false;
    private Vector2 lastTouchPosition;

    public float rotationSpeed = 0.2f;
    public float zoomSpeed = 0.05f;

    private float initialPinchDistance;

    void Update() {
        if (Input.touchCount == 1) {
            Rotate();
        }
        else if (Input.touchCount == 2) {
            Zoom();
        }
    }

    private void Rotate() {

        Touch touch = Input.GetTouch(0);

        // Handle rotation
        if (touch.phase == TouchPhase.Began) {
            isRotating = true;
            lastTouchPosition = touch.position;
        }
        else if (touch.phase == TouchPhase.Ended) {
            isRotating = false;
        }

        if (isRotating) {
            Vector2 touchDelta = touch.position - lastTouchPosition;

            // Invert the movement along the X-axis
            touchDelta.x *= -1.0f;

            // Rotate around the Y-axis (vertical)
            transform.Rotate(Vector3.up, touchDelta.x * rotationSpeed, Space.World);

            // Rotate around the X-axis (horizontal)
            transform.Rotate(Vector3.right, touchDelta.y * rotationSpeed, Space.World);
        }

        lastTouchPosition = touch.position;
    }

    private void Zoom() {
        // Check for pinch gesture
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began) {
            // Store initial pinch distance
            initialPinchDistance = Vector2.Distance(touch1.position, touch2.position);
        }
        else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved) {
            // Calculate current pinch distance
            float currentPinchDistance = Vector2.Distance(touch1.position, touch2.position);

            // Calculate pinch delta
            float pinchDelta = initialPinchDistance - currentPinchDistance;

            // Adjust zoom based on pinch delta
            Camera.main.fieldOfView += pinchDelta * zoomSpeed;

            // Update initial pinch distance for the next frame
            initialPinchDistance = currentPinchDistance;
        }
    }
}
