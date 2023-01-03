using System;
using System.Collections;
using System.Collections.Generic;
using GameGlobals;
using GameGlobals.GameManager_Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamemodeScript : MonoBehaviour
{
    [SerializeField] private bool isVisibleOnStart = false;
    [SerializeField] private List<Button> buttons;
    [SerializeField] private List<String> buttonKeys;
    [SerializeField] private MainMenuScript mainMenuScript;

    private void Start()
    {
        setVisible(isVisibleOnStart);

        foreach (var button in buttons)
        {
            button.onClick.AddListener(() =>
            {
                var pickedGamemode = buttonKeys[buttons.IndexOf(button)];
                Debug.Log(pickedGamemode);
                setVisible(false);
                mainMenuScript.setVisible(true);
            });
        }
    }

    public void setVisible(bool visible)
    {
        setVisible(gameObject, visible);
    }

    private void setVisible(GameObject go, bool visible)
    {
        foreach (Transform child in go.transform)
        {
            setVisible(child.gameObject, visible);
        }
        
        if (go.GetComponent<TMP_Text>() != null)
        {
            go.GetComponent<TMP_Text>().enabled = visible;
        }
        if (go.GetComponent<Image>() != null)
        {
            go.GetComponent<Image>().enabled = visible;
        }
        if (go.GetComponent<Button>() != null)
        {
            go.GetComponent<Button>().enabled = visible;
        }
    }

    private void changeGameModeClicked(Difficulty difficulty)
    {
        GameManager.Instance.SessionData._difficulty = difficulty;
    }
    
    public void easyClicked()
    {
        changeGameModeClicked(Difficulty.Easy);
    }
    
    public void mediumClicked()
    {
        changeGameModeClicked(Difficulty.Medium);
    }
    
    public void hardClicked()
    {
        changeGameModeClicked(Difficulty.Hard);
    }
    
}
