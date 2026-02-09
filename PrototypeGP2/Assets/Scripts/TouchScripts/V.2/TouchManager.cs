using _Project._Scripts.Sound_and_Music;
using UnityEngine;

using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour
{
    [Header("References")]
    public Camera cam;

    [Header("Layers")]
    public LayerMask draggableMask;

    private DragItem current;

    private void Update()
    {
        cam = Camera.main;
        
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    TryPick(touch.position);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    current?.Drag(touch.position);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    Release();
                    break;
            }
        }

#if UNITY_EDITOR
    if (Input.GetMouseButtonDown(0)) TryPick(Input.mousePosition);
    if (Input.GetMouseButton(0)) current?.Drag(Input.mousePosition);
    if (Input.GetMouseButtonUp(0)) Release();
#endif
    }

    void TryPick(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, draggableMask))
        {
            Debug.Log("Ray hit: " + hit.collider.name);

            current = hit.collider.GetComponentInParent<DragItem>();
            if (current != null)
            {
                Debug.Log("Found DragItem: " + current.name);
                current.BeginDrag(hit.point);

                //-----FMOD Integration-----
                AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
                AudioManager.Instance.PlayMusic(FMODEvents.instance.CookMusic);


            }
            else
            {
                Debug.LogWarning("Ray hit something but no DragItem component in parent!");
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


