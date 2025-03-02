using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _despawnTime = 10;

    private void Start()
    {
        Destroy(gameObject, _despawnTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
