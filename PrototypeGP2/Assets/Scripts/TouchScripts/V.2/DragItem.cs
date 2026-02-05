using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class DragItem : MonoBehaviour
{
    public Camera cam;
    public float dragHeight = 0.9f;
    public float planeDistance = 0.6f; 

    private Rigidbody rb;
    private bool dragging;
    private Vector3 grabOffset;
    private Plane dragPlane;

    void Awake()
    {
        if (!cam)
            cam = Camera.main;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public void BeginDrag(Vector3 hitPoint)
    {
        dragging = true;

     
        dragPlane = new Plane(
            -cam.transform.forward,
            cam.transform.position + cam.transform.forward * planeDistance
        );

        grabOffset = transform.position - hitPoint;
    }

    public void Drag(Vector2 screenPos)
    {
 		 if (rb == null)
    {
        Debug.LogWarning("Rigidbody is missing, cannot drag!");
        return;
    }
        if (!dragging) return;

        Ray ray = cam.ScreenPointToRay(screenPos);

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 worldPos = ray.GetPoint(enter);
            Vector3 target = worldPos + grabOffset;
            target.y += dragHeight;

            rb.MovePosition(target);
        }
    }

    public void EndDrag()
    {
        dragging = false;
        //rb.isKinematic = false;
        //rb.useGravity = true;
    }
}

