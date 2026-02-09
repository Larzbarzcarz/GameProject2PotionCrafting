using UnityEngine;



public class ItemDragController : MonoBehaviour
{
    private DragItem dragItem;
    public Camera cam;

    void Awake()
    {
     	dragItem = GetComponentInParent<DragItem>();
        cam = dragItem.cam != null ? dragItem.cam : Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryBeginDrag();

        if (Input.GetMouseButton(0))
            dragItem.Drag(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
            dragItem.EndDrag();
    }

	    void TryBeginDrag()
    {
	
		Camera cam = dragItem.cam;
        if (cam !=  null)
        { Debug.Log(cam.name);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                    dragItem.BeginDrag(hit.point);
            }
        }
       
    }
}


