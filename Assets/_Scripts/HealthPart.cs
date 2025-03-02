using UnityEngine;

public class HealthPart : MonoBehaviour, IDamageable
{
    [field: SerializeField] public HealthPartType Type { get; private set; }
    [SerializeField] private Health _health;
    [SerializeField] private float _damageMultiplier;

    public void TakeDamage(float damage)
    {
        _health.TakeDamage(damage * _damageMultiplier);
    }
}

public enum HealthPartType
{
    Head,
    Body
}
