using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilamentSetup : MonoBehaviour {


    public Transform filamentParent;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh mesh;
    public Bounds FilamentBounds { get { return meshBounds; } }
    Bounds meshBounds;
    Vector3 meshOriginalPosition;
    Vector3[] meshVertices;
    int[] meshTriangles;
    Vector3 offSetPosition = new Vector3();

    public Material filemantMaterial;

    public GameObject InitializeFilament(GameObject filamentObject)
    {
        GameObject instantiatedFilament = InstantiateFilamet(filamentObject);
        if(instantiatedFilament.GetComponent<MeshFilter>() == null)
        {
            return instantiatedFilament;
        }
        meshFilter = instantiatedFilament.GetComponent<MeshFilter>();
        meshRenderer = instantiatedFilament.GetComponent<MeshRenderer>();

        mesh = meshFilter.sharedMesh;
        meshVertices = mesh.vertices;
        meshTriangles = mesh.triangles;

        //Render mesh double sided
        CreateDoubleSidedMesh(meshVertices, meshTriangles, instantiatedFilament);
        CreateFilamentColliders(meshVertices, meshTriangles, instantiatedFilament);
        MeshHelper.CalculateMeshBoundingBox(meshVertices, out meshBounds, out meshOriginalPosition);
        instantiatedFilament.transform.position = offSetPosition;
        return instantiatedFilament;
    }

    GameObject InstantiateFilamet(GameObject filament)
    {
        GameObject tempFilament = Instantiate(filament, Vector3.zero, Quaternion.identity);
        if(tempFilament.GetComponent<Renderer>() != null)
        {
            tempFilament.GetComponent<Renderer>().material = filemantMaterial;
        }

        tempFilament.transform.localPosition = Vector3.zero;
        tempFilament.transform.SetParent(filamentParent);

        return tempFilament;
    }

    void CreateDoubleSidedMesh(Vector3[] originalVertices, int[] originalTriangles, GameObject filamentObject)
    {
        int originalTriangleCount = originalTriangles.Length;

        Vector3[] frontVertices = new Vector3[originalTriangleCount];
        int[] frontTriangles = new int[originalTriangleCount];

        Vector3[] backVertices = new Vector3[originalTriangleCount];
        int[] backTriangles = new int[originalTriangleCount];

        for (int i = 0; i < originalTriangleCount; i += 3)
        {
            frontVertices[i] = originalVertices[originalTriangles[i]];
            frontVertices[i + 1] = originalVertices[originalTriangles[i + 1]];
            frontVertices[i + 2] = originalVertices[originalTriangles[i + 2]];

            frontTriangles[i] = i;
            frontTriangles[i + 1] = i + 1;
            frontTriangles[i + 2] = i + 2;

            backVertices[i] = originalVertices[originalTriangles[i]];
            backVertices[i + 1] = originalVertices[originalTriangles[i + 1]];
            backVertices[i + 2] = originalVertices[originalTriangles[i + 2]];

            backTriangles[i] = i + 2;
            backTriangles[i + 1] = i + 1;
            backTriangles[i + 2] = i;
        }

        //Set front side object
        Mesh frontMesh = new Mesh();
        frontMesh.vertices = frontVertices;
        frontMesh.triangles = frontTriangles;
        //mesh.normals = normals;
        frontMesh.RecalculateNormals();
        meshFilter.sharedMesh = frontMesh;

        //Set back side object
        GameObject go = Instantiate(filamentObject);
        //go.transform.SetParent(filamentObject.transform.parent);
        MeshFilter secondFilter = go.GetComponent<MeshFilter>();
        go.GetComponent<Renderer>().material = filemantMaterial;
        go.transform.localScale = new Vector3(1, 1, 1);
        offSetPosition = Vector3.zero - go.GetComponent<Renderer>().bounds.center;
        go.transform.position = offSetPosition;
        go.transform.SetParent(filamentParent);

        Mesh backMesh = new Mesh();
        backMesh.vertices = backVertices;
        backMesh.triangles = backTriangles;
        backMesh.RecalculateNormals();
        secondFilter.sharedMesh = backMesh;

        CreateFilamentColliders(backVertices, backTriangles, go);
    }

    void CreateFilamentColliders(Vector3[] originalVertices, int[] originalTriangles, GameObject filamentObject)
    {

        //Transform parent = new GameObject("ColliderParent").transform;
        //parent.SetParent(filamentObject.transform);
        //parent.transform.localPosition = Vector3.zero;
        //parent.transform.localScale = new Vector3(1, 1, 1);
        int layer = LayerMask.NameToLayer(Constants.FilamentLayerName);
        filamentObject.AddComponent<MeshCollider>();
        filamentObject.layer = layer;
        //for (int i = 0; i < originalTriangles.Length; i += 3)
        //{
        //    GameObject go = new GameObject((i / 3).ToString());
        //    //go.transform.SetParent(parent);
        //    go.layer = layer;
        //    //go.transform.localPosition = Vector3.zero;
        //    //go.transform.localScale = new Vector3(1, 1, 1);
        //    //MeshCollider collider = go.AddComponent<MeshCollider>();

        //    //Mesh m = new Mesh();
        //    //m.vertices = new Vector3[] { originalVertices[originalTriangles[i]], originalVertices[originalTriangles[i + 1]], originalVertices[originalTriangles[i + 2]] };
        //    //m.triangles = new int[] { 0, 1, 2, 2, 1, 0 };
        //    //collider.sharedMesh = m;
        //}
    }

}
