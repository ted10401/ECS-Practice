using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine;
using Unity.Rendering;
using Unity.Collections;

public class ShootingSystem : ComponentSystem
{
    private EntityManager m_entityManager;
    private EntityArchetype m_entityArchetype;
    private ShootingManager m_shootManager;
    private float m_timer;

    protected override void OnCreate()
    {
        m_entityManager = World.Active.EntityManager;
        m_entityArchetype = m_entityManager.CreateArchetype(typeof(Translation), typeof(Rotation), typeof(RenderMesh), typeof(LocalToWorld), typeof(Bullet));
    }

    protected override void OnUpdate()
    {
        if(m_shootManager == null)
        {
            m_shootManager = ShootingManager.Instance;
        }

        if(m_shootManager == null)
        {
            return;
        }

        m_timer -= Time.deltaTime;
        if(m_timer <= 0)
        {
            m_timer = 1f / m_shootManager.shootPerSeconds;

            if (Input.GetKey(KeyCode.Space))
            {
                NativeArray<Entity> entities = new NativeArray<Entity>(m_shootManager.shootCount * m_shootManager.shootCount, Allocator.Temp);
                m_entityManager.CreateEntity(m_entityArchetype, entities);
                for(int i = 0; i < entities.Length; i++)
                {
                    m_entityManager.SetComponentData(entities[i], new Translation { Value = m_shootManager.shootPosition.position });
                    m_entityManager.SetComponentData(entities[i], new Rotation { Value = Quaternion.Euler(m_shootManager.shootPosition.forward + Vector3.forward * -90) });
                    m_entityManager.SetSharedComponentData(entities[i], new RenderMesh { mesh = m_shootManager.mesh, material = m_shootManager.material });
                    m_entityManager.SetComponentData(entities[i], new Bullet { direction = Quaternion.Euler(0, i % m_shootManager.shootCount * m_shootManager.shootAngle, i / m_shootManager.shootCount * m_shootManager.shootAngle) * m_shootManager.shootPosition.forward, speed = 50f });
                }
            }
        }
    }
}
