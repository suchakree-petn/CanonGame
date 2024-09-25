using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tutor
{
    public class RandomSelector<T>
    {
        private List<T> items; // The list of items
        private List<T> tempList; // A temporary copy to draw from
        private System.Random random;

        // Constructor: Initialize the random selector with the list
        public RandomSelector(List<T> originalList)
        {
            items = new List<T>(originalList);
            tempList = new List<T>(items);
            random = new System.Random();
        }

        // Method to get a random element, ensuring each is picked at least once before reshuffling
        public T GetRandomElement()
        {
            // If the temp list is empty, refill and reshuffle the original list
            if (tempList.Count == 0)
            {
                ResetTempList();
            }

            // Randomly pick an element from the tempList
            int index = random.Next(tempList.Count);
            T selectedElement = tempList[index];

            // Remove the selected element so it can't be picked again in this round
            tempList.RemoveAt(index);

            return selectedElement;
        }

        // Reset the temp list by refilling it and shuffling the items
        private void ResetTempList()
        {
            tempList = new List<T>(items);
            Shuffle(tempList);
        }

        // Shuffle the list using Fisher-Yates algorithm
        private void Shuffle(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }

    public class RandomEnumSelector<T> where T : Enum
    {
        private List<T> enumValues; // Original list of enum values
        private List<T> tempList;    // Temporary list for drawing
        private System.Random random;

        // Constructor: Initialize with enum values
        public RandomEnumSelector()
        {
            // Get all enum values and convert them to a list
            enumValues = new List<T>((T[])Enum.GetValues(typeof(T)));
            tempList = new List<T>(enumValues);
            random = new System.Random();
        }

        // Method to get a random enum value, ensuring each is picked at least once before reshuffling
        public T GetRandomEnumValue()
        {
            // If the tempList is empty, reshuffle and refill it
            if (tempList.Count == 0)
            {
                ResetTempList();
            }

            // Randomly select an element from the tempList
            int index = random.Next(tempList.Count);
            T selectedValue = tempList[index];

            // Remove the selected value to prevent repetition within this round
            tempList.RemoveAt(index);
            return selectedValue;
        }

        // Reset and shuffle the temporary list
        private void ResetTempList()
        {
            tempList = new List<T>(enumValues);
            Shuffle(tempList);
        }

        // Shuffle using the Fisher-Yates algorithm
        private void Shuffle(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
