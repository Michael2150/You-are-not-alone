using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private bool isVisibleOnStart = true;
    
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _modeButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private TMP_InputField _edtSeed;
    [SerializeField] private Button _btnLastSeed;
    [SerializeField] private TMP_Text _txtLastSeed;
    
    [SerializeField] private long _seed;
    [SerializeField] private long _lastSeed;
    
    [SerializeField] private GamemodeScript _gamemodeScript;

    public long lastSeed
    {
        get => _lastSeed;
        set
        {
            _lastSeed = value;
            _txtLastSeed.text = "Last Seed: " + _lastSeed.ToString();
        }
    }
    public long seed
    {
        get =>  long.Parse(_edtSeed.text);
        set
        {
            _seed = value;
            _edtSeed.text = _seed.ToString();
        }
    }

    private void Start()
    {
        _playButton.onClick.AddListener(Play);
        _modeButton.onClick.AddListener(Mode);
        _exitButton.onClick.AddListener(Exit);
        _btnLastSeed.onClick.AddListener(copySeed);
        
        long randomSeed = (long)Random.Range(1000000000000000, 9999999999999999);
        seed = randomSeed;
        lastSeed = randomSeed;
        
        setVisible(gameObject, isVisibleOnStart);
    }

    private void Play()
    {
        //Play the debug scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Playground");
    }
    
    private void Mode()
    {
        setVisible(gameObject, false);
        _gamemodeScript.setVisible(true);
    }
    
    private void copySeed()
    {
        GUIUtility.systemCopyBuffer = lastSeed.ToString();
    }
    
    private void Exit()
    {
        //Close the game
        Application.Quit();
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
}