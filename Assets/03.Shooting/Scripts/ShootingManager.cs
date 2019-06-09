using UnityEngine;

public class ShootingManager : MonoBehaviour
{
    public static ShootingManager Instance
    {
        get;
        private set;
    }

    public Transform shootPosition;
    public Material material;
    public Mesh mesh;
    public float bulletSpeed = 100f;
    public float shootPerSeconds = 10f;
    public float shootAngle;
    public int shootCount = 1;

    private void Awake()
    {
        Instance = this;
    }
}
