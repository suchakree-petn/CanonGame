using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SSVEPBlinking : MonoBehaviour
{
    public bool ShowUIOnStart = true;
    public Color startColor = Color.white;
    public Color endColor = Color.black;
    [Range(1, 40)]
    public int frequencyImg = 1; // Frequency in Hz

    //Image Blinking Paramemter 
    public Image blinkImg;
    private float elapsedTime = 0f;
    private bool isStartColor = true;
    bool startBlink = false;

    //For interval calculations
    private float interval;

    void Start()
    {
        interval = 1f / frequencyImg;
        blinkImg.gameObject.SetActive(ShowUIOnStart);
    }

    // Update is called once per frame
    void Update()
    {
        SetStart();
        if (startBlink){
            elapsedTime += Time.deltaTime; // Increment elapsed time

            if (elapsedTime >= interval / 2f) // Check if it's time to switch colors
            {

                blinkImg.color = isStartColor ? endColor : startColor;// Color changing
                isStartColor = !isStartColor; // Toggle the color flag
                elapsedTime = 0f; // Reset the elapsed time
            }
        }
    }

    void SetStart(){
        if(GameManager.Instance.IsPlayerTurn && !startBlink){
            startBlink = true;
            blinkImg.gameObject.SetActive(true);
        }
    }
}