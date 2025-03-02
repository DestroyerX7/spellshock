using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private InputAction _dashAction;

    private CharacterController _characterController;
    [SerializeField] private float _walkSpeed = 5;
    [SerializeField] private float _sprintSpeed = 8;
    [SerializeField] private float _lookSensitivity = 0.25f;
    [SerializeField] private float _jumpHeight = 1;
    private float _yVelocity;
    private float _gravity;

    private float _xRotation;
    private Camera _cam;
    [SerializeField] private Transform _camPivot;

    [Range(0.001f, 0.01f)]
    [SerializeField] private float _amount = 0.005f;

    [Range(1, 30)]
    [SerializeField] private float _walkFrequency = 10;

    [Range(1, 30)]
    [SerializeField] private float _sprintFrequency = 20;
    private float _currentFrequency;

    [Range(10, 100)]
    [SerializeField] private float _smooth = 50;

    // [SerializeField] private Image _healthbar;

    private Health _health;
    private bool _canHeal = true;
    [SerializeField] private Image _bloodOverlay;

    [SerializeField] private float _dashSpeed = 30;
    private Vector3 _currentDashVelocity;
    [SerializeField] private float _dashCooldown = 2;
    private float _currentDashCooldown;

    [SerializeField] private AudioClip[] _footsteps;
    private AudioSource _audioSource;
    private float _footStepCoolDown;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
        _lookAction = _playerInput.actions["Look"];
        _jumpAction = _playerInput.actions["Jump"];
        _sprintAction = _playerInput.actions["Sprint"];
        _dashAction = _playerInput.actions["Dash"];

        _characterController = GetComponent<CharacterController>();
        _gravity = Physics2D.gravity.y;

        _cam = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;

        _health = GetComponent<Health>();
        _health.OnTakeDamage.AddListener(OnTakeDamage);
        _health.OnDie.AddListener(() => GameManager.Instance.GameOver());

        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Move();
        Look();

        if (_canHeal)
        {
            Heal();
        }

        // _healthbar.fillAmount = _health.CurrentHealth / _health.MaxHealth;

        byte alpha = (byte)(255f - 255f * _health.CurrentHealth / _health.MaxHealth);
        _bloodOverlay.color = new Color32(255, 255, 255, alpha);

        if (transform.position.y < -10)
        {
            _health.TakeDamage(500);
        }
    }

    private void Move()
    {
        bool isGrounded = _characterController.isGrounded;

        if (isGrounded && _yVelocity < 0)
        {
            _yVelocity = -0.1f;
        }

        if (_jumpAction.triggered && isGrounded)
        {
            float jumpPower = Mathf.Sqrt(_gravity * -2 * _jumpHeight);
            _yVelocity += jumpPower;
        }

        Vector2 moveInput = _moveAction.ReadValue<Vector2>();

        if (_dashAction.triggered && _currentDashCooldown <= 0)
        {
            Vector3 dashDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
            _currentDashVelocity = dashDirection * _dashSpeed;
            _currentDashCooldown = _dashCooldown;
        }

        _characterController.Move(_currentDashVelocity * Time.deltaTime);
        _currentDashVelocity = Vector3.MoveTowards(_currentDashVelocity, Vector3.zero, _dashSpeed * 2 * Time.deltaTime);
        _currentDashCooldown = Mathf.MoveTowards(_currentDashCooldown, 0, Time.deltaTime);
        GameManager.Instance.SetDashCooldownUI(1 - _currentDashCooldown / _dashCooldown);

        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        float moveSpeed;

        if (_sprintAction.inProgress && moveInput.y > 0)
        {
            moveSpeed = _sprintSpeed;
            _currentFrequency = _sprintFrequency;
        }
        else
        {
            moveSpeed = _walkSpeed;
            _currentFrequency = _walkFrequency;
        }

        _characterController.Move(moveSpeed * Time.deltaTime * moveDirection);

        if (moveInput != Vector2.zero)
        {
            CameraBob();

            if (isGrounded)
            {
                if (_footStepCoolDown <= 0)
                {
                    PlayFoosteps();
                }

                _footStepCoolDown -= Time.deltaTime;
            }
        }
        else
        {
            _cam.transform.localPosition = Vector3.MoveTowards(_cam.transform.localPosition, Vector3.zero, Time.deltaTime);
        }

        _characterController.Move(_yVelocity * Time.deltaTime * Vector2.up);
        _yVelocity += _gravity * Time.deltaTime;
    }

    private void Look()
    {
        Vector2 lookInput = _lookAction.ReadValue<Vector2>();

        _xRotation -= lookInput.y * _lookSensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -90, 90);
        _camPivot.localRotation = Quaternion.Euler(_xRotation, 0, 0);

        transform.Rotate(0, lookInput.x * _lookSensitivity, 0);
    }

    private void CameraBob()
    {
        Vector3 pos = Vector2.zero;
        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * _currentFrequency) * _amount * 1.4f, _smooth * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * _currentFrequency / 2) * _amount * 1.6f, _smooth * Time.deltaTime);
        _cam.transform.localPosition += pos;
    }

    private void OnTakeDamage(float currentHealth, float maxHealth)
    {
        _canHeal = false;
        CancelInvoke(nameof(AllowHealing));
        Invoke(nameof(AllowHealing), 5);
    }

    private void AllowHealing()
    {
        _canHeal = true;
    }

    private void Heal()
    {
        float health = Mathf.MoveTowards(_health.CurrentHealth, _health.MaxHealth, 5 * Time.deltaTime);
        _health.SetHealth(health);
    }

    private void PlayFoosteps()
    {
        AudioClip audioClip = _footsteps[Random.Range(0, _footsteps.Length)];
        _audioSource.PlayOneShot(audioClip);
        _footStepCoolDown = 5 / _currentFrequency;
    }
}
