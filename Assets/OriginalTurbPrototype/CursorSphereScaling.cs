using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSphereScaling : MonoBehaviour
{
    #region Variables
    const float MaxOutsideSphereRadius = 0.05f;
    const float OuterPointDistance = 100f;
    static readonly Vector3[] outerPoints = new Vector3[6] { Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };

    public GameObject filament;
    public LineRenderer[] lineRenderer;


    

    public GameObject[] sphereDot;
    public GameObject sphereDotPrefab;
  //  public GameObject ghostSphere;
    public Color[] sphereColor;
    Material sphereMaterial;

    public bool IsActive { get { return isActive; } }

    public Transform Transform { get { return transform; } }

    public float Radius { get { return shortestDistance; } }

    public bool IsInsideMesh { get { return isInsideMesh; } }

    public bool isWiggling { get; private set; }

    new Transform transform;

    bool isActive;

    Light sphereLight;

    Mesh mesh;
    Vector3[] meshVertices;
    int[] meshTriangles;

    Vector3 closestPoint;
    float shortestDistance;
    float sqrShortestDistance;
    Vector3[] projectionOnEdge;

    bool isInsideMesh;
    int targetLayerMask;
    RaycastHit[] hits;
    int insideCount, outsideCount;
    #endregion
    #region Mono
    void Awake()
    {
        transform = GetComponent<Transform>();

        projectionOnEdge = new Vector3[3];
        targetLayerMask = LayerMask.GetMask(Constants.FilamentLayerName);

        sphereMaterial = GetComponent<Renderer>().material;

        SetTarget(filament.transform, filament.GetComponent<MeshFilter>().mesh);
        transform.position = (meshVertices[meshTriangles[0]] + meshVertices[meshTriangles[1]]) * 0.5f;
        SetActive(true);
       
        isWiggling = false;

        sphereLight = GetComponent<Light>();

    }

    void Update()
    {

            if (isActive)
            {
                isInsideMesh = IsSphereInsideMesh();
                if (isInsideMesh)
                    sphereMaterial.SetColor("_WireframeColor", sphereColor[0]);
                else
                    sphereMaterial.SetColor("_WireframeColor", sphereColor[1]);



                RaycastHit[] hits;
                hits = Physics.RaycastAll(CalculateRayPosToSphere(), Mathf.Infinity, LayerMask.GetMask(Constants.FilamentLayerName));

                if (hits.Length > 1)
                {

                    Vector3 minDistPos = hits[0].point;
                    Vector3 maxDistPos = Camera.main.transform.position;

                    for (int i = 0; i < hits.Length; i++)
                    {
                        minDistPos = Vector3.Distance(hits[i].point, Camera.main.transform.position) < Vector3.Distance(minDistPos, Camera.main.transform.position) ? hits[i].point : minDistPos;
                        maxDistPos = Vector3.Distance(hits[i].point, Camera.main.transform.position) > Vector3.Distance(maxDistPos, Camera.main.transform.position) ? hits[i].point : maxDistPos;
                    }

                    DepthCalculation(out shortestDistance, minDistPos, maxDistPos);
                    sphereDot[3].transform.position = minDistPos;
                    sphereDot[4].transform.position = maxDistPos;
                    
                }


                ScaleSphere(shortestDistance, transform);
            }
        
    }
    #endregion
    #region Public Methods
    public void SetTarget(Transform target, Mesh mesh)
    {
        if (target != null && mesh != null)
        {
            filament = target.gameObject;
            this.mesh = mesh;
            meshVertices = this.mesh.vertices;
            meshTriangles = this.mesh.triangles;
                Transform parent = new GameObject("ColliderParent").transform;
                int layer = LayerMask.NameToLayer(Constants.FilamentLayerName);

                for (int i = 0; i < meshTriangles.Length; i += 3)
                {
                    GameObject go = new GameObject((i / 3).ToString());
                    go.transform.SetParent(parent);
                   // go.layer = layer;
                    MeshCollider collider = go.AddComponent<MeshCollider>();

                    Mesh m = new Mesh();
                    m.vertices = new Vector3[] { meshVertices[meshTriangles[i]], meshVertices[meshTriangles[i + 1]], meshVertices[meshTriangles[i + 2]] };
                    m.triangles = new int[] { 0, 1, 2, 2, 1, 0 };
                    collider.sharedMesh = m;
                }
            
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    public void UpdateSphereSize()
    {
        if (isActive)
        {
            isInsideMesh = IsSphereInsideMesh();
            if (isInsideMesh)
                sphereMaterial.SetColor("_WireframeColor", sphereColor[0] * 1.2f);
            else
                sphereMaterial.SetColor("_WireframeColor", sphereColor[1] * 1.2f);

            CalculateDistanceToMesh(transform.position, out shortestDistance);

            float radius = (isInsideMesh ? shortestDistance : Mathf.Min(shortestDistance, MaxOutsideSphereRadius));
            ScaleSphere(radius, transform);
        }
    }

    public IEnumerator Wiggle(float amount)
    {
        isWiggling = true;

        float duration = 1.2f;
        float timeElapsed = 0f;
        Vector3 startPos = transform.position;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            float damper = (duration - timeElapsed) / duration;

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

    void DepthCalculation(out float shortestDistance, Vector3 enterBoundingBox, Vector3 exitBoundingBox)
    {

        Vector3 dir = (exitBoundingBox - enterBoundingBox).normalized;
        float dist = Vector3.Distance(exitBoundingBox, enterBoundingBox);
        int distanceDivision = 10;
        float biggestDist = 0;

        float distIncrement = dist / distanceDivision;


        for (int i = 0; i < distanceDivision; i++)
        {
            Vector3 position = enterBoundingBox + dir * distIncrement * i;
            CalculateDistanceToMesh(position, out shortestDistance);
            biggestDist = shortestDistance > biggestDist ? shortestDistance : biggestDist;
        }
        shortestDistance = biggestDist;

        // Get normal of camera
        // Get enterpoint of BoundingBox
        // Get exitPoint of boudningBox
        // Normalize distance and divide into positions   
    }

    void CalculateDistanceToMesh(Vector3 centerPoint, out float distance)
    {
        sqrShortestDistance = Mathf.Infinity;

        float newSqrDistance;
        Vector3 nrml = Vector3.one;

        for (int i = 0; i < meshTriangles.Length; i += 3)
        {
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

            float angleInTriangle1 = Vector3.Angle(p3 - p1, p2 - p1);
            float angleInTriangle2 = Vector3.Angle(p1 - p2, p3 - p2);
            float angleInTriangle3 = Vector3.Angle(p2 - p3, p1 - p3);

            float angleToPointInPlane1 = Math3D.SignedVectorAngle(edge2, pointWithinPlane - p1, normal) - 0;//180;
            angleToPointInPlane1 = (angleToPointInPlane1 < 0f ? angleToPointInPlane1 + 360f : angleToPointInPlane1);
            float angleToPointInPlane2 = Math3D.SignedVectorAngle(edge1, pointWithinPlane - p2, normal) - 0;//180;
            angleToPointInPlane2 = (angleToPointInPlane2 < 0f ? angleToPointInPlane2 + 360f : angleToPointInPlane2);
            float angleToPointInPlane3 = Math3D.SignedVectorAngle(edge3, pointWithinPlane - p3, normal) - 0;//180;
            angleToPointInPlane3 = (angleToPointInPlane3 < 0f ? angleToPointInPlane3 + 360f : angleToPointInPlane3);

            bool outOfTriangle = angleToPointInPlane1 > angleInTriangle1 || angleToPointInPlane2 > angleInTriangle2 || angleToPointInPlane3 > angleInTriangle3;

            if (!outOfTriangle)
            {
                newSqrDistance = (centerPoint - pointWithinPlane).sqrMagnitude;

                if (newSqrDistance < sqrShortestDistance)
                {
                    closestPoint = pointWithinPlane;
                    sqrShortestDistance = newSqrDistance;
                    sphereDot[0].transform.position = closestPoint;
                    sphereDot[1].transform.position = new Vector3(10000f, 10000f, 10000f);
                    sphereDot[2].transform.position = new Vector3(10000f, 10000f, 10000f);

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


                        sphereDot[0].transform.position = projectionOnEdge[0];
                        sphereDot[1].transform.position = projectionOnEdge[1];
                        sphereDot[2].transform.position = projectionOnEdge[2];

                    }
                }
            }
        }

        distance = Mathf.Sqrt(sqrShortestDistance);
    }

    List<MeshRenderer> triangleRenderers = new List<MeshRenderer>();

    bool IsSphereInsideMesh()
    {
        for (int i = 0; i < triangleRenderers.Count; i++)
            {
                triangleRenderers[i].enabled = false;
            }
        triangleRenderers.Clear();
        insideCount = outsideCount = 0;
        for (int i = 0; i < 6; i++)
        {
            hits = Physics.RaycastAll(transform.position, outerPoints[i], OuterPointDistance, targetLayerMask);

            if (hits.Length % 2 != 0)
            {
                insideCount++;
            }
            else
            {
                outsideCount++;
            }            
                if (insideCount > 0 && outsideCount > 0)
                {
                    Debug.LogWarning(i + ": " + hits.Length);
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
        if (insideCount > 0 && outsideCount > 0)
        {
            Debug.LogError(insideCount + "|" + outsideCount + "|" + transform.position.ToString("f6"));
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


    #endregion
}