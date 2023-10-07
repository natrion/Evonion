using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] private Vector2 Seed;
    public int widthSegments = 10;
    [SerializeField] private int lengthSegments = 10;
    public int detail = 1;
    [SerializeField] private Material material;
    [SerializeField] private int perlinNoiseLayers;
    [SerializeField] private float perlinNoiseIntensity;
    [SerializeField] private float perlinDestortion;
    private float MaxDistance;
    public float[,] hights;
    public Vector3 CalculateHight(float x,float plusHight ,float z)
    {
        int xDown = Mathf.FloorToInt((x + widthSegments / detail / 2)/detail);
        int zDown = Mathf.FloorToInt((z + lengthSegments / detail / 2)/ detail);
        if (hights.GetLength(0) < xDown+2| hights.GetLength(1) < zDown+2| 0 > xDown-1 | 0 > zDown-1  ) return new Vector3(x, 10+plusHight, z);
        print(hights.GetLength(1));
        float Hight = (hights[xDown, zDown] + hights[xDown + 1, zDown] + hights[xDown, zDown + 1] + hights[xDown + 1, zDown + 1]) / 4;
        if (Hight< 10  ) return new Vector3(x, 10 + plusHight, z);

        return new Vector3(x, Hight+ plusHight, z);
    }
    private void Start()
    {
        Seed = new Vector2(Random.Range(0, 1000), Random.Range(0, 1000));
        MaxDistance = Vector2.Distance(Vector2.zero, new Vector2(widthSegments / 2 / detail, lengthSegments / 2 / detail));

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
        hights = new float[widthSegments+1, lengthSegments+1];

        for (int z = 0, i = 0; z <= lengthSegments; z++)
        {
            for (int x = 0; x <= widthSegments; x++, i++)
            {
                float xPos = x /detail;
                float zPos = z /detail;

                vertices[i] = CalculetadPerlinNoise(xPos, zPos);
                hights[x, z] = vertices[i].y;
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
        gameObject.transform.position = new Vector3(0 - widthSegments / detail / 2, 0, 0 - lengthSegments / detail / 2);
        
    }
    Vector3 CalculetadPerlinNoise(float x, float y )
    {
        float perlinNoise = 1;
        for (int i = 1; i < perlinNoiseLayers; i++)
        {
            perlinNoise *= 1+Mathf.PerlinNoise(x * i* perlinDestortion + Seed.x * i, y *i*perlinDestortion + Seed.y*i)/i ;
        }
        float IslandCurve = 1-Vector2.Distance(new Vector2(x, y), new Vector2(widthSegments / 2/detail, lengthSegments / 2 / detail))/ MaxDistance;
        return new Vector3(x, perlinNoise * perlinNoiseIntensity* IslandCurve, y);
    }
}
