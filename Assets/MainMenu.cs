using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public Button start;
    public Button options;
    public Button credits;
    public Button quit;
    public void Awake()
    {
        start.onClick.AddListener(StartGame);
        options.onClick.AddListener(Options);
        credits.onClick.AddListener(Credits);
        quit.onClick.AddListener(QuitGame);
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void Credits()
    {
        throw new System.NotImplementedException();
    }

    private void Options()
    {
        throw new System.NotImplementedException();
    }

    private void StartGame()
    {
        Application.LoadLevel("Newest");
    }
}
