using Unity.Entities;
using UnityEngine;

public struct PerlinSphere : IComponentData
{
    public float waveScale;
    public float waveSpeed;
    public float waveHeight;
    public Vector3 center;
    public Vector3 normal;
}
