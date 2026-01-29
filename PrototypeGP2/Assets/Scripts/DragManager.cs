using UnityEngine;

public class DragManager : MonoBehaviour
{
    public LayerMask draggableMask;
    private Camera cam;
    private DraggableItem current;
    private Vector3 lastHitPoint;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) TryPick(touch.position);
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) current?.Drag(touch.position);
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) Release();

            return;
        }

        if (Input.GetMouseButtonDown(0)) TryPick(Input.mousePosition);
        if (Input.GetMouseButton(0)) current?.Drag(Input.mousePosition);
        if (Input.GetMouseButtonUp(0)) Release();
    }

    void TryPick(Vector2 screenPos)
    {
        //Ray ray = cam.ScreenPointToRay(screenPos);
        //if (Physics.Raycast(ray, out RaycastHit hit, 200f, draggableMask))
        //{
        //    current = hit.collider.GetComponentInParent<DraggableItem>();
        //    if (current != null)
        //    {
        //        lastHitPoint = hit.point;
        //        current.BeginDrag(hit.point);
        //    }

        //}

        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, ~0, QueryTriggerInteraction.Collide))
        {
            Debug.Log($"HIT: {hit.collider.name} | Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)} | Tag: {hit.collider.tag}");

            current = hit.collider.GetComponentInParent<DraggableItem>();
            if (current != null)
            {
                Debug.Log("Picked up draggable item: " + current.name);
                current.BeginDrag(hit.point);
            }
            else
            {
                Debug.Log("Hit something, but it has NO DraggableWorldItem component in parent.");
            }
        }
        else
        {
            Debug.Log("Raycast hit NOTHING");
        }
    }

    void Release()
    {
        if (current == null)
            return;

        current.EndDrag();
        current = null;
    }
}
