using UnityEngine;
using System;
using System.IO;

public static class MeshHelper
{
    #region Public Methods
    public static void MeshDataToBytes(Vector3[] vertices, int[] triangles, out byte[] bytes)
    {
        //Byte format:
        //Vertex count (2 bytes)
        //-Loop for each vertex:
        //Vertex x (4 bytes)
        //Vertex y (4 bytes)
        //Vertex z (4 bytes)
        //Triangle count (2 bytes)
        //-Loop for each triangle index:
        //Triangle index (2 bytes)

        if (vertices == null || triangles == null || vertices.Length == 0 || triangles.Length == 0)
        {
            bytes = null;
            return;
        }

        int vertexCount = vertices.Length;
        int triangleCount = triangles.Length;

        int size = 2 + 2 + vertexCount * 12 + triangleCount * 2;
        bytes = new byte[size];
        int index = 0;

        CopyBytes(BitConverter.GetBytes((ushort)vertexCount), bytes, ref index);

        for (int i = 0; i < vertexCount; i++)
        {
            CopyBytes(BitConverter.GetBytes(vertices[i].x), bytes, ref index);
            CopyBytes(BitConverter.GetBytes(vertices[i].y), bytes, ref index);
            CopyBytes(BitConverter.GetBytes(vertices[i].z), bytes, ref index);
        }

        CopyBytes(BitConverter.GetBytes((ushort)triangleCount), bytes, ref index);

        for (int i = 0; i < triangleCount; i++)
        {
            CopyBytes(BitConverter.GetBytes((ushort)triangles[i]), bytes, ref index);
        }
    }

    public static void BytesToMeshData(byte[] bytes, out Vector3[] vertices, out int[] triangles)
    {
        //Byte format:
        //Vertex count (2 bytes)
        //-Loop for each vertex:
        //Vertex x (4 bytes)
        //Vertex y (4 bytes)
        //Vertex z (4 bytes)
        //Triangle count (2 bytes)
        //-Loop for each triangle index:
        //Triangle index (2 bytes)

        int index = 0;

        int length;
        GetUShort(bytes, ref index, out length);

        vertices = new Vector3[length];
        for (int i = 0; i < length; i++)
        {
            GetFloat(bytes, ref index, out vertices[i].x);
            GetFloat(bytes, ref index, out vertices[i].y);
            GetFloat(bytes, ref index, out vertices[i].z);
        }

        GetUShort(bytes, ref index, out length);

        triangles = new int[length];
        for (int i = 0; i < length; i++)
        {
            GetUShort(bytes, ref index, out triangles[i]);
        }
    }

    public static void BytesToMesh(byte[] bytes, out Mesh mesh)
    {
        Vector3[] vertices;
        int[] triangles;
        BytesToMeshData(bytes, out vertices, out triangles);
        CreateMesh(vertices, triangles, out mesh);
    }

    public static void ReadSTLFile(string inputPath, out Vector3[] vertices, out int[] triangles)
    {
        byte[] bytes = File.ReadAllBytes(inputPath);

        Debug.Log(System.Text.Encoding.UTF8.GetString(bytes, 0, 80));

        int index = 80; //Omit header data which is 80 bytes
        uint length;
        GetUInt(bytes, ref index, out length);
        int vertexIndex;

        vertices = new Vector3[length * 3];
        triangles = new int[vertices.Length];

        for (int i = 0; i < length; i++)
        {
            index += 12;
            vertexIndex = i * 3;

            GetFloat(bytes, ref index, out vertices[vertexIndex].x);
            GetFloat(bytes, ref index, out vertices[vertexIndex].y);
            GetFloat(bytes, ref index, out vertices[vertexIndex].z);

            GetFloat(bytes, ref index, out vertices[vertexIndex + 1].x);
            GetFloat(bytes, ref index, out vertices[vertexIndex + 1].y);
            GetFloat(bytes, ref index, out vertices[vertexIndex + 1].z);

            GetFloat(bytes, ref index, out vertices[vertexIndex + 2].x);
            GetFloat(bytes, ref index, out vertices[vertexIndex + 2].y);
            GetFloat(bytes, ref index, out vertices[vertexIndex + 2].z);

            triangles[vertexIndex] = vertexIndex;
            triangles[vertexIndex + 1] = vertexIndex + 1;
            triangles[vertexIndex + 2] = vertexIndex + 2;

            index += 2;
        }
    }

    public static void CreateMesh(Vector3[] vertices, int[] triangles, out Mesh mesh)
    {
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public static void InstantiateMesh(Vector3[] vertices, int[] triangles)
    {
        Mesh mesh;
        CreateMesh(vertices, triangles, out mesh);

        InstantiateMesh(mesh);
    }

    public static GameObject InstantiateMesh(Mesh mesh)
    {
        GameObject go = new GameObject();
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>();
        return go;
    }

    public static void CalculateMeshBoundingSphere(Vector3[] vertices, out BoundingSphere boundingSphere, out Vector3 originalPosition)
    {
        if (vertices == null || vertices.Length == 0)
        {
            boundingSphere = new BoundingSphere(Vector3.zero, 0f);
            originalPosition = new Vector3();
            return;
        }

        Vector3 center;
        float radius, sqrRadius;
        Vector3 min, max;
        int xMinIndex, xMaxIndex, yMinIndex, yMaxIndex, zMinIndex, zMaxIndex;
        int vertexCount = vertices.Length;

        //Find a large diameter from bounding box
        min = max = vertices[0];
        xMinIndex = xMaxIndex = yMinIndex = yMaxIndex = zMinIndex = zMaxIndex = 0;

        for (int i = 1; i < vertexCount; i++)
        {
            if (vertices[i].x < min.x)
            {
                min.x = vertices[i].x;
                xMinIndex = i;
            }
            else if (vertices[i].x > max.x)
            {
                max.x = vertices[i].x;
                xMaxIndex = i;
            }
            if (vertices[i].y < min.y)
            {
                min.y = vertices[i].y;
                yMinIndex = i;
            }
            else if (vertices[i].y > max.y)
            {
                max.y = vertices[i].y;
                yMaxIndex = i;
            }
            if (vertices[i].z < min.z)
            {
                min.z = vertices[i].z;
                zMinIndex = i;
            }
            else if (vertices[i].z > max.z)
            {
                max.z = vertices[i].z;
                zMaxIndex = i;
            }
        }

        //Save original position
        originalPosition = (min + max) * 0.5f;

        //Select the largest of the 3 axis as the initial diameter
        Vector3 xDiff = vertices[xMaxIndex] - vertices[xMinIndex];
        Vector3 yDiff = vertices[yMaxIndex] - vertices[yMinIndex];
        Vector3 zDiff = vertices[zMaxIndex] - vertices[zMinIndex];
        float xDiffSqr = xDiff.sqrMagnitude;
        float yDiffSqr = yDiff.sqrMagnitude;
        float zDiffSqr = zDiff.sqrMagnitude;

        if (xDiffSqr >= yDiffSqr && xDiffSqr >= zDiffSqr)
        {
            center = vertices[xMinIndex] + xDiff * 0.5f;
            sqrRadius = (vertices[xMaxIndex] - center).sqrMagnitude;
        }
        else if (yDiffSqr >= zDiffSqr)
        {
            center = vertices[yMinIndex] + yDiff * 0.5f;
            sqrRadius = (vertices[yMaxIndex] - center).sqrMagnitude;
        }
        else
        {
            center = vertices[zMinIndex] + zDiff * 0.5f;
            sqrRadius = (vertices[zMaxIndex] - center).sqrMagnitude;

        }
        radius = Mathf.Sqrt(sqrRadius);

        //Expand the radius to include vertices outside the sphere
        Vector3 diff;
        float dist, distSqr;
        for (int i = 0; i < vertexCount; i++)
        {
            diff = vertices[i] - center;
            distSqr = diff.sqrMagnitude;
            if (distSqr <= sqrRadius)
            {
                continue;
            }

            //Expand to include vertex
            dist = Mathf.Sqrt(distSqr);
            radius = (radius + dist) * 0.5f;
            sqrRadius = radius * radius;
            center = center + ((dist - radius) / dist) * diff;
        }

        boundingSphere = new BoundingSphere(center, radius);

        //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //sphere.transform.localPosition = Vector3.zero;
        //sphere.transform.localScale = Vector3.one * boundingSphere.radius * 2f;
    }

    public static void CalculateMeshBoundingBox(Vector3[] vertices, out Bounds boundingBox, out Vector3 originalPosition)
    {
        if (vertices == null || vertices.Length == 0)
        {
            boundingBox = new Bounds(Vector3.zero, Vector3.zero);
            originalPosition = new Vector3();
            return;
        }

        Vector3 min, max;
        int vertexCount = vertices.Length;

        min = max = vertices[0];

        for (int i = 1; i < vertexCount; i++)
        {
            if (vertices[i].x < min.x)
            {
                min.x = vertices[i].x;
            }
            else if (vertices[i].x > max.x)
            {
                max.x = vertices[i].x;
            }
            if (vertices[i].y < min.y)
            {
                min.y = vertices[i].y;
            }
            else if (vertices[i].y > max.y)
            {
                max.y = vertices[i].y;
            }
            if (vertices[i].z < min.z)
            {
                min.z = vertices[i].z;
            }
            else if (vertices[i].z > max.z)
            {
                max.z = vertices[i].z;
            }
        }

        //Save original position
        originalPosition = (min + max) * 0.5f;

        boundingBox = new Bounds(originalPosition, (max - min));
    }

    public static void CopyBytes(Array sourceArray, Array destinationArray, ref int destinationIndex)
    {
        Buffer.BlockCopy(sourceArray, 0, destinationArray, destinationIndex, sourceArray.Length);
        destinationIndex += sourceArray.Length;
    }
    #endregion
    #region Private Methods
    static void GetUInt(byte[] array, ref int index, out uint value)
    {
        value = BitConverter.ToUInt32(array, index);
        index += 4;
    }

    static void GetUShort(byte[] array, ref int index, out ushort value)
    {
        value = BitConverter.ToUInt16(array, index);
        index += 2;
    }

    static void GetUShort(byte[] array, ref int index, out int value)
    {
        value = BitConverter.ToUInt16(array, index);
        index += 2;
    }

    static void GetFloat(byte[] array, ref int index, out float value)
    {
        value = BitConverter.ToSingle(array, index);
        index += 4;
    }
    #endregion
}