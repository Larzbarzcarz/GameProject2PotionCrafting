using _Project._Scripts.Sound_and_Music;
using System.Collections;
using UnityEngine;

using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour
{
    [Header("References")]
    public Camera cam;

    [Header("Layers")]
    public LayerMask draggableMask;

    private DragItem current;
    [Header("Potion Throw")]
    public PotionThrowController potionThrow;

    //-----FMOD Integration-----
    public bool weCooking = false;

    void Awake()
    {
        GameObject gameObject = GameObject.FindGameObjectWithTag("Craft");
        cam = Camera.main;
        if (!cam)
            cam = Camera.main;

        if (!cam)
            Debug.LogError("DragManager: No camera assigned");
    }

    private void Update()
    {
if (Input.touchCount > 0)
{
    Debug.Log("Touch detected");
}
      cam = Camera.main;
        
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
			  if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;		

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
Debug.DrawRay(ray.origin, ray.direction * 5f, Color.red, 1f);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, draggableMask))
        {
            Debug.Log("Ray hit: " + hit.collider.name);

            current = hit.collider.GetComponentInParent<DragItem>();
            if (current != null)
            {
                Debug.Log("Found DragItem: " + current.name);
                current.BeginDrag(hit.point);

                //-----FMOD Integration-----
                if (!weCooking)
                {
                    StartCoroutine(cookDelay());
                }
                else
                {
                    AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
                }
                    
                


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

    private IEnumerator cookDelay()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.bass);
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlayMusic(FMODEvents.instance.cookMusic);
        weCooking = true;
    }
}


