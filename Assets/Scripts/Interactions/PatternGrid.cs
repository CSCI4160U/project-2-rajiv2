using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class PatternGrid : MonoBehaviour
{
    public bool isActive = false;
    private PatternBlock[,] patternBlocks;
    private int n;

    private int inputIndex;
    private int displayIndex;

    private List<PatternBlock> displayBlocks;

    private List<int> randomList;
    private System.Random rand;

    private bool showPattern;
    private bool userCanGuess;

    // current pattern
    List<PatternBlock> currentPattern;
    private int guessesAllowed;
    private int currentGuess;

    private void Start()
    {
        userCanGuess = false;
        showPattern = true;
        displayIndex = 1;
        n = transform.childCount;
        patternBlocks = new PatternBlock[n, n];
        displayBlocks = new List<PatternBlock>();
        randomList = new List<int>();
        rand = new System.Random();

        GenerateRandomSequence();

        for (int s = 0; s < n * n; s++)
        {
            // Adding to the display queue
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {

                    if (patternBlocks[i, j].sequenceNumber == s)
                    {
                        displayBlocks.Add(patternBlocks[i, j]);
                        Debug.Log("BLOCK: " + patternBlocks[i, j].name + " {ROW " + i + "} {SEQ = "+ patternBlocks[i, j].sequenceNumber + "}");
                        
                    }
                }
            }
        }
    }

    private void GenerateRandomSequence()
    {
        int sequenceNumber = rand.Next(0, n * n);
        int randomIndex = 0;

        while (randomIndex != n * n)
        {
            if (!randomList.Contains(sequenceNumber))
            {
                randomList.Add(sequenceNumber);
                randomIndex++;
            }

            sequenceNumber = rand.Next(0, n * n);

        }

        randomIndex = 0;

        // making pattern randomly
        for (int i = 0; i < n; i++)
        {
            PatternBlock[] rowBlocks = transform.GetChild(i).GetComponentsInChildren<PatternBlock>();
            for (int j = 0; j < rowBlocks.Length; j++)
            {
                patternBlocks[i, j] = rowBlocks[j];
                patternBlocks[i, j].sequenceNumber = randomList[randomIndex++];
            }
        }
    }

    private void FlashBlocksIncorrect()
    {
        for (int i = 0; i < displayBlocks.Count; i++)
        {
            StartCoroutine(displayBlocks[i].FlashBlockIncorrect());
        }
    }

    private void FlashBlocksCorrect()
    {
        for (int i = 0; i < displayBlocks.Count; i++)
        {
            StartCoroutine(displayBlocks[i].FlashBlockCorrect());
        }
    }

    IEnumerator FlashPattern(List<PatternBlock> blocks)
    {
        userCanGuess = false;

        for (int i = 0; i < blocks.Count; i++)
        {
            Debug.Log("Array: " + blocks[i] + " {SEQ = "+blocks[i].sequenceNumber + "}");

            yield return new WaitForSeconds(blocks[i].GetActiveTime());

            Debug.Log("---");

            StartCoroutine(blocks[i].FlashBlockActive());

            
        }

        currentGuess = 0;
        userCanGuess = true;
        yield return null;
    }
    
    private void ShowPattern()
    {
        currentPattern = displayBlocks.GetRange(0, displayIndex);

        if (showPattern)
        {
            StartCoroutine(FlashPattern(currentPattern));
            showPattern = false;
        }
    }

    private void CheckPatternAttempts()
    {
        if (userCanGuess)
        {

            guessesAllowed = currentPattern.Count;

            if (currentGuess < guessesAllowed)
            {
                if (currentPattern[currentGuess].wasActivated)
                {
                    Debug.Log("Correct Guess");
                    currentPattern[currentGuess].wasActivated = false;
                    currentGuess++;
                }
                else
                {
                    bool startOver = false;
                    for (int i = 0; i < displayBlocks.Count; i++)
                    {
                        // if any other one was activated
                        if (displayBlocks[i].wasActivated)
                        {
                            displayBlocks[i].wasActivated = false;
                            startOver = true;
                            break;
                        }
                    }

                    if (startOver)
                    {
                        FlashBlocksIncorrect();
                        Start();

                    }
                }
            }

            if (currentGuess == displayBlocks.Count)
            {
                // Done
                FlashBlocksCorrect();
                isActive = false;
            }
            else if (currentGuess == guessesAllowed)
            {
                displayIndex++;
                showPattern = true;
            }
        }
    }
    

    private void FixedUpdate()
    {
        if (!isActive)
        {
            return;
        }

        if(displayIndex < displayBlocks.Count && displayIndex >= 1)
        {
            ShowPattern();

            CheckPatternAttempts();
        }    

        
    }
}
