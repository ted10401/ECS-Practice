using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine;

public class BulletSystem : JobComponentSystem
{
    private float m_time;

    [BurstCompile]
    private struct BulletJob : IJobForEach<Translation, Bullet>
    {
        public float time;

        public void Execute(ref Translation c0, ref Bullet c1)
        {
            c0.Value += c1.direction * c1.speed * time;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        m_time = Time.deltaTime;

        var job = new BulletJob()
        {
            time = m_time,
        };

        return job.Schedule(this, inputDeps);
    }
}
