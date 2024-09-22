using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, MeshSettings meshSettings, int levelOfDetail)
    {
        int meshVertexIncrementalValue = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;

        int borderSize = heightMap.GetLength(0);
        int meshSize = borderSize - 2 * meshVertexIncrementalValue;
        int meshSizeNormal = borderSize - 2;

        float topLeftX = (meshSizeNormal - 1) / -2f;
        float topLeftZ = (meshSizeNormal - 1) / 2f;

        int verticesPerLine = (meshSize - 1) / meshVertexIncrementalValue + 1;

        MeshData meshData = new MeshData(verticesPerLine);

        int[,] vertexIndexMap = new int[borderSize, borderSize];
        int borderVerticeIndex = -1;
        int meshVerticeIndex = 0;

        for (int y = 0; y < borderSize; y += meshVertexIncrementalValue)
            for (int x = 0; x < borderSize; x += meshVertexIncrementalValue)
            {
                bool isBorderVertex = x == 0 || x == borderSize - 1 || y == 0 || y == borderSize - 1;
                
                if (isBorderVertex)
                {
                    vertexIndexMap[x, y] = borderVerticeIndex;
                    borderVerticeIndex--;
                }
                else
                {
                    vertexIndexMap[x, y] = meshVerticeIndex;
                    meshVerticeIndex++;
                }
            }

        for (int y = 0; y < borderSize; y += meshVertexIncrementalValue)
            for (int x = 0; x < borderSize; x += meshVertexIncrementalValue)
            {
                int vertexIndex = vertexIndexMap[x, y];
                Vector2 percent = new Vector2((x - meshVertexIncrementalValue) / (float)meshSize, (y - meshVertexIncrementalValue) / (float)meshSize);
                Vector3 vertexPosition = new Vector3((topLeftX + percent.x * meshSizeNormal) * meshSettings.scale, heightMap[x, y], (topLeftZ - percent.y * meshSizeNormal) * meshSettings.scale);

                meshData.AddVertex(vertexIndex, vertexPosition, percent);

                if (x <  borderSize - 1 && y < borderSize - 1)
                {
                    int a = vertexIndexMap[x, y];
                    int b = vertexIndexMap[x + meshVertexIncrementalValue, y];
                    int c = vertexIndexMap[x, y + meshVertexIncrementalValue];
                    int d = vertexIndexMap[x + meshVertexIncrementalValue, y + meshVertexIncrementalValue];
                    meshData.AddTriangle(a, d, c);
                    meshData.AddTriangle(d, a, b);
                }

            }

        meshData.CalculateNormal();

        return meshData;
    }
}

public class MeshData
{
    Vector3[] vertices;
    public Vector3[] normals;
    Vector2[] uvs;
    int[] triangles;
    int triangleIndex = 0;

    Vector3[] borderedVertices;
    int[] borderedTriangle;
    int borderedTriangleIndex = 0;

    public MeshData(int verticePerLine)
    {
        vertices = new Vector3[verticePerLine * verticePerLine];
        triangles = new int[(verticePerLine - 1) * (verticePerLine - 1) * 6];
        uvs = new Vector2[verticePerLine * verticePerLine];

        borderedVertices = new Vector3[4 * verticePerLine + 4];
        borderedTriangle = new int[verticePerLine * 24];
    }

    public void AddVertex(int vertexIndex, Vector3 vertexPosition, Vector2 uv)
    {
        if (vertexIndex < 0)
            borderedVertices[-vertexIndex - 1] = vertexPosition;
        else
        {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = uv;
        }

        if (vertexIndex == 0 || vertexIndex == vertices.Length - 1)
        {
            //Debug.Log(vertices[vertexIndex]);
        }
    }

    public void AddTriangle(int a, int b, int c)
    {
        if (a >= 0 && b >= 0 && c >= 0)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }
        else
        {
            borderedTriangle[borderedTriangleIndex] = a;
            borderedTriangle[borderedTriangleIndex + 1] = b;
            borderedTriangle[borderedTriangleIndex + 2] = c;
            borderedTriangleIndex += 3;
        }
    }

    public void CalculateNormal()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int numberOfTriangle = triangles.Length / 3;

        for (int i = 0; i < numberOfTriangle; i++)
        {
            int verticeIndexA = triangles[i * 3];
            int verticeIndexB = triangles[i * 3 + 1];
            int verticeIndexC = triangles[i * 3 + 2];

            Vector3 triangleNormal = NormalFromIndexes(verticeIndexA, verticeIndexB, verticeIndexC);
            vertexNormals[verticeIndexA] += triangleNormal;
            vertexNormals[verticeIndexB] += triangleNormal;
            vertexNormals[verticeIndexC] += triangleNormal;
        }

        int numberOfBorderedTriangle = borderedTriangle.Length / 3;

        for (int i = 0; i < numberOfBorderedTriangle; i++)
        {
            int verticeIndexA = borderedTriangle[i * 3];
            int verticeIndexB = borderedTriangle[i * 3 + 1];
            int verticeIndexC = borderedTriangle[i * 3 + 2];

            Vector3 triangleNormal = NormalFromIndexes(verticeIndexA, verticeIndexB, verticeIndexC);

            if (verticeIndexA >= 0)
                vertexNormals[verticeIndexA] += triangleNormal;
            if (verticeIndexB >= 0)
                vertexNormals[verticeIndexB] += triangleNormal;
            if (verticeIndexC >= 0)
                vertexNormals[verticeIndexC] += triangleNormal;
        }

        for (int i = 0; i < vertexNormals.Length; i++)
            vertexNormals[i] = vertexNormals[i].normalized;

        normals = vertexNormals;
    }

    public Vector3 NormalFromIndexes(int verticeIndexA, int verticeIndexB, int verticeIndexC) 
    {
        Vector3 pointA = verticeIndexA < 0 ? borderedVertices[-verticeIndexA - 1] : vertices[verticeIndexA];
        Vector3 pointB = verticeIndexB < 0 ? borderedVertices[-verticeIndexB - 1] : vertices[verticeIndexB];
        Vector3 pointC = verticeIndexC < 0 ? borderedVertices[-verticeIndexC - 1] : vertices[verticeIndexC];

        Vector3 vectorAB = pointB - pointA;
        Vector3 vectorAC = pointC - pointA;

        return Vector3.Cross(vectorAB, vectorAC).normalized;

    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = normals;
        //mesh.normals = CalculateNormal();
        return mesh;
    }
}
