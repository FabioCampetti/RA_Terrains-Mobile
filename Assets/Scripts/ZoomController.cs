using UnityEngine;

public class ZoomController : MonoBehaviour {
    public float zoomSpeed = 0.0000000000000001f;

    private float initialPinchDistance;

    void Update()
    {
        // Check for pinch gesture
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                // Store initial pinch distance
                initialPinchDistance = Vector2.Distance(touch1.position, touch2.position);
            }
            else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                // Calculate current pinch distance
                float currentPinchDistance = Vector2.Distance(touch1.position, touch2.position);

                // Calculate pinch delta
                float pinchDelta = currentPinchDistance - initialPinchDistance;

                // Adjust zoom based on pinch delta
                Camera.main.fieldOfView += pinchDelta * zoomSpeed;

                // Update initial pinch distance for the next frame
                initialPinchDistance = currentPinchDistance;
            }
        }
    }
}