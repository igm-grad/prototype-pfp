using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseManager : MonoBehaviour {

    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;
    public Button quitButton;

    public CanvasGroup panel;

    public bool IsPaused { get; private set; }
    float targetAlpha;

    public void Awake()
    {
        resumeButton.onClick.AddListener(OnResumeButtonClick);
        restartButton.onClick.AddListener(OnRestartButtonClick);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);
    }

    private void OnQuitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnMainMenuButtonClick()
    {
        Pause();
        Application.LoadLevel("MainMenu");
    }

    private void OnRestartButtonClick()
    {
        Pause();
        Application.LoadLevel(Application.loadedLevel);
    }

    private void OnResumeButtonClick()
    {
        Pause();
    }

    public void Pause()
    {
        IsPaused = !IsPaused;
        panel.interactable = IsPaused;
        Time.timeScale = IsPaused ? 0 : 1;
        targetAlpha = IsPaused ? 1 : 0;
    }

    private void Update()
    {
        panel.alpha = Mathf.Lerp(panel.alpha, targetAlpha, 0.25f);
        if (Input.GetKeyUp(KeyCode.Escape)) 
        {
            Pause();
        }
    }
	
}
