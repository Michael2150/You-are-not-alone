using System;
using System.Collections;
using System.Collections.Generic;
using GameGlobals;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SplashScreenScript : MonoBehaviour
{
    public bool debug = false;
    private float timer = 0f;
    private float new_char_timer = 0f;
    private float fade_timer = 0f;
    private int text_index = 0;
    private int char_index = 0;
    public List<string> splashTexts = new List<string>();
    public List<float> splashTimes = new List<float>();
    public List<float> newCharSpeeds = new List<float>();
    public List<float> textSizes = new List<float>();
    public TMP_Text text;
    public Image image;
    public float fadeTime = 2f;

    //Start is called before the first frame update
    private void Start()
    {
        text.text = "";
        Cursor.lockState = CursorLockMode.None;
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (text_index >= splashTexts.Count || debug || !GameManager.Instance.isFirstTime)
        {
            //Start fading out the image
            fade_timer += Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1 - (fade_timer / fadeTime));
            if (fade_timer >= fadeTime)
                gameObject.SetActive(false);
        }
        else
        {
            //Update the timer
            timer += Time.deltaTime;
            new_char_timer += Time.deltaTime;
        
            //Check if we need to add a new character or move to the next text
            if (text_index >= 0 && text_index < splashTimes.Count && timer > splashTimes[text_index])
            {
                text_index++;
                timer = 0f;
                new_char_timer = 0f;
                char_index = 0;
                text.text = "";
            }

            //Add a new character if needed
            if (!(text_index >= 0 && text_index < newCharSpeeds.Count && new_char_timer > newCharSpeeds[text_index])) return;
            new_char_timer = 0f;
            char_index++;
            if (char_index >= 0 && char_index <= splashTexts[text_index].Length)
                text.text = splashTexts[text_index][..char_index];   
            
            //Update the text size
            if (text_index >= 0 && text_index < textSizes.Count)
                text.fontSize = textSizes[text_index];
        }
        
        GameManager.Instance.isFirstTime = false;
    }
}
