using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float MaxHealth { get; private set; } = 100;
    public float CurrentHealth { get; private set; }

    public UnityEvent<float, float> OnTakeDamage;
    public UnityEvent OnDie;

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (CurrentHealth <= 0)
        {
            return;
        }

        CurrentHealth -= damage;
        OnTakeDamage?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
        {
            OnDie?.Invoke();
        }
    }

    public void SetHealth(float health)
    {
        CurrentHealth = health;
    }

    public void Despawn(float time)
    {
        Destroy(gameObject, time);
    }
}

public interface IDamageable
{
    public void TakeDamage(float damage);
}
