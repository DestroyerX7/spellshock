using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Play();
        }
    }

    public void Play()
    {
        SceneManager.LoadScene("Game");
    }
}
