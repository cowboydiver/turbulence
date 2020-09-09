using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TargetSphere : MonoBehaviour
{
    #region Variables
    const float MaxOutsideSphereRadius = 0.05f;
    const float OuterPointDistance = 100f;
    static readonly Vector3[] outerPoints = new Vector3[6] {Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
    //new Vector3(0, 1, 1), new Vector3(1, 0, 1), new Vector3(1, 1, 0f), new Vector3(0, -1, -1), new Vector3(-1, 0, -1), new Vector3(-1, -1, 0), new Vector3(0, 1, -1), new Vector3(1, 0, -1), new Vector3(1, -1, 0), new Vector3(0, -1, 1), new Vector3(-1, 0, -1), new Vector3(0, 1, -1), new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(1, -1, 1), new Vector3(1, 1, -1), 

    GameObject filament;
    public LineRenderer[] lineRenderer;

    public bool active;

    public GameObject[] sphereDot;
    public GameObject sphereDotPrefab;
    public GameObject ghostSphere;
    public Color[] sphereColor;
    Material sphereMaterial;

    List<GameObject> hitSpheres = new List<GameObject>();
    public GameObject hitSpherePrefab;
    public Transform hitSphereInitPos;

    public Transform Transform { get { return transform; } }

    public float Radius { get { return shortestDistance; } }

    public bool IsInsideMesh { get { return isInsideMesh; } }

    public bool isWiggling { get; private set; }

    Light sphereLight;
    readonly Mesh mesh;
    Vector3[] meshVertices;
    int[] meshTriangles;

    Vector3 closestPoint;
    float shortestDistance;
    float sqrShortestDistance;
    Vector3[] projectionOnEdge;
    float biggestRadius = 0;

    bool isInsideMesh;
    int targetLayerMask;
    RaycastHit[] hits;
    int insideCount, outsideCount;

    bool showHitPointers = false;
    #endregion
    #region Mono
    void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject hitSphere = Instantiate(hitSpherePrefab, hitSphereInitPos);
            hitSpheres.Add(hitSphere);
        }
        projectionOnEdge = new Vector3[3];
        targetLayerMask = LayerMask.GetMask(Constants.FilamentLayerName);

        sphereMaterial = GetComponent<Renderer>().material;

        isWiggling = false;

        sphereLight = GetComponent<Light>();

    }

    public void SetupTarget(GameObject filament)
    {
        //shortestDistance = 0;
        //sqrShortestDistance = 0;
        //biggestRadius = 0;
        this.filament = filament;
        ghostSphere.GetComponent<GhostSphere>().Init();
        Mesh mesh = filament.GetComponent<MeshFilter>().mesh;
        meshVertices = mesh.vertices;
        meshTriangles = mesh.triangles;
        
        transform.position = Vector3.zero;
        active = true;

        //transform.position = (meshVertices[meshTriangles[0]] + meshVertices[meshTriangles[1]]) * 0.5f;
    }

	void Update()
    {
        if(active)
        {
            if (Input.GetMouseButton(1))
            {
                //int layer = LayerMask.NameToLayer(FilamentLayerName);
                //Ray ray =  new Ray(CalculateRayPosToSphere(),  //Camera.main.ScreenPointToRay(Input.mousePosition);
                //Debug.Log(CalculateRayPosToSphere());

                //Debug.DrawRay(CalculateRayPosToSphere());
            }
            if(filament == null)
            {
                return;
            }
            isInsideMesh = IsSphereInsideMesh();
            if (isInsideMesh)
                sphereMaterial.SetColor("_WireframeColor", sphereColor[0]);
            else
                sphereMaterial.SetColor("_WireframeColor", sphereColor[1]);

            //CalculateDistanceToMesh(out shortestDistance);

            RaycastHit[] hits;
            hits = Physics.RaycastAll(CalculateRayPosToSphere(), Mathf.Infinity, LayerMask.GetMask(Constants.FilamentLayerName));

            if (hits.Length > 1)
            {
                //print("In Move");
                Vector3 minDistPos = hits[0].point;
                //print("hitLength " + hits.Length);
                Vector3 maxDistPos = Camera.main.transform.position;

                for (int i = 0; i < hits.Length; i++)
                {
                    minDistPos = Vector3.Distance(hits[i].point, Camera.main.transform.position) < Vector3.Distance(minDistPos, Camera.main.transform.position) ? hits[i].point : minDistPos;
                    maxDistPos = Vector3.Distance(hits[i].point, Camera.main.transform.position) > Vector3.Distance(maxDistPos, Camera.main.transform.position) ? hits[i].point : maxDistPos;
                }

               // DepthCalculation(out shortestDistance, minDistPos, maxDistPos);

                if (active)
                {
                    sphereDot[3].transform.position = minDistPos;
                    sphereDot[4].transform.position = maxDistPos;
                }
            }


            ScaleSphere(shortestDistance, transform);
        }
	}
    #endregion
    #region Public Methods

    public void UpdateSphereSize()
    {
        isInsideMesh = IsSphereInsideMesh();
        if (isInsideMesh)
        {
            sphereMaterial.SetColor("_WireframeColor", sphereColor[0]);
            //print("inside");
        }
        else
        {
            //print("Outside");
            sphereMaterial.SetColor("_WireframeColor", sphereColor[1]);
        }

        CalculateDistanceToMesh(transform.position, out shortestDistance);

        float radius = (isInsideMesh ? shortestDistance : Mathf.Min(shortestDistance, MaxOutsideSphereRadius));
        ScaleSphere(radius, transform);
    }

    public IEnumerator Wiggle(float amount)
    {
        isWiggling = true;

        //AudioManager.Inst.PlayWiggleSound();

        float duration = 1.2f;
        float timeElapsed = 0f;
        Vector3 startPos = transform.position;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            float damper = (duration - timeElapsed)/ duration;

            transform.position = startPos;

            transform.Translate(Random.insideUnitSphere * amount * damper);

            yield return new WaitForEndOfFrame();
        }

        isWiggling = false;

        transform.position = startPos;

        yield return null;
    }
    #endregion
    #region Private Methods
    Ray CalculateRayPosToSphere()
    {

        float distance = 100;

        Vector3 dir = Vector3.Normalize(Camera.main.transform.position - transform.position);

        Vector3 rayInitPos = transform.position - (dir * distance);

        return new Ray(rayInitPos, dir);

    }

    void DepthCalculation(out float shortestDistance, Vector3 enterBoundingBox, Vector3 exitBoundingBox) {

        Vector3 dir = (exitBoundingBox - enterBoundingBox).normalized;
        float dist = Vector3.Distance(exitBoundingBox, enterBoundingBox);
        int distanceDivision = 10;
        float biggestDist = 0;

        float distIncrement = dist / distanceDivision;


        for (int i = 0; i < distanceDivision; i++) {
            Vector3 position = enterBoundingBox + dir * distIncrement * i;
            CalculateDistanceToMesh(position, out shortestDistance);
            biggestDist = shortestDistance > biggestDist ? shortestDistance : biggestDist;
        }
        shortestDistance = biggestDist;
    }

    void CalculateDistanceToMesh(Vector3 centerPoint, out float distance)
    {
        sqrShortestDistance = Mathf.Infinity;

        float newSqrDistance;
        Vector3 nrml = Vector3.one;

        for (int i = 0; i < meshTriangles.Length; i += 3)
        {
            //filament.transform.position + filament.transform.rotation * 
            Vector3 p1 = filament.transform.TransformPoint(meshVertices[meshTriangles[i + 0]]);
            Vector3 p2 = filament.transform.TransformPoint(meshVertices[meshTriangles[i + 1]]);
            Vector3 p3 = filament.transform.TransformPoint(meshVertices[meshTriangles[i + 2]]);

            Vector3 edge1 = p1 - p2;
            Vector3 edge2 = p3 - p1;
            Vector3 edge3 = p2 - p3;


            Vector3 perp = Vector3.Cross(edge1, edge2);
            float perpLenght = perp.magnitude;
            Vector3 normal = perp / perpLenght;
            
            Vector3 pointWithinPlane = Math3D.ProjectPointOnPlane(normal, p1, centerPoint);

            nrml = normal;

            //Debug.Log(((360 - 2 * Vector3.Angle(p2 - p1, p3 - p1)) / 2) + "|" + Vector3.Angle(p3 - p1, p2 - p1) + "|" + Vector3.Angle(p1 - p2, p3 - p2) + "|" + Vector3.Angle(p2 - p3, p1 - p3));
            float angleInTriangle1 = Vector3.Angle(p3 - p1, p2 - p1);//(360 - 2 * Vector3.Angle(edge1, edge2)) / 2;
            float angleInTriangle2 = Vector3.Angle(p1 - p2, p3 - p2);//(360 - 2 * Vector3.Angle(edge3, edge1)) / 2;
            float angleInTriangle3 = Vector3.Angle(p2 - p3, p1 - p3);//(360 - 2 * Vector3.Angle(edge2, edge3)) / 2;

            float angleToPointInPlane1 = Math3D.SignedVectorAngle(edge2, pointWithinPlane - p1, normal) - 0;//180;
            angleToPointInPlane1 = (angleToPointInPlane1 < 0f ? angleToPointInPlane1 + 360f : angleToPointInPlane1);
            float angleToPointInPlane2 = Math3D.SignedVectorAngle(edge1, pointWithinPlane - p2, normal) - 0;//180;
            angleToPointInPlane2 = (angleToPointInPlane2 < 0f ? angleToPointInPlane2 + 360f : angleToPointInPlane2);
            float angleToPointInPlane3 = Math3D.SignedVectorAngle(edge3, pointWithinPlane - p3, normal) - 0;//180;
            angleToPointInPlane3 = (angleToPointInPlane3 < 0f ? angleToPointInPlane3 + 360f : angleToPointInPlane3);

            //Debug.Log("(" + angleInTriangle1 + "|" + angleToPointInPlane1 + ") (" + angleInTriangle2 + "|" +  angleToPointInPlane2 + ") (" + angleInTriangle3 + "|" + angleToPointInPlane3 + ")");
            //Debug.DrawLine(point, pointWithinPlane, Color.black, 100f);

            //Debug.DrawLine(point, p1, Color.red, 100f);
            //Debug.DrawLine(point, p2, Color.green, 100f);
            //Debug.DrawLine(point, p3, Color.blue, 100f);
            //
            //Debug.DrawLine(p1, p1 - normal, Color.red, 100f);
            //Debug.DrawLine(p2, p2 - normal, Color.green, 100f);
            //Debug.DrawLine(p3, p3 - normal, Color.blue, 100f);

            bool outOfTriangle = angleToPointInPlane1 > angleInTriangle1 || angleToPointInPlane2 > angleInTriangle2 || angleToPointInPlane3 > angleInTriangle3;
            //mat.color = !outOfTriangle ? sphereColor[0] : sphereColor[1];

            if (!outOfTriangle)
            {
                newSqrDistance = (centerPoint - pointWithinPlane).sqrMagnitude;

                if (newSqrDistance < sqrShortestDistance)
                {
                    closestPoint = pointWithinPlane;
                    sqrShortestDistance = newSqrDistance;

                    if(active)
                    {
                        sphereDot[0].transform.position = closestPoint;
                        sphereDot[1].transform.position = new Vector3(10000f, 10000f, 10000f);
                        sphereDot[2].transform.position = new Vector3(10000f, 10000f, 10000f);
                    }

                    //insideFilament = Vector3.Angle(pointWithinPlane - point, normal) > 90;
                    //if (insideFilament)
                    //{
                    //    //  DrawLine(closestPoint, normal * -1, 0);
                    //    //Debug.DrawRay((p1 + p2 + p3) / 3f, normal* -1, Color.green, 10);
                    //}
                }                
            }
            else
            {
                projectionOnEdge[0] = Math3D.ProjectPointOnLineSegment(p2, p3, pointWithinPlane);
                projectionOnEdge[1] = Math3D.ProjectPointOnLineSegment(p3, p1, pointWithinPlane);
                projectionOnEdge[2] = Math3D.ProjectPointOnLineSegment(p1, p2, pointWithinPlane);

                for (int j = 0; j < 3; j++)
                {
                    newSqrDistance = (centerPoint - (projectionOnEdge[j])).sqrMagnitude;

                    if (newSqrDistance < sqrShortestDistance)
                    {
                        closestPoint = projectionOnEdge[j];
                        sqrShortestDistance = newSqrDistance;

                        if(active)
                        {
                            sphereDot[0].transform.position = projectionOnEdge[0];
                            sphereDot[1].transform.position = projectionOnEdge[1];
                            sphereDot[2].transform.position = projectionOnEdge[2];
                        }

                        //insideFilament = Vector3.Angle(pointWithinPlane - point, normal) > 90;
                        //if (insideFilament)
                        //{
                        //    //Debug.Log("p1 " + p1 + " sphere " + transform.position);
                        //    Debug.DrawRay((p1 + p2 + p3) / 3f, normal * -1, Color.yellow, 10);
                        //    // DrawLine(closestPoint, normal * -1, 0);
                        //}
                    }
                }                
            }
        }

        distance = Mathf.Sqrt(sqrShortestDistance);
        //mat.color = insideFilament == true ? sphereColor[0] : sphereColor[1];
        
        if (shortestDistance > biggestRadius && IsInsideMesh)
        {
            biggestRadius = shortestDistance;
            ghostSphere.transform.position = transform.position;
            ScaleSphere(biggestRadius, ghostSphere.transform);
        }

#if UNITY_EDITOR
        if (active)
        {
            if (Input.GetKey(KeyCode.A))
            {
                foreach (UnityEditor.SceneView item in UnityEditor.SceneView.sceneViews)
                {
                    item.LookAtDirect(Vector3.zero, Quaternion.LookRotation(nrml));
                }
            }
        }
#endif

        if(active) DrawLine(centerPoint, closestPoint, 1);
    }

    List<MeshRenderer> triangleRenderers = new List<MeshRenderer>();
    bool IsSphereInsideMesh()
    {
        //if(active)
        //{
        //    for (int i = 0; i < triangleRenderers.Count; i++)
        //    {
        //        triangleRenderers[i].enabled = false;
        //    }
        //    triangleRenderers.Clear();
        //}

        insideCount = outsideCount = 0;
        int totalCounts = 0;
        for (int i = 0; i < outerPoints.Length; i++)
        {
            hits = Physics.RaycastAll(transform.position, outerPoints[i], OuterPointDistance, targetLayerMask);

            totalCounts += FindRayCastHits(transform.position, outerPoints[i], i);

            if (hits.Length % 2 != 0)
            {
                insideCount++;
            }
            else
            {
                outsideCount++;
            }
            

            if(active)
            {
                if (insideCount > 0 && outsideCount > 0)
                {
                    //print(insideCount + "|" + outsideCount);
                }

                for (int j = 0; j < hits.Length; j++)
                {
                    GameObject go = hits[j].collider.gameObject;


                    if (go.GetComponent<MeshFilter>() == null)
                    {
                        go.AddComponent<MeshFilter>().sharedMesh = go.GetComponent<MeshCollider>().sharedMesh;
                        go.AddComponent<MeshRenderer>();
                    }

                    triangleRenderers.Add(go.GetComponent<MeshRenderer>());
                    triangleRenderers[triangleRenderers.Count - 1].enabled = true;
                }

                Debug.DrawRay(transform.position, outerPoints[i] * OuterPointDistance, (hits.Length % 2 != 0 ? Color.green : Color.red), 0.001f);
            }
        }

        //print("TotalCounts " + totalCounts);
        //print(totalCounts % 2 != 0 ? "Inside" : "Outside");

        if(active)
        {
            if (insideCount > 0 && outsideCount > 0)
            {
                //Debug.LogError(insideCount + "|" + outsideCount + "|" + transform.position.ToString("f6"));
            }
        }
        return insideCount > outsideCount;
    }

    void DrawLine(Vector3 from, Vector3 to, int lineRenderIndex)
    {
        lineRenderer[lineRenderIndex].SetPosition(0, from);
        lineRenderer[lineRenderIndex].SetPosition(1, to);
    }

    void ScaleSphere(float radius, Transform sphereTransform)
    {
        sphereTransform.localScale = Vector3.one * radius * 2f;
        sphereLight.range = radius * 4f;
    }

    int FindRayCastHits(Vector3 origin, Vector3 direction, int outerPointCount)
    {
        RaycastHit hit;
        List<RaycastHit> raycastHits = new List<RaycastHit>();
        if(Physics.Raycast(origin, direction, out hit, Mathf.Infinity, targetLayerMask))
        raycastHits.Add(hit);

        //Debug.DrawRay(origin, direction.normalized, Color.cyan, 1);
        if (showHitPointers)
        {
            hitSpheres[outerPointCount].transform.position = hit.point;
        }

        Debug.DrawRay(hit.point + direction.normalized * 0.01f, direction, Color.blue, 1);
        //while (Physics.Raycast(hit.point + direction.normalized * 1.01f, direction, out hit, Mathf.Infinity, targetLayerMask))
        //{            
        //    raycastHits.Add(hit);
        //    print("Hiting " + raycastHits.Count);
        //    if (raycastHits.Count > 50)
        //    {
        //        print("breaking out");
        //        break;

        //    }
        //}
        return raycastHits.Count;
    }

    public void DisplayHitPointers()
    {
        showHitPointers = !showHitPointers;
        if (!showHitPointers)
        {
            for (int i = 0; i < hitSpheres.Count; i++)
            {
                hitSpheres[i].transform.position = Vector3.zero;
            }
        }
    }

    #endregion
}