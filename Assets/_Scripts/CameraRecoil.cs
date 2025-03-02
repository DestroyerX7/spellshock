using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    private Vector3 _currentRotation;
    private Vector3 _targetRotation;
    private float _returnSpeed;
    private float _snappiness;

    private void Update()
    {
        _currentRotation = Vector3.MoveTowards(_currentRotation, _targetRotation, _snappiness * Time.deltaTime);
        _targetRotation = Vector3.MoveTowards(_targetRotation, Vector3.zero, _returnSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(_currentRotation);
    }

    public void Recoil(Vector3 recoil, float returnSpeed, float snappiness)
    {
        _targetRotation -= recoil;
        _returnSpeed = returnSpeed;
        _snappiness = snappiness;
    }
}
