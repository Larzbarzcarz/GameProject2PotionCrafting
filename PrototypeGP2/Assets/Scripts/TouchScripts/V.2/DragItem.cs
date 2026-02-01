using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class DragItem : MonoBehaviour
{
    public LayerMask groundMask;
    public Camera cam;
    
    private Rigidbody rb;
    private Vector3 grabOffset;
    private bool dragging = false;
    private Ray ray;
    public float dragHeight = 0.3f;
    public float dragDistanceFromCamera = 2f;
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
        Debug.Log("BEGIN DRAG");
        dragging = true;
      
        grabOffset = transform.position - hitPoint;
    }

   
    public void Drag(Vector2 screenPos)
    {
        if (!dragging || cam == null || rb == null) return;

      
        Vector3 screenPosWithZ = new Vector3(screenPos.x, screenPos.y, dragDistanceFromCamera);
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPosWithZ);

   
        Vector3 target = worldPos + grabOffset;

     
        target += Vector3.up * dragHeight;

        rb.MovePosition(target);
    }


    public void EndDrag()
    {
        dragging = false;
        rb.isKinematic = true;
        rb.useGravity = false;
    }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray);
        }
  
    
}


