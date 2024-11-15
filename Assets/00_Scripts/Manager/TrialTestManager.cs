using System.Collections;
using System.Collections.Generic;
//using System.IO.Ports; // Config is into Edit>ProjectSettings>Player>ApiCompatibilityLevel to .NET framework
using UnityEngine;
using UnityEngine.UI;
using LSL;
using TMPro;

public class TrialTestManager : MonoBehaviour
{
    public Color startColor = Color.white;
    public Color endColor = Color.black;
    public Color targetColor = Color.red;

    [Range(1, 40)]
    public int frequencyDown = 1; // Frequency in Hz

    [Range(1, 40)]
    public int frequencyLeft = 1; 

    [Range(1, 40)]
    public int frequencyUp = 1;

    [Range(1, 40)]
    public int frequencyRight = 1;

    public Image blinkImgDown;
    public Image blinkImgLeft;
    public Image blinkImgUp;
    public Image blinkImgRight;

    public TMP_Text delayText; // Text to display during the delay

    public float delayBeforeExperiment = 5f;
    public float blinkDuration = 5.0f;
    public float delayBeforeBlink = 1f;
    public float delayBeforeNextTrial = 2f;
    public int numberOfTrial = 4;

    //For image blinking
    private float elapsedTime = 0f;
    private bool isStartColor = true;
    private bool SSVEPTrigger = true;

    //For interval calculations
    private float intervalDown;
    private float intervalLeft;
    private float intervalUp;
    private float intervalRight;

    //For delay before start
    private bool isBlinkingActive = false; // To check if blinking should start

    // For calculate equally class trials
    private int[] randomNumbers;
    private int currentIndex = 0;
    private float indexTimer = 0f;

    //For main timer control
    private float delayTimer = 0f;
    private bool isDelayActive = false;

    //For LSL
    string StreamName = "UnityEvent";
    string StreamType = "Markers";
    private StreamOutlet outlet;
    private string[] sample = { "" };

    //private SerialPort serialPort;

    private bool startTest = false;

    // Start is called before the first frame update
    void Start()
    {
        var hash = new Hash128();
        hash.Append(StreamName);
        hash.Append(StreamType);
        hash.Append(gameObject.GetInstanceID());
        StreamInfo streamInfo = new StreamInfo(StreamName, StreamType, 1, LSL.LSL.IRREGULAR_RATE,
            channel_format_t.cf_string, hash.ToString());
        outlet = new StreamOutlet(streamInfo);

        //serialPort = new SerialPort("COM5", 115200);

        //try
        //{
        //    serialPort.Open();
        //    Debug.Log("Serial port opened successfully.");
        //}
        //catch (System.Exception e)
        //{
        //    Debug.LogError("Error opening serial port: " + e.Message);
        //}

        //// Initialize the array with the desired number of random numbers
        GenerateEqualRandomNumbers();

        intervalDown = 1f / frequencyDown; // Calculate the interval based on frequency
        intervalLeft = 1f / frequencyLeft; 
        intervalUp = 1f / frequencyUp;
        intervalRight = 1f / frequencyRight;
        
    }

    // Update is called once per frame
    void Update()  
    {
        
        if(GameManager.Instance){
            if(!startTest){
                SetStartTestTime();
                return;
            }
        }else{
            delayText.gameObject.SetActive(true);
            delayText.text = $"Starting in {Mathf.Ceil(delayBeforeExperiment)} seconds...";
        }
        
        
        if (!isBlinkingActive)
        {
            // During the delay period
            delayBeforeExperiment -= Time.deltaTime; // Decrement the delay timer
            delayText.text = $"Starting in {Mathf.Ceil(delayBeforeExperiment)} seconds...";

            if (delayBeforeExperiment <= 0f)
            {
                // Start blinking after the delay
                isBlinkingActive = true;
                delayText.enabled = false; // Hide the delay text
                if (outlet != null)
                {
                    //SendSerrialPortTrigger(1);
                    sample[0] = "Trial_Begin";
                    outlet.push_sample(sample);
                }
            }
        }

        else
        {
            if (currentIndex >= randomNumbers.Length)
            {
                // Indicate completion
                //if (serialPort != null && serialPort.IsOpen)
                //{
                //    serialPort.Close();
                //}

                return;
            }

            if (isDelayActive)
            {
                // During delay before moving to next index
                delayTimer += Time.deltaTime;
                if (delayTimer >= delayBeforeNextTrial)
                {
                    currentIndex++;
                    // Reset the timer and move to the next index after the delay
                    if (currentIndex < randomNumbers.Length)
                    {
                        //SendSerrialPortTrigger(1);
                        sample[0] = "Trial_Begin";
                        outlet.push_sample(sample);
                    }
                    else
                    {
                        //SendSerrialPortTrigger(40);
                        sample[0] = "End_Experiment";
                        outlet.push_sample(sample);
                    }

                    delayTimer = 0f;
                    isDelayActive = false;
                }
            }

            else
            {
                // Increment the index timer
                indexTimer += Time.deltaTime;

                if (indexTimer < blinkDuration + delayBeforeBlink)
                {
                    int currentRandomNumber = randomNumbers[currentIndex];

                    // Check for the current random number
                    if (currentRandomNumber == 0)
                    {
                        if (indexTimer < delayBeforeBlink)
                        {
                            blinkImgLeft.color = targetColor;
                        }
                        else
                        {
                            HandleBlinking(blinkImgLeft, intervalLeft, "SSVEPLeft", 2);
                        }
                    }
                    else if (currentRandomNumber == 1)
                    {
                        if (indexTimer < delayBeforeBlink)
                        {
                            blinkImgRight.color = targetColor;
                        }
                        else
                        {
                            HandleBlinking(blinkImgRight, intervalRight, "SSVEPRight", 4);
                        }
                    }
                    else if (currentRandomNumber == 2)
                    {
                        if (indexTimer < delayBeforeBlink)
                        {
                            blinkImgUp.color = targetColor;
                        }
                        else
                        {
                            HandleBlinking(blinkImgUp, intervalUp, "SSVEPUp", 8);
                        }
                    }
                    else
                    {
                        if (indexTimer < delayBeforeBlink)
                        {
                            blinkImgDown.color = targetColor;
                        }
                        else
                        {
                            HandleBlinking(blinkImgDown, intervalDown, "SSVEPDown", 10);
                        }
                    }
                }

                else
                {
                    // Reset the timer and move to the next index after the duration
                    //SendSerrialPortTrigger(20);
                    sample[0] = "End_of_trial";
                    outlet.push_sample(sample);
                    indexTimer = 0f;
                    blinkImgLeft.color = startColor;
                    blinkImgRight.color = startColor;
                    blinkImgUp.color = startColor;
                    blinkImgDown.color = startColor;
                    isDelayActive = true;
                    SSVEPTrigger = true;
                }
            }
        }

    }

    private void SetStartTestTime(){
        if(GameManager.Instance.IsPlayerTurn && !startTest){
            startTest = true;
            delayText.gameObject.SetActive(true);
            delayText.text = $"Starting in {Mathf.Ceil(delayBeforeExperiment)} seconds...";
        }
    }

    private void GenerateEqualRandomNumbers()
    {
        randomNumbers = new int[numberOfTrial];

        // Calculate the number of times each number should appear
        int countPerNumber = numberOfTrial / 4;
        int remainder = numberOfTrial % 4;

        // Fill the array with equal numbers of 0, 1, 2, and 3
        int index = 0;
        for (int num = 0; num < 4; num++)
        {
            for (int i = 0; i < countPerNumber; i++)
            {
                randomNumbers[index++] = num;
            }
        }

        // Distribute the remainder randomly
        for (int i = 0; i < remainder; i++)
        {
            randomNumbers[index++] = Random.Range(0, 4);
        }

        // Shuffle the array to randomize the order
        ShuffleArray(randomNumbers);
    }

    private void ShuffleArray(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    private void HandleBlinking(Image Img_direct, float interval, string direction, int nowCond)
    {
        if (SSVEPTrigger)
        {
            //SendSerrialPortTrigger(nowCond);
            sample[0] = direction;
            outlet.push_sample(sample);
        }

        elapsedTime += Time.deltaTime; // Increment elapsed time


        if (elapsedTime >= interval / 2f) // Check if it's time to switch colors
        {
            //Debug.Log(elapsedTime);
            //Debug.Log(interval / 2f);
            Img_direct.color = isStartColor ? endColor : startColor;// Toggle the color
            isStartColor = !isStartColor; // Toggle the color flag
            elapsedTime = 0f; // Reset the elapsed time
        }

        SSVEPTrigger = false;
    }

    //private void SendSerrialPortTrigger(int nowCond)
    //{
    //    if (serialPort.IsOpen)
    //    {
    //        byte[] triggerBytes = new byte[] { (byte)nowCond };
    //        serialPort.Write(triggerBytes, 0, 1);
    //    }
    //}
}
