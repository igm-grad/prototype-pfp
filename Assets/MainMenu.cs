using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public Button start;
    public Button options;
    public Button credits;
    public Button quit;

    public CanvasGroup CreditsPanel;
    public CanvasGroup OptionsPanel;
    public CanvasGroup MainMenuPanel;

    private float mainScreenTargetAlpha = 1.0f;
    private float creditsTargetAlpha = 0.0f;
    private float optionsTargetAlpha = 0.0f;

    private bool isCredits = false;
    private bool isOptions = false;
    private bool skip = false;


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
        skip = true;
        isCredits = true;
        mainScreenTargetAlpha = 0.0f;
        creditsTargetAlpha = 1.0f;
        MainMenuPanel.interactable = false;
        CreditsPanel.interactable = true;
    }

    private void Options()
    {
        skip = true;
        isOptions = true;
        mainScreenTargetAlpha = 0.0f;
        optionsTargetAlpha = 1.0f;
        MainMenuPanel.interactable = false;
        OptionsPanel.interactable = true;
    }

    private void Update()
    {
        MainMenuPanel.alpha = Mathf.Lerp(MainMenuPanel.alpha, mainScreenTargetAlpha, 0.25f);
        CreditsPanel.alpha = Mathf.Lerp(CreditsPanel.alpha, creditsTargetAlpha, 0.25f);
        OptionsPanel.alpha = Mathf.Lerp(OptionsPanel.alpha, optionsTargetAlpha, 0.25f);

        if (!skip && Input.GetMouseButtonUp(0) && isCredits)
        {
            isCredits = false;
            mainScreenTargetAlpha = 1.0f;
            creditsTargetAlpha = 0.0f;
            MainMenuPanel.interactable = true;
            CreditsPanel.interactable = false;

        }

        if (!skip && Input.GetMouseButtonUp(0) && isOptions)
        {
            isOptions = false;
            mainScreenTargetAlpha = 1.0f;
            optionsTargetAlpha = 0.0f;
            MainMenuPanel.interactable = true;
            OptionsPanel.interactable = false;
        }

        if (Input.GetMouseButtonUp(0) && skip)
        {
            skip = false;
        }

    }
    private void StartGame()
    {
        Application.LoadLevel("Newest");
    }

}
