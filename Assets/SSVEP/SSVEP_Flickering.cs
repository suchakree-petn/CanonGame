using LSL;
using UnityEngine;
using UnityEngine.UI;

public class SSVEP_Flickering : MonoBehaviour
{
    public bool IsActive = false;
    public Color startColor = Color.white;
    public Color endColor = Color.black;
    public Color targetColor = Color.red;

    [Range(1, 40)]
    public int frequencyImg = 1; // Frequency in Hz

    //Image Blinking Paramemter 
    public Image blinkImg;
    private float elapsedTime = 0f;
    private bool isStartColor = true;

    //For interval calculations
    private float interval;

    //For cue
    public float blinkDuration = 5.0f;
    public float delayBeforeBlink = 1f;
    public float delayBeforeNextTrial = 2f;
    private float indexTimer = 0f;
    private float delayTimer = 0f;
    private bool isDelayActive = false;

    //For LSL Outlet
    string StreamName = "UnityEventSSVEP"; // Stream Name
    string StreamType = "Markers";
    private StreamOutlet outlet;
    private string[] sample = { "" };


    private void Awake()
    {
        blinkImg = GetComponent<Image>();
    }

    void Start()
    {
        interval = 1f / frequencyImg;


        var hash = new Hash128();
        hash.Append(StreamName);
        hash.Append(StreamType);
        hash.Append(gameObject.GetInstanceID());
        StreamInfo streamInfo = new StreamInfo(StreamName, StreamType, 1, LSL.LSL.IRREGULAR_RATE,
            channel_format_t.cf_string, hash.ToString());
        outlet = new StreamOutlet(streamInfo);

        if (outlet != null)
        {
            sample[0] = "Experiment_Begin"; //Event Name
            outlet.push_sample(sample);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsActive) return;

        if (isDelayActive)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= delayBeforeNextTrial) // Delay for blank time 
            {

                delayTimer = 0f;
                isDelayActive = false;
            }
        }

        else
        {
            indexTimer += Time.deltaTime;

            if (indexTimer < blinkDuration + delayBeforeBlink) // Delay for Blink duration 
            {
                if (indexTimer < delayBeforeBlink) // Delay for cue duration 
                {
                    blinkImg.color = targetColor;
                }
                else
                {
                    elapsedTime += Time.deltaTime; // Increment elapsed time

                    if (elapsedTime >= interval / 2f) // Check if it's time to switch colors
                    {
                        blinkImg.color = isStartColor ? endColor : startColor;// Color changing
                        isStartColor = !isStartColor; // Toggle the color flag
                        elapsedTime = 0f; // Reset the elapsed time
                    }
                }
            }

            else
            {
                indexTimer = 0f;
                blinkImg.color = startColor;
                isDelayActive = true;
            }
        }
    }
}