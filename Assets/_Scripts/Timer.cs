using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private TextMeshProUGUI _timeText;
    private bool _active = true;
    private float _currentTime;

    private void Start()
    {
        _timeText = GetComponent<TextMeshProUGUI>();
        GameManager.Instance.OnGameOver.AddListener(() => _active = false);
    }

    private void Update()
    {
        if (!_active)
        {
            return;
        }

        _currentTime += Time.deltaTime;

        int minutes = (int)_currentTime / 60;

        int seconds = (int)_currentTime % 60;

        int milliseconds = (int)(_currentTime * 100 % 100);

        _timeText.text = minutes.ToString("00") + " : " + seconds.ToString("00") + " : " + milliseconds.ToString("00");
    }
}
