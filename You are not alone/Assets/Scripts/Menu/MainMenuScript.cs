using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _modeButton;
    [SerializeField] private Button _exitButton;

    private void Start()
    {
        _playButton.onClick.AddListener(Play);
        _modeButton.onClick.AddListener(Mode);
        _exitButton.onClick.AddListener(Exit);
    }
    
    private void Play()
    {
        Debug.Log("Play");
    }
    
    private void Mode()
    {
        Debug.Log("Mode");
    }
    
    private void Exit()
    {
        //Close the game
        Application.Quit();
    }
}