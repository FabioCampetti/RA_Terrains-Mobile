using UnityEngine;

public class ObjectViewer : MonoBehaviour
{
    private bool isRotating = false;
    private Vector2 lastTouchPosition;

    public float touchRotationSpeed = 1.0f;
    public float touchZoomSpeed = 1.0f;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Handle rotation
            if (touch.phase == TouchPhase.Began)
            {
                isRotating = true;
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isRotating = false;
            }

            if (isRotating)
            {
                Vector2 touchDelta = touch.position - lastTouchPosition;

                // Invert the movement along the X-axis
                touchDelta.x *= -1.0f;

                // Rotate around the Y-axis (vertical)
                transform.Rotate(Vector3.up, touchDelta.x * touchRotationSpeed, Space.World);

                // Rotate around the X-axis (horizontal)
                transform.Rotate(Vector3.right, touchDelta.y * touchRotationSpeed, Space.World);
            }

            // Handle zoom
            float zoomDelta = touch.deltaPosition.y * touchZoomSpeed * Time.deltaTime;
            transform.Translate(Vector3.forward * zoomDelta, Space.Self);

            lastTouchPosition = touch.position;
        }
    }
}
