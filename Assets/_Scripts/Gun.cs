using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    private enum FireMode
    {
        SemiAuto,
        FullAuto
    }

    private PlayerInput _playerInput;
    private InputAction _shootAction;
    private InputAction _reloadAction;

    [SerializeField] private float _damage;

    private Camera _cam;

    [SerializeField] private Rigidbody _projectilePrefab;
    [SerializeField] private float _shootSpeed = 25;
    [SerializeField] private Transform _shootPos;

    private CameraRecoil _cameraRecoil;
    [SerializeField] private Vector3 _camRecoil = new(10, 5, 0);
    [SerializeField] private float _camReturnSpeed = 100;
    [SerializeField] private float _camSnappiness = 50;

    private Vector3 _currentRotation;
    private Vector3 _targetRotation;
    [SerializeField] private Vector3 _gunRecoil = new(10, 5, 0);
    [SerializeField] private float _gunReturnSpeed = 100;
    [SerializeField] private float _gunSnappiness = 50;

    [SerializeField] private int _magSize = 8;
    private int _currentAmmoInMag;
    [SerializeField] private float _reloadTime = 1.5f;
    private bool _isReloading;

    [SerializeField] private FireMode _fireMode;
    [SerializeField] private float _fireRate;
    private bool _canShoot = true;

    private Animator _animator;
    private AudioSource _audioSoure;

    [SerializeField] private AudioClip[] _gunShots;
    [SerializeField] private AudioClip _reloadSound;

    private void Start()
    {
        _playerInput = GetComponentInParent<PlayerInput>();
        _shootAction = _playerInput.actions["Attack"];
        _reloadAction = _playerInput.actions["Reload"];

        _cam = transform.parent.GetComponentInChildren<Camera>();
        _cameraRecoil = GetComponentInParent<CameraRecoil>();

        _currentAmmoInMag = _magSize;

        _animator = GetComponent<Animator>();
        _audioSoure = GetComponent<AudioSource>();
    }

    private void Update()
    {
        _currentRotation = Vector3.MoveTowards(_currentRotation, _targetRotation, _gunSnappiness * Time.deltaTime);
        _targetRotation = Vector3.MoveTowards(_targetRotation, Vector3.zero, _gunReturnSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(_currentRotation);

        bool isShooting = (_fireMode == FireMode.FullAuto && _shootAction.inProgress) || (_fireMode == FireMode.SemiAuto && _shootAction.triggered);

        if (isShooting && _canShoot && _currentAmmoInMag > 0 && !_isReloading)
        {
            StartCoroutine(Shoot());
        }

        if (!_isReloading && (_reloadAction.triggered || (_currentAmmoInMag == 0 && isShooting)))
        {
            StartCoroutine(Reload());
        }

        GameManager.Instance.SetAmmoUI(_currentAmmoInMag + "/" + _magSize);
    }

    private IEnumerator Shoot()
    {
        AudioClip audioClip = _gunShots[Random.Range(0, _gunShots.Length)];
        _audioSoure.PlayOneShot(audioClip);
        _canShoot = false;

        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit hit) && hit.collider.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_damage);
            bool isHeadShot = hit.collider.TryGetComponent(out HealthPart healthPart);
            HitMarkerManager.Instance.ShowHitMarker(isHeadShot);
        }

        Vector3 camRecoil = new(Random.Range(0.5f * _camRecoil.x, _camRecoil.x), Random.Range(-_camRecoil.y, _camRecoil.y), Random.Range(-_camRecoil.z, _camRecoil.z));
        _cameraRecoil.Recoil(camRecoil, _camReturnSpeed, _camSnappiness);

        Vector3 gunRecoil = new(Random.Range(0.5f * _gunRecoil.x, _gunRecoil.x), Random.Range(-_gunRecoil.y, _gunRecoil.y), Random.Range(-_gunRecoil.z, _gunRecoil.z));
        _targetRotation -= gunRecoil;

        Vector3 direction = hit.collider != null ? (hit.point - _shootPos.position).normalized : _cam.transform.forward;

        Rigidbody projectile = Instantiate(_projectilePrefab, _shootPos.position, Quaternion.identity);
        projectile.linearVelocity = direction * _shootSpeed;

        _currentAmmoInMag--;

        yield return new WaitForSeconds(60 / _fireRate);

        _canShoot = true;
    }

    private IEnumerator Reload()
    {
        _audioSoure.PlayOneShot(_reloadSound);

        _isReloading = true;

        _animator.SetTrigger("Reload");

        yield return new WaitForSeconds(_reloadTime);

        _currentAmmoInMag = _magSize;
        _isReloading = false;
    }
}
