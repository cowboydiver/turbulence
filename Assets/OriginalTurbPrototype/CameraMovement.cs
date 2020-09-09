using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    #region Variables
    const float RotateSpeed = 0.15f;
    const float MinYRotation = -90f;//40f;
    const float MaxYRotation = 90f;
    const float ZoomSpeed = 4f;
    const float CameraLerpSpeed = 10f;

    public GameObject filamentObject;

    public TargetSphere Target;
    public GhostSphere GhostSphere;
    //public GameObject GridLinePrefab;

    //new Transform transform;
    new Camera camera;

    float minZoomDistance = 0.4f;
    float maxZoomDistance = 5f;

    float zoomDistance = 1.5f;
    float currentActualZoomDistance;
    Vector3 lookDirection;
    Vector2 rotationAngles;

    bool clickedGhostSphere;
    Vector3 clickedGhostSpherePosition;
    bool ghostSphereOver;

    bool rotating;
    Vector3 initialMousePosition;
    Vector2 initialRotation;
    Quaternion rotation;
    Vector3 rotationForward;

    bool moving;
    Vector3 initialSpherePosition;
    Vector3 initialIntersection;
    RaycastHit hitInfo;
    Ray ray;
    int targetSphereLayerMask;
    int ghostSphereLayerMask;

    LineRenderer[] gridLines;
    float currentGridLinesZDistance;
    #endregion
    #region Mono

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }
    void LateUpdate()
    {
        if (Target != null)
        {

            //GhostSphereInput();
            MoveInput();
            ZoomInput();
            RotateInput();
        }
    }
    #endregion
    #region Public Methods
    //public void Init(float maxDistance)
    //{

    //    initialRotation = new Vector2(0f, 0f);
    //    rotationAngles = initialRotation;

    //    //CreateGrid();

    //    GhostSphere.Init();

    //    targetSphereLayerMask = LayerMask.GetMask(LayerMask.LayerToName(Target.Transform.gameObject.layer));
    //    ghostSphereLayerMask = LayerMask.GetMask(LayerMask.LayerToName(GhostSphere.Transform.gameObject.layer));

    //    SetMaxZoomDistance(maxDistance);

    //    //SetTargetPosition(FilamentSceneLogic.Inst.FilamentBounds.center);
    //}

    public void SetMaxZoomDistance(float maxDistance)
    {
        maxZoomDistance = maxDistance;
        SetZoom(zoomDistance);
    }

    public void JumpToGhostSphere()
    {
        Target.Transform.position = GhostSphere.Transform.position;
        Target.UpdateSphereSize();
        //AudioManager.Inst.PlayJumpToBest();
    }

    public void SetTargetPosition(Vector3 position)
    {
        Target.Transform.position = position;

        Target.UpdateSphereSize();

        //UIManager.Inst.GetMenu<FilamentMenu>(4).UpdateCurrentRadius((Target.IsInsideMesh ? Target.Radius : 0f));

        if (Target.IsInsideMesh)
        {
            GhostSphere.SetPosition(position, Target.Radius);
        }
    }
    #endregion
    #region Private Methods
    void GhostSphereInput()
    {
        if(Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hitInfo, 200f, ghostSphereLayerMask))
        {
            if(!ghostSphereOver)
            {
                ghostSphereOver = true;
                GhostSphere.SetOver(true);
            }

            if (!moving && !rotating)
            {
                if (!clickedGhostSphere && Input.GetMouseButton(0))
                {
                    clickedGhostSphere = true;
                    clickedGhostSpherePosition = Input.mousePosition;
                }
                else if (clickedGhostSphere && !Input.GetMouseButton(0))
                {
                    clickedGhostSphere = false;

                    if ((Input.mousePosition - clickedGhostSpherePosition).magnitude < 4f)
                    {
                        //Clicked ghost sphere, snap target sphere to same position
                        JumpToGhostSphere();
                    }
                }
            }
        }
        else
        {
            if(ghostSphereOver)
            {
                ghostSphereOver = false;
                GhostSphere.SetOver(false);
            }
        }
    }

    void MoveInput()
    {
        if(!moving && Input.GetMouseButtonDown(0)/* &&
           Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hitInfo, 200f, sphereLayerMask) && !Input.GetKey(KeyCode.LeftControl) && !EventSystem.current.IsPointerOverGameObject()*/)
        {
            moving = true;
            initialSpherePosition = Target.transform.position;

            ray = camera.ScreenPointToRay(Input.mousePosition);
            MathHelper.LinePlaneIntersection(out initialIntersection, ray.origin, ray.direction, transform.forward, initialSpherePosition);
        }
        else if(moving && !Input.GetMouseButton(0))
        {
            moving = false;
        }

        if(moving)
        {
            Vector3 intersection;
            ray = camera.ScreenPointToRay(Input.mousePosition);

            if (MathHelper.LinePlaneIntersection(out intersection, ray.origin, ray.direction, transform.forward, initialSpherePosition))
            {
                Vector3 newPosition = initialSpherePosition + (intersection - initialIntersection);
                Bounds bounds = filamentObject.GetComponent<MeshRenderer>().bounds;

                newPosition.x = Mathf.Clamp(newPosition.x, bounds.min.x, bounds.max.x);
                newPosition.y = Mathf.Clamp(newPosition.y, bounds.min.y, bounds.max.y);
                newPosition.z = Mathf.Clamp(newPosition.z, bounds.min.z, bounds.max.z);

                if(Target.Transform.position != newPosition)
                {
                    SetTargetPosition(newPosition);
                }
            }
        }

        //Debug.Log("Moving");
    }

    void ZoomInput()
    {
        if(Input.mouseScrollDelta.y != 0f || minZoomDistance != Target.Transform.localScale.x * 0.5f)
        {
            if (Input.mouseScrollDelta.y > 0) {
            }
            else if(Input.mouseScrollDelta.y < 0) {
            }
            minZoomDistance = Target.Transform.localScale.x * 0.5f;
            SetZoom(zoomDistance - (Input.mouseScrollDelta.y * ZoomSpeed * Time.deltaTime));
        }
    }

    void RotateInput()
    {
        if (!moving && !rotating && (Input.GetMouseButton(1) || Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl)))
        {
            rotating = true;
            initialMousePosition = Input.mousePosition;
            rotationAngles = initialRotation;
        }
        else if (moving || rotating && !(Input.GetMouseButton(1) || Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl)))
        {
            rotating = false;
            initialRotation = rotationAngles;
        }
        else if (rotating)
        {
            rotationAngles = (Input.mousePosition - initialMousePosition) * RotateSpeed;
            rotationAngles.y = -rotationAngles.y;
            rotationAngles = initialRotation + rotationAngles;
            rotationAngles.y = Mathf.Clamp(rotationAngles.y, MinYRotation, MaxYRotation);
        }

        rotation = Quaternion.Euler(rotationAngles.y, rotationAngles.x, 0f);

        transform.rotation = rotation;
        rotationForward = rotation * Vector3.forward;

        currentActualZoomDistance = zoomDistance;

        if (!moving && rotating)
        {
            transform.position = Target.Transform.position - rotationForward * zoomDistance;
        }
        else if(!moving)
        {
            transform.position = Vector3.Lerp(transform.position, Target.Transform.position - rotationForward * zoomDistance, CameraLerpSpeed * Time.deltaTime);

            Vector3 intersection;
            MathHelper.LinePlaneIntersection(out intersection, transform.position, rotationForward, rotationForward, Target.Transform.position);
            currentActualZoomDistance = (intersection - transform.position).magnitude;
        }
        else
        {
            transform.position = initialSpherePosition - rotationForward * zoomDistance;
        }

        if(currentGridLinesZDistance != currentActualZoomDistance)
        {
            currentGridLinesZDistance = currentActualZoomDistance;
            //UpdateGrid();
        }
    }

    void SetZoom(float zoom)
    {
        //print("camera " + camera.name);
        zoomDistance = Mathf.Clamp(zoom, minZoomDistance + camera.nearClipPlane, maxZoomDistance);
    }

    //void CreateGrid()
    //{
    //    return;
    //    const int AxisLineCount = 100;
    //    const float Size = 2f;
    //    const float HalfSize = Size * 0.5f;
    //    const float GapSize = Size / AxisLineCount;

    //    Vector2 initialPoint = -Vector2.one * HalfSize;
    //    Vector3 point;

    //    gridLines = new LineRenderer[AxisLineCount + AxisLineCount + 2];
    //    bool xAxis;

    //    for (int i = 0; i < 2; i++)
    //    {
    //        xAxis = i == 0;
    //        for (int j = 0; j < (AxisLineCount + 1); j++)
    //        {
    //            Transform lineTransform = Instantiate(GridLinePrefab, transform).transform;
    //            lineTransform.localPosition = Vector3.zero;
    //            lineTransform.localScale = Vector3.one;
    //            lineTransform.localRotation = Quaternion.identity;

    //            LineRenderer line = lineTransform.GetComponent<LineRenderer>();

    //            if(xAxis)
    //            {
    //                point = new Vector3(initialPoint.x + (j * GapSize), -HalfSize, 0f);
    //            }
    //            else
    //            {
    //                point = new Vector3(-HalfSize, initialPoint.y + (j * GapSize), 0f);
    //            }

    //            line.SetPosition(0, point);
                
    //            if(xAxis)
    //            {
    //                point.y = HalfSize;
    //            }
    //            else
    //            {
    //                point.x = HalfSize;
    //            }

    //            line.SetPosition(1, point);

    //            gridLines[(i * (AxisLineCount + 1)) + j] = line;
    //        }
    //    }
    //}

    //void UpdateGrid()
    //{
    //    return;
    //    Vector3 point;
    //    for (int i = 0; i < gridLines.Length; i++)
    //    {
    //        for (int j = 0; j < 2; j++)
    //        {
    //            point = gridLines[i].GetPosition(j);
    //            point.z = currentGridLinesZDistance;
    //            gridLines[i].SetPosition(j, point);
    //        }
    //    }
    //}
    #endregion
}