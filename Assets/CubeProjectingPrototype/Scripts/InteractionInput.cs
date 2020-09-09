using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionInput : MonoBehaviour {

    public Transform projectionCube;
    public Transform filament;
    public float speed = 20;
    Vector2 oldPosition;
    Camera camera;

    // Use this for initialization
    void Start () {
        camera = GetComponent<Camera>();
	}


    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetMouseButtonDown(0))
            {
                oldPosition = Input.mousePosition;
            }
            // rotate cube y and x
            RotateCube(0);
        }
        if (Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftControl))
        {
            // rotate cube z
            RotateCube(1);
        }
        if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl))
        {
            // move filament y and x
            MoveFilament(0);
        }
        if (Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftControl))
        {
            // move filament z
            MoveFilament(1);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "ProjectionFaces")
                {
                    print("hit.transform.GetComponent<ProjectionFill>().Camera.transform " + hit.transform.GetComponent<ProjectionFill>().CubeFaceCamera.transform.name);
                    filament.GetComponent<FilamentMovement>().moveDirection = hit.transform.GetComponent<ProjectionFill>().CubeFaceCamera.transform.position - filament.parent.localPosition;
                }
            }
        }

        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            ZoomFilament(Input.GetAxis("Mouse ScrollWheel"));
        }
    }

    void RotateCube(int axis)
    {
        Vector3 direction = transform.position - projectionCube.position;
        
        switch (axis)
        {
            case 0:
                if (Mathf.Abs(oldPosition.x - Input.mousePosition.x) > Mathf.Abs(oldPosition.y - Input.mousePosition.y))
                {
                    projectionCube.Rotate(Vector3.up, -Input.GetAxis("Mouse X") * Time.deltaTime * speed, Space.World);
                }
                else
                {
                    projectionCube.Rotate(Vector3.left, -Input.GetAxis("Mouse Y") * Time.deltaTime * speed, Space.World);
                }

                break;
            case 1:
                projectionCube.RotateAround(transform.position, direction, Input.GetAxis("Mouse X") * Time.deltaTime * speed);
                break;
            default:
                break;
        }
    }

    void MoveFilament (int axis)
    {
        switch (axis)
        {
            case 0:

                break;
            case 1:

                break;
            default:
                break;
        }
    }

    void ZoomFilament(float scroolWheelDelta)
    {
        filament.GetComponent<FilamentMovement>().Move(scroolWheelDelta);
    }
    
}
