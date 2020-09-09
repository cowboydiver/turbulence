using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilamentSetupLegacy : MonoBehaviour {

    GameObject filamentObject;
    readonly Transform filamentParent;

    Vector3 meshOriginalPosition;
    Bounds meshBounds;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh mesh;
    Vector3[] meshVertices;
    int[] meshTriangles;


    #region Private Methods
    bool SetupFilament(string bundleName)
    {

        //filamentObject = FilamentManager.Inst.InstantiateFilament(bundleName);

        // filamentObject should be read from the list of vertices

        if (filamentObject != null)
        {
            filamentObject.transform.SetParent(filamentParent);

            meshFilter = filamentObject.GetComponent<MeshFilter>();
            meshRenderer = filamentObject.GetComponent<MeshRenderer>();

            mesh = meshFilter.sharedMesh;
            meshVertices = mesh.vertices;
            meshTriangles = mesh.triangles;

            //Render mesh double sided
            CreateDoubleSidedMesh(meshVertices, meshTriangles);

            MeshHelper.CalculateMeshBoundingBox(meshVertices, out meshBounds, out meshOriginalPosition);

            CreateFilamentColliders(meshVertices, meshTriangles);

            return true;
        }
        return false;
    }

    void CreateDoubleSidedMesh(Vector3[] originalVertices, int[] originalTriangles)
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

        //Normals
        //Vector3[] normals = new Vector3[vertices.Length];
        //for (int i = 0; i < triangles.Length; i += 3)
        //{
        //    Vector3 side1 = vertices[triangles[i + 1]] - vertices[triangles[i]];
        //    Vector3 side2 = vertices[triangles[i + 2]] - vertices[triangles[i]];
        //    Vector3 perp = Vector3.Cross(side1, side2);
        //    float perpLength = perp.magnitude;
        //    perp /= perpLength;
        //
        //    normals[triangles[i]] = normals[triangles[i + 1]] = normals[triangles[i + 2]] = perp;
        //    Vector3 center = (vertices[triangles[i]] + vertices[triangles[i + 1]] + vertices[triangles[i + 2]]) / 3f;
        //    Debug.DrawRay(center, perp * 0.05f, (i < originalTriangles.Length ? Color.cyan : Color.green), 1000f);
        //

        //Set front side object
        Mesh frontMesh = new Mesh();
        frontMesh.vertices = frontVertices;
        frontMesh.triangles = frontTriangles;
        //mesh.normals = normals;
        frontMesh.RecalculateNormals();
        meshFilter.sharedMesh = frontMesh;

        //Set back side object
        GameObject go = Instantiate(filamentObject);
        go.transform.SetParent(filamentObject.transform.parent);
        MeshFilter secondFilter = go.GetComponent<MeshFilter>();

        Mesh backMesh = new Mesh();
        backMesh.vertices = backVertices;
        backMesh.triangles = backTriangles;
        backMesh.RecalculateNormals();
        secondFilter.sharedMesh = backMesh;
    }

    void CreateFilamentColliders(Vector3[] originalVertices, int[] originalTriangles)
    {
        Transform parent = new GameObject("ColliderParent").transform;
        parent.SetParent(filamentParent);

        //int layer = LayerMask.NameToLayer(Constants.FilamentLayerName);

        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            GameObject go = new GameObject((i / 3).ToString());
            go.transform.SetParent(parent);
            //go.layer = layer;
            MeshCollider collider = go.AddComponent<MeshCollider>();

            Mesh m = new Mesh();
            m.vertices = new Vector3[] { originalVertices[originalTriangles[i]], originalVertices[originalTriangles[i + 1]], originalVertices[originalTriangles[i + 2]] };
            m.triangles = new int[] { 0, 1, 2, 2, 1, 0 };
            collider.sharedMesh = m;
        }
    }

    #endregion
}
