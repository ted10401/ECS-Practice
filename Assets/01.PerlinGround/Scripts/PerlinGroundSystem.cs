using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;

public class PerlinGroundSystem : JobComponentSystem
{
    private float m_time;

    [BurstCompile]
    private struct PerlinGroundJob : IJobForEach<Translation, PerlinGround>
    {
        public float time;

        public void Execute(ref Translation c0, ref PerlinGround c1)
        {
            c0.Value.y = Mathf.PerlinNoise(c0.Value.x * c1.waveScale + time * c1.waveSpeed, c0.Value.z * c1.waveScale + time * c1.waveSpeed) * c1.waveHeight;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        m_time = Time.timeSinceLevelLoad;

        var job = new PerlinGroundJob()
        {
            time = m_time
        };

        return job.Schedule(this, inputDeps);
    }
}
