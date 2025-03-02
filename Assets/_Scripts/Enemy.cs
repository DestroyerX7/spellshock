using System.Collections;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform _target;

    [SerializeField] private float _speed = 6;
    [SerializeField] private float _attackRange = 2;
    [SerializeField] private float _attackDuration = 1;
    [SerializeField] private float _damage = 10;
    private bool _canAttack = true;

    private Rigidbody _rb;
    public AudioSource[] SFX;
    private float walkTime = 0.34f;

    [SerializeField] private Animator _animator;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _target = FindAnyObjectByType<PlayerController>()?.transform;

        GetComponent<Health>().OnDie.AddListener(AddPoint);
    }

    private void Update()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 moveDirection = _target.position - transform.position;

        if (_canAttack)
        {
            moveDirection.Normalize();

            _rb.linearVelocity = new(moveDirection.x * _speed, _rb.linearVelocity.y, moveDirection.z * _speed);
            walkTime -= Time.deltaTime;
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }

        _animator.SetBool("IsRunning", _canAttack);
        if (walkTime <= 0)
        {
            SFX[0].Play();
            walkTime = 0.34f;
        }
        float angle = Mathf.Atan2(moveDirection.z, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, -angle, 0);

        if (_canAttack && Vector3.Distance(_target.position, transform.position) <= _attackRange)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        _animator.SetTrigger("AttackOne");
        _canAttack = false;

        Collider collider = Physics.OverlapSphere(transform.position, _attackRange, LayerMask.GetMask("Player")).FirstOrDefault();

        if (collider != null && collider.TryGetComponent(out Health health))
        {
            health.TakeDamage(_damage);
        }

        yield return new WaitForSeconds(_attackDuration);

        _canAttack = true;
    }

    public void AddPoint()
    {
        GameManager.Instance.AddPoint();
    }
}
