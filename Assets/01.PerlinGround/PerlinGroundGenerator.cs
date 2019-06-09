using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Collections;

public class PerlinGroundGenerator : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    public float waveScale = 0.1f;
    public float waveSpeed = 0.25f;
    public float waveHeight = 20f;
    public int range = 300;

    private void Start()
    {
        EntityManager entityManager = World.Active.EntityManager;
        EntityArchetype entityArchetype = entityManager.CreateArchetype(typeof(Translation), typeof(RenderMesh), typeof(LocalToWorld), typeof(PerlinGround));
        NativeArray<Entity> entities = new NativeArray<Entity>(range * range, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entities);

        int count = -1;
        for(int x = 0; x < range; x++)
        {
            for(int z = 0; z < range; z++)
            {
                count++;
                entityManager.SetComponentData(entities[count], new Translation { Value = new Unity.Mathematics.float3(x - range / 2, 0, z - range / 2) });
                entityManager.SetSharedComponentData(entities[count], new RenderMesh { mesh = mesh, material = material });
                entityManager.SetComponentData(entities[count], new PerlinGround { waveScale = waveScale, waveSpeed = waveSpeed, waveHeight = waveHeight });
            }
        }
    }
}
