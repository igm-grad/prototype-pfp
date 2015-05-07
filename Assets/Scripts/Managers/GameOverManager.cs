using UnityEngine;
using UnityEngine.UI;

namespace CompleteProject
{
    public class GameOverManager : MonoBehaviour
    {
        public PlayerHealth playerHealth;       // Reference to the player's health.
        Animator anim;                          // Reference to the animator component.
        public Button restartButton;
        public Button mainMenuButton;
        public Button quitButton;
        public CanvasGroup panel;
        public CanvasGroup panelp;
        public Text text;
        float targetAlpha;

        void Awake ()
        {
            // Set up the reference.
            anim = GetComponent <Animator> ();
            restartButton.onClick.AddListener(OnRestartButtonClick);
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClick);
            quitButton.onClick.AddListener(OnQuitButtonClick);
        }

        private void OnQuitButtonClick()
        {
            Application.Quit();
        }

        private void OnMainMenuButtonClick()
        {
            Application.LoadLevel("MainMenu");
        }

        private void OnRestartButtonClick()
        {
            Application.LoadLevel(Application.loadedLevel);
        }


        void Update ()
        {
            panel.alpha = Mathf.Lerp(panel.alpha, targetAlpha, 0.25f);
            // If the player has run out of health...
            if(GameManager.Instance.CheckGameOver())
            {
                panelp.gameObject.SetActive(false);
                targetAlpha = 1;
                panel.interactable = true;

                if(GameManager.Instance.gameLost)
                {
                    text.text = "Game Over!!!";
                }

                if(GameManager.Instance.gameWon)
                {
                    text.text = "Congrats!!!";
                }
                // ... tell the animator the game is over.
                anim.SetTrigger ("GameOver");
            }
        }


    }
}