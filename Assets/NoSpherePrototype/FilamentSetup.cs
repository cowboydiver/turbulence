using System.Collections.Generic;
using UnityEngine;

namespace NoSpherePrototype
{
    public class FilamentSetup : MonoBehaviour
    {
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Mesh mesh;
        public Bounds FilamentBounds { get { return meshBounds; } }
        private Bounds meshBounds;
        private Vector3 meshOriginalPosition;
        private Vector3[] meshVertices;
        private int[] meshTriangles;
        private Vector3 offSetPosition = new Vector3();

        public Material filemantMaterial;

        public List<GameObject> InitializeFilament(Filament filamentObject)
        {
            List<GameObject> instantiatedFilaments = new List<GameObject>();
            GameObject instantiatedFilament = InstantiateFilamet(filamentObject.transform.GetChild(0).gameObject);
            instantiatedFilament.AddComponent<Filament>().biggestPossibleSphereSize = filamentObject.biggestPossibleSphereSize;

            if (instantiatedFilament.GetComponent<MeshFilter>() == null)
            {
                instantiatedFilaments.Add(instantiatedFilament);
                return instantiatedFilaments;
            }
            meshFilter = instantiatedFilament.GetComponent<MeshFilter>();
            meshRenderer = instantiatedFilament.GetComponent<MeshRenderer>();

            mesh = meshFilter.sharedMesh;
            meshVertices = mesh.vertices;
            meshTriangles = mesh.triangles;

            //Render mesh double sided
            instantiatedFilaments.Add(CreateDoubleSidedMesh(meshVertices, meshTriangles, instantiatedFilament));
            CreateFilamentColliders(meshVertices, meshTriangles, instantiatedFilament);
            MeshHelper.CalculateMeshBoundingBox(meshVertices, out meshBounds, out meshOriginalPosition);
            instantiatedFilament.transform.position = offSetPosition;
            instantiatedFilaments.Add(instantiatedFilament);

            return instantiatedFilaments;
        }

        private GameObject InstantiateFilamet(GameObject filament)
        {
            GameObject tempFilament = Instantiate(filament, Vector3.zero, Quaternion.identity);
            if (tempFilament.GetComponent<Renderer>() != null)
            {
                tempFilament.GetComponent<Renderer>().material = filemantMaterial;
            }

            tempFilament.transform.localPosition = Vector3.zero;
            //tempFilament.transform.SetParent(transform);

            return tempFilament;
        }

        private GameObject CreateDoubleSidedMesh(Vector3[] originalVertices, int[] originalTriangles, GameObject filamentObject)
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
            Mesh frontMesh = new Mesh
            {
                vertices = frontVertices,
                triangles = frontTriangles
            };
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
            go.transform.SetParent(transform);

            Mesh backMesh = new Mesh();
            backMesh.vertices = backVertices;
            backMesh.triangles = backTriangles;
            backMesh.RecalculateNormals();
            secondFilter.sharedMesh = backMesh;

            CreateFilamentColliders(backVertices, backTriangles, go);

            return go;
        }

        private void CreateFilamentColliders(Vector3[] originalVertices, int[] originalTriangles, GameObject filamentObject)
        {
            int layer = LayerMask.NameToLayer(Constants.FilamentLayerName);
            filamentObject.AddComponent<MeshCollider>();
            filamentObject.layer = layer;
        }
    }
}