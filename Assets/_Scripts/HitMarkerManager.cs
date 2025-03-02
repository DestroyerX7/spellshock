using UnityEngine;
using UnityEngine.UI;

public class HitMarkerManager : MonoBehaviour
{
    public static HitMarkerManager Instance { get; private set; }

    [SerializeField] private Image _hitMarkerImage;
    [SerializeField] private float _hitMarkerDuration = 0.5f;
    private float _currentAlpha;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        _currentAlpha = Mathf.MoveTowards(_currentAlpha, 0, 255 * Time.deltaTime / _hitMarkerDuration);
        _hitMarkerImage.color = new Color32((byte)_hitMarkerImage.color.r, (byte)_hitMarkerImage.color.g, (byte)_hitMarkerImage.color.b, (byte)_currentAlpha);
    }

    public void ShowHitMarker(bool isHeadShot = false)
    {
        _currentAlpha = 255;
        _hitMarkerImage.color = isHeadShot ? Color.red : Color.white;
    }
}
