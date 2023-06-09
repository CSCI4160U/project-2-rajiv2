using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

public class PatternGrid : MonoBehaviour
{
    public bool isActive = false;
    public bool startWithFullPattern = false;
    public bool isComplete = false;
    [SerializeField] private Canvas hintCanvas = null;
    private TextMeshProUGUI hintText;
    private PatternBlock[,] patternBlocks;
    private int n;

    private int displayIndex;

    private List<PatternBlock> displayBlocks;
    //private System.Random rand;

    private bool showPattern;
    private bool userCanGuess;

    // current pattern
    List<PatternBlock> currentPattern;
    private int guessesAllowed;
    private int currentGuess;

    private void Awake()
    {
        if (hintCanvas != null)
        {
            hintText = hintCanvas.GetComponentInChildren<TextMeshProUGUI>();
            hintCanvas.gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        userCanGuess = false;
        showPattern = true;
        
        n = transform.childCount;

        // all blocks according to each row
        patternBlocks = new PatternBlock[n, n];

        // queue of blocks being displayed in order of sequence number
        displayBlocks = new List<PatternBlock>();

        if (startWithFullPattern)
        {
            displayIndex = n;
        }
        else
        {
            displayIndex = 1;
        }
        

        GenerateRandomSequence();

        GetPatternBlocksInOrder();
    }

    private void GetPatternBlocksInOrder()
    {
        // for each sequence number
        for (int s = 0; s < n * n; s++)
        {
            // for each pattern block
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (patternBlocks[i, j].sequenceNumber == s)
                    {
                        
                        if (isComplete) 
                        {
                            // reset material
                            patternBlocks[i, j].ResetMaterial();
                        }

                        // Add to the display queue
                        displayBlocks.Add(patternBlocks[i, j]);

                    }
                }
            }
        }
    }

    private void GenerateRandomSequence()
    {
        List<int> randomList = new List<int>();
        System.Random rand = new System.Random();
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

    private void ShowBlocksCorrect()
    {
        for (int i = 0; i < displayBlocks.Count; i++)
        {
            displayBlocks[i].ShowBlockCorrect();
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
                ShowBlocksCorrect();
                isComplete = true;
                isActive = false;
            }
            else if (currentGuess == guessesAllowed)
            {
                displayIndex++;
                Debug.Log("Display index = " + displayIndex);
                showPattern = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (hintCanvas != null)
            {
                hintCanvas.gameObject.SetActive(true);
                // show on UI to press Q
                if (isActive)
                {
                    // Reset Pattern
                    hintText.text = "Press Q to Reset Pattern";
                }
                else
                {
                    // Start Pattern
                    hintText.text = "Press Q to Start Pattern";
                }
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // hide UI
        if (hintCanvas != null)
        {
            hintCanvas.gameObject.SetActive(false);
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isActive = !isActive;
        }
    }

    private void FixedUpdate()
    {
        if (!isActive)
        {
            return;
        }
        if (displayIndex <= displayBlocks.Count && displayIndex >= 1)
        {
            ShowPattern();

            CheckPatternAttempts();
        }    

        
    }
}
