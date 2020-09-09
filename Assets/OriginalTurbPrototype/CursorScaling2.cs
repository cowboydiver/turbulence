using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorScaling2 : MonoBehaviour {

    //public GameObject filament;
    public Color[] sphereColor;
    Material sphereMaterial;
    bool isInsideMesh;

    Mesh mesh;
    Vector3[] meshVertices;
    int[] meshTriangles;
    List<MeshRenderer> triangleRenderers = new List<MeshRenderer>();
    RaycastHit[] hits;
    static readonly Vector3[] outerPoints = new Vector3[6] { Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
    const float OuterPointDistance = 100f;
    int targetLayerMask;
    int insideCount, outsideCount;

    void Awake() {
        sphereMaterial = GetComponent<Renderer>().material;
        targetLayerMask = LayerMask.NameToLayer("FilamentLayer");
    }
    void Update() {

        isInsideMesh = IsSphereInsideMesh();
        if (isInsideMesh)
        {
            //print("YES");
            sphereMaterial.SetColor("_WireframeColor", sphereColor[0]);
        }
        else {
            //print("NO");
            sphereMaterial.SetColor("_WireframeColor", sphereColor[1]);
        }
    }

    bool IsSphereInsideMesh()
    {
        //Resetting stuff
        for (int i = 0; i < triangleRenderers.Count; i++)
        {
            triangleRenderers[i].enabled = false;
        }
        triangleRenderers.Clear();
        insideCount = outsideCount = 0;

        //send our rays in 6 directions.
        for (int i = 0; i < 6; i++)
        {
            hits = Physics.RaycastAll(transform.position, outerPoints[i], OuterPointDistance, targetLayerMask);
            if (hits.Length % 2 != 0)
            {
                insideCount++;
                //print("inside");
            }
            else
            {
                outsideCount++;
                //print("outside");
            }
            if (insideCount > 0 && outsideCount > 0)
            {
                Debug.Log(i + ": " + hits.Length);
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
            //Debug.LogError(insideCount + "|" + outsideCount + "|" + transform.position.ToString("f6"));
        }


        return insideCount > outsideCount;
    }

}