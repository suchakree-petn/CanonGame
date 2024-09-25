using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SSVEP_Training : MonoBehaviour
{
    // For calculate equally class trials
    public int numberOfTrial = 10;
    public int numberOfClass = 2;
    private int[] randomNumbers;
    // private int currentIndex = 0;

    [FoldoutGroup("Reference")]
    [SerializeField] List<Image> images;

    [FoldoutGroup("Reference"), ReadOnly]
    [SerializeField] List<SSVEP_Flickering> flickerings;

    private void Awake()
    {



    }

    void Start()
    {
        GenerateEqualRandomNumbers();
        Sequence trainingSequence = DOTween.Sequence();

        for (int i = 0; i < randomNumbers.Length; i++)
        {
            int index = randomNumbers[i];
            SSVEP_Flickering flickering = flickerings[index];
            float trialDuration = flickering.blinkDuration + flickering.delayBeforeBlink + flickering.delayBeforeNextTrial;
            trainingSequence.AppendCallback(() => flickering.IsActive = true);
            trainingSequence.AppendInterval(trialDuration);
            trainingSequence.AppendCallback(() => flickering.IsActive = false);
        }

        trainingSequence.Play();
    }

    void Update()
    {

    }
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying) return;
        flickerings.Clear();
        foreach (var image in images)
        {
            flickerings.Add(image.GetComponent<SSVEP_Flickering>());
        }
#endif
    }

    private void GenerateEqualRandomNumbers()
    {
        randomNumbers = new int[numberOfTrial];

        // Calculate the number of times each number should appear
        int countPerNumber = numberOfTrial / numberOfClass;
        int remainder = numberOfTrial % numberOfClass;

        // Fill the array with equal numbers of 0, 1, 2, and 3
        int index = 0;
        for (int num = 0; num < numberOfClass; num++)
        {
            for (int i = 0; i < countPerNumber; i++)
            {
                randomNumbers[index++] = num;
            }
        }

        // Distribute the remainder randomly
        for (int i = 0; i < remainder; i++)
        {
            randomNumbers[index++] = Random.Range(0, numberOfClass);
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

}
