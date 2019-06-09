using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;

public class PerlinSphereGenerator : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    [Range(2, 1000)]
    public int resolution = 2;
    public float radius = 100f;
    public float waveScale = 0.1f;
    public float waveSpeed = 0.25f;
    public float waveHeight = 20;

    private void Start()
    {
        EntityManager entityManager = World.Active.EntityManager;
        EntityArchetype entityArchetype = entityManager.CreateArchetype(typeof(Translation), typeof(Rotation), typeof(RenderMesh), typeof(LocalToWorld), typeof(PerlinSphere));
        NativeArray<Entity> entities = new NativeArray<Entity>(6 * (resolution - 1) * (resolution - 1), Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entities);

        int count = -1;
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        for (int i = 0; i < directions.Length; i++)
        {
            TerrainFace terrainFace = new TerrainFace(resolution, directions[i], radius);
            foreach(Vector3 vector in terrainFace.centers)
            {
                count++;
                entityManager.SetComponentData(entities[count], new Translation { Value = vector });
                entityManager.SetComponentData(entities[count], new Rotation { Value = Quaternion.LookRotation(vector) });
                entityManager.SetSharedComponentData(entities[count], new RenderMesh { mesh = mesh, material = material });
                entityManager.SetComponentData(entities[count], new PerlinSphere { waveScale = waveScale, waveSpeed = waveSpeed, waveHeight = waveHeight, center = vector, normal = vector.normalized });
            }
        }
    }
}


public class TerrainFace
{
    int resolution;
    Vector3 localUp;
    float radius;
    Vector3 axisA;
    Vector3 axisB;
    public List<Vector3> centers = new List<Vector3>();

    public TerrainFace(int resolution, Vector3 localUp, float radius)
    {
        this.resolution = resolution;
        this.localUp = localUp;
        this.radius = radius;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);

        ConstructMesh();
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized * radius;
                vertices[i] = pointOnUnitSphere;

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }

        for(int i = 0; i < triangles.Length; i += 6)
        {
            centers.Add((vertices[triangles[i]] + vertices[triangles[i + 1]]) / 2);
        }
    }
}
