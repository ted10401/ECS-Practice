using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine;

public class PerlinSphereSystem : JobComponentSystem
{
    private float m_time;

    [BurstCompile]
    private struct DynamicSphereJob : IJobForEach<Translation, PerlinSphere>
    {
        public float time;

        public void Execute(ref Translation c0, ref PerlinSphere c1)
        {
            c0.Value = c1.center + c1.normal * Perlin3D(c1.center, c1.waveScale, c1.waveSpeed) * c1.waveHeight;
        }

        public float Perlin3D(float3 vector3, float waveScale, float waveSpeed)
        {
            float AB = Mathf.PerlinNoise(vector3.x * waveScale + time * waveSpeed, vector3.y * waveScale + time * waveSpeed);
            float BC = Mathf.PerlinNoise(vector3.y * waveScale + time * waveSpeed, vector3.z * waveScale + time * waveSpeed);
            float AC = Mathf.PerlinNoise(vector3.x * waveScale + time * waveSpeed, vector3.z * waveScale + time * waveSpeed);

            float BA = Mathf.PerlinNoise(vector3.y * waveScale + time * waveSpeed, vector3.x * waveScale + time * waveSpeed);
            float CB = Mathf.PerlinNoise(vector3.z * waveScale + time * waveSpeed, vector3.y * waveScale + time * waveSpeed);
            float CA = Mathf.PerlinNoise(vector3.z * waveScale + time * waveSpeed, vector3.x * waveScale + time * waveSpeed);

            return (AB + BC + AC + BA + CB + CA) / 6;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        m_time = Time.timeSinceLevelLoad;

        var job = new DynamicSphereJob()
        {
            time = m_time,
        };

        return job.Schedule(this, inputDeps);
    }
}
