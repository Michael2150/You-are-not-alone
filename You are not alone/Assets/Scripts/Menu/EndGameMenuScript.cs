using System;
using GameGlobals;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class EndGameMenuScript : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text fastestTimeText;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private void Start()
        {
            //Get the mouse back
            Cursor.lockState = CursorLockMode.None;
            
            newGameButton.onClick.AddListener(() => { GameManager.Instance.SessionData.StartSession(); });
            restartButton.onClick.AddListener(() => { GameManager.Instance.SessionData.StartSession(GameManager.Instance.SessionData.Seed); });
            mainMenuButton.onClick.AddListener(() => { SceneManager.LoadScene("MainMenu"); });

            // Set the text
            var sesh = GameManager.Instance.SessionData;
            titleText.text = sesh.DidWin ? "You Win!" : "You Lose!";
            timeText.text = $"Time: {sesh.SessionTime:0.00}s";
        }
        
        private void Update()
        {
            if (Cursor.lockState != CursorLockMode.None)
                Cursor.lockState = CursorLockMode.None;
        }
    }
}
