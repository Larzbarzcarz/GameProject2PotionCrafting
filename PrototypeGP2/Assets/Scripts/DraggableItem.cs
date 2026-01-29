using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    public int ingredientID;
    public float followSpeed;
    public LayerMask groundMask;

    private Camera cam;
    private Rigidbody rb;
    private Vector3 grabOffset;
    private bool isDragging;
    private float dragY;
    private Ray ray;


    private void Start()
    {
        SetCamera();
    }

    private void SetCamera()
    {
        if (cam != null)
            return;

        cam = Camera.main;
        if (cam == null)
        {
            cam = FindFirstObjectByType<Camera>();
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
    }

    public void BeginDrag(Vector3 hitPoint)
    {
        isDragging = true;
        dragY = transform.position.y;
        grabOffset = transform.position - hitPoint;
    }

    public void Drag(Vector2 screenPos)
    {
        if (!isDragging)
            return;

        SetCamera();
        if (cam == null)
            return;

        Ray ray = cam.ScreenPointToRay(screenPos);
        this.ray = ray;

        if (Physics.Raycast(ray, out RaycastHit hit, 200f, groundMask, QueryTriggerInteraction.Collide))
        {
            Debug.Log("GROUND HIT: " + hit.collider.name);
            Vector_attach(hit.point);
        }
        else
        {
            Debug.Log("NO GROUND HIT");
        }
    }

    public void EndDrag()
    {
        isDragging = false;
    }

    private void Vector_attach(Vector3 groundPoint)
    {
        Vector3 target = groundPoint + grabOffset;
        target.y = dragY;

        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * followSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray);

    }
}