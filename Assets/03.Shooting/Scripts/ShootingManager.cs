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

    private void Awake()
    {
        Instance = this;
    }
}
