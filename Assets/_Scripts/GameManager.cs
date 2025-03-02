using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int _score;
    private static int _highScore;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _highScoreText;

    [SerializeField] private GameObject _deathUI;

    public UnityEvent OnGameOver;

    [SerializeField] private TextMeshProUGUI _ammoUI;

    public int CurrentRound { get; private set; } = 1;
    private int _killsThisRound = 0;

    [SerializeField] private GameObject _gunOne;
    [SerializeField] private GameObject _gunTwo;

    public UnityEvent OnRoundComplete;
    public UnityEvent OnNextRound;

    [SerializeField] private TextMeshProUGUI _roundText;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
    }

    public void AddPoint()
    {
        _score++;
        _scoreText.text = "Score : " + _score;

        _killsThisRound++;

        if (_killsThisRound >= CurrentRound * 2)
        {
            CompleteRound();
        }
    }

    private void CompleteRound()
    {
        _roundText.gameObject.SetActive(true);
        _roundText.text = "Round " + (CurrentRound + 1);
        OnRoundComplete?.Invoke();
        Invoke(nameof(NextRound), 2);
    }

    private void NextRound()
    {
        _roundText.gameObject.SetActive(false);
        _killsThisRound = 0;
        CurrentRound++;
        OnNextRound?.Invoke();

        if (CurrentRound == 10)
        {
            _gunOne.SetActive(false);
            _gunTwo.SetActive(true);
        }
    }

    public void GameOver()
    {
        if (_score > _highScore)
        {
            _highScore = _score;
        }

        _highScoreText.text = "High Score : " + _highScore;
        _deathUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        OnGameOver?.Invoke();
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Game");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SetAmmoUI(string text)
    {
        _ammoUI.text = text;
    }

    public void SetDashCooldownUI(float fillAmount)
    {

    }
}
