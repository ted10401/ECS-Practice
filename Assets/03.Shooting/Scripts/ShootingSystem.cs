using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine;
using Unity.Rendering;

public class ShootingSystem : ComponentSystem
{
    private EntityManager m_entityManager;
    private EntityArchetype m_entityArchetype;
    private float m_timer;

    protected override void OnCreate()
    {
        m_entityManager = World.Active.EntityManager;
        m_entityArchetype = m_entityManager.CreateArchetype(typeof(Translation), typeof(Rotation), typeof(RenderMesh), typeof(LocalToWorld), typeof(Bullet));
    }

    protected override void OnUpdate()
    {
        m_timer -= Time.deltaTime;
        if(m_timer <= 0)
        {
            m_timer = 1f / ShootingManager.Instance.shootPerSeconds;

            if (Input.GetKey(KeyCode.Space))
            {
                Entity entity = m_entityManager.CreateEntity(m_entityArchetype);
                m_entityManager.SetComponentData(entity, new Translation { Value = ShootingManager.Instance.shootPosition.position });
                m_entityManager.SetComponentData(entity, new Rotation { Value = Quaternion.Euler(ShootingManager.Instance.shootPosition.forward + Vector3.forward * -90) });
                m_entityManager.SetComponentData(entity, new Bullet { direction = ShootingManager.Instance.shootPosition.forward, speed = 50f });
                m_entityManager.SetSharedComponentData(entity, new RenderMesh { mesh = ShootingManager.Instance.mesh, material = ShootingManager.Instance.material });
            }
        }
    }
}
