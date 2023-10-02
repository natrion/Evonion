using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] private Vector2 Seed;
    [SerializeField] private int widthSegments = 10;
    [SerializeField] private int lengthSegments = 10;
    [SerializeField] private float detail = 1;
    [SerializeField] private Material material;
    [SerializeField] private int perlinNoiseLayers;
    [SerializeField] private float perlinNoiseIntensity;
    [SerializeField] private float perlinDestortion;
    private void Start()
    {
        GeneratePlane();
    }

    void GeneratePlane()
    {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;

        Mesh mesh = new Mesh();

        int numVertices = (widthSegments + 1) * (lengthSegments + 1);
        int numTriangles = widthSegments * lengthSegments * 6;

        Vector3[] vertices = new Vector3[numVertices];
        Vector2[] uv = new Vector2[numVertices];
        int[] triangles = new int[numTriangles];

        for (int z = 0, i = 0; z <= lengthSegments; z++)
        {
            for (int x = 0; x <= widthSegments; x++, i++)
            {
                float xPos = x /detail;
                float zPos = z /detail;

                vertices[i] = CalculetadPerlinNoise(xPos, zPos);

                // Calculate UV coordinates based on position
                uv[i] = new Vector2((float)x*0.1f , (float)z * 0.1f);
            }
        }

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < lengthSegments; z++)
        {
            for (int x = 0; x < widthSegments; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + widthSegments + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + widthSegments + 1;
                triangles[tris + 5] = vert + widthSegments + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        mesh.vertices = vertices;
        mesh.uv = uv; // Assign UV coordinates
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        //mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshFilter.mesh = mesh;
        gameObject.AddComponent<MeshCollider>();
        
    }
    Vector3 CalculetadPerlinNoise(float x, float y )
    {
        float perlinNoise = 1;
        for (int i = 1; i < perlinNoiseLayers; i++)
        {
            perlinNoise *= 1+Mathf.PerlinNoise(x * i* perlinDestortion+Seed.x*i, y * i*perlinDestortion + Seed.y*i)/i ;
        }
        return new Vector3(x, perlinNoise * perlinNoiseIntensity, y);
    }
}
