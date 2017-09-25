using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap;
using System;
using System.Text;
using Leap.Unity;

public class NextPhraseHandler : MonoBehaviour {

    public Leap.Finger.FingerType supportedFinger;
    public static GameObject testPhraseBar;
    public static GameObject phraseNumber;
    public static GameObject completedBlockImage;
    public static GameObject completedBlockText;
    public static GameObject LeapHandController;
    public static List<string> phrases;
    public static int counterx;
    private int characterCounter, wordCounter = 0;
    private static bool newPhrasePress, newWordPress;
    public static float timeBetweenPhraseInput;
    public static bool buttonPressed;
    private static bool reachedEndOfBlock;
    private static int blockNumber = 0;
    private static string blockName;
    public static int phraseNumb, wordNumb;
    private int alignHandsCounter;
    public static bool leapHandsActive = false;
    public static string selectedWord;
    private static string[] currentWords;
    private static int currentWordLength;
    public static bool newPhraseSet = true;
    public static bool endPress = false;


    // Use this for initialization
    void Start () {      
        phrases = DictionaryLoader.loadPhraseSets("Assets/PhraseSets/T-20.txt");
        currentWords = phrases[0].Split(' ');
        currentWordLength = currentWords[0].Length;
        wordCounter = phrases[counterx].Split(' ')[0].Length + 1;
        selectedWord = phrases[0].Substring(0, currentWordLength);
        Debug.Log(selectedWord + "HAMANAFNAWEIONFAWEIFNAWEIOGN");
        testPhraseBar = GameObject.Find("PhraseSet");
        testPhraseBar.GetComponent<Text>().text = selectedWord + phrases[0].Substring(currentWordLength, phrases[0].Length - currentWordLength);

        phraseNumber = GameObject.Find("PhraseNumber");
        phraseNumber.GetComponent<Text>().text = "Block 1, Phrase 1:";
        blockName = "Block 1";
        counterx = 0;
        phraseNumb = 1;
        wordNumb = 1;
        completedBlockImage = GameObject.Find("Image");
        completedBlockText = GameObject.Find("CompletedBlock");
        completedBlockImage.SetActive(true);
        completedBlockText.SetActive(true);

        LeapHandController = GameObject.Find("LeapHandController");
        
        var hand_pool = FindObjectOfType<LeapHandController>();
        hand_pool.GetComponent<HandPool>().EnableGroup("Graphics_Hands");


    }

   
    private void FixedUpdate()
    {
        if (alignHandsCounter < 500)
            alignHandsCounter++;
        if (alignHandsCounter == 500)
        {
            alignHandsCounter++;
            var hand_pool = FindObjectOfType<LeapHandController>();
            //hand_pool.GetComponent<HandPool>().DisableGroup("Graphics_Hands");
        }
    }
    // Update is called once per frame
    void Update () {
        if (timeBetweenPhraseInput > 0.5 && Input.GetKeyDown("b") && leapHandsActive)
        {
            endPress = true;
            newPhrasePress = true;
            
        }
            

        if (timeBetweenPhraseInput > 0.5 && newWordPress == true)
        {
            newWordPress = false;
            timeBetweenPhraseInput = 0;
            wordNumb++;
            if (characterCounter < phrases[counterx].Split(' ').Length - 2)
            {
                characterCounter++;               
                
                string firstpart = phrases[counterx].Substring(0, wordCounter - 1) + " ";
                selectedWord = phrases[counterx].Split(' ')[characterCounter];
                string secondpart = " " + phrases[counterx].Substring(wordCounter + phrases[counterx].Split(' ')[characterCounter].Length + 1);

                testPhraseBar.GetComponent<Text>().text = firstpart + "<color=red>" + selectedWord + "</color>" + secondpart;
                wordCounter += phrases[counterx].Split(' ')[characterCounter].Length + 1;               
            }
            else
            {                
                newPhrasePress = true;
            }
            
        }
        if (newPhrasePress == true)
        {           
            newPhrasePress = false;
            counterx++;
            newPhraseSet = true;
            characterCounter = 0;          
            
            timeBetweenPhraseInput = 0;

            KeyTapHandler.keyList.Clear();
            KeyTapHandler.inputBar.GetComponent<Text>().text = "";
                    
            phraseNumb++;
            if (counterx < phrases.Count)
            {
                wordCounter = phrases[counterx].Split(' ')[0].Length + 1;
                selectedWord = phrases[counterx].Split(' ')[0];
                testPhraseBar.GetComponent<Text>().text = selectedWord + phrases[counterx].Substring(phrases[counterx].Split(' ')[0].Length);

                phraseNumber.GetComponent<Text>().text = blockName + ", Phrase " + (counterx + 1) + "/" + phrases.Count + ":";
                
            }
            else
            {
                Debug.Log("reached end of block");
                reachedEndOfBlock = true;
            }

            if (reachedEndOfBlock)
            {
                reachedEndOfBlock = false;
                blockNumber++;
                
                if (blockNumber == 1)
                {
                    phrases = DictionaryLoader.loadPhraseSets("Assets/PhraseSets/T-20_2.txt");
                    counterx = 0;
                    wordCounter = phrases[counterx].Split(' ')[0].Length + 1;
                    selectedWord = phrases[counterx].Split(' ')[0];
                    testPhraseBar.GetComponent<Text>().text = selectedWord + phrases[counterx].Substring(phrases[counterx].Split(' ')[0].Length);
                    phraseNumber.GetComponent<Text>().text = "Block 2, Phrase " + (counterx + 1) + "/5" + ":";
                    blockName = "Block 2";
                    completedBlockText.GetComponent<Text>().text = "You have completed the block! You may take a 1 minute break before continuing. When you are done, press the space bar to begin the next block.";
                }
                else if (blockNumber == 2)
                {
                    phrases = DictionaryLoader.loadPhraseSets("Assets/PhraseSets/T-20_3.txt");
                    counterx = 0;
                    wordCounter = phrases[counterx].Split(' ')[0].Length + 1;
                    selectedWord = phrases[counterx].Split(' ')[0];
                    testPhraseBar.GetComponent<Text>().text = selectedWord + phrases[counterx].Substring(phrases[counterx].Split(' ')[0].Length);
                    phraseNumber.GetComponent<Text>().text = "Block 3, Phrase " + (counterx + 1) + "/5" + ":";
                    blockName = "Block 3";
                }
                else if (blockNumber == 3)
                {
                    phrases = DictionaryLoader.loadPhraseSets("Assets/PhraseSets/T-20_4.txt");
                    counterx = 0;
                    wordCounter = phrases[counterx].Split(' ')[0].Length + 1;
                    selectedWord = phrases[counterx].Split(' ')[0];
                    testPhraseBar.GetComponent<Text>().text = selectedWord + phrases[counterx].Substring(phrases[counterx].Split(' ')[0].Length);
                    phraseNumber.GetComponent<Text>().text = "Block 4, Phrase " + (counterx + 1) + "/5" + ":";
                    blockName = "Block 4";
                }
                else if (blockNumber == 4)
                {
                    completedBlockText.GetComponent<Text>().text = "You have completed all 4 blocks. Thank you for participating in our user study!";
                }
            

                              
                completedBlockImage.SetActive(true);
                completedBlockText.SetActive(true);
                leapHandsActive = false;
                LeapHandController.SetActive(false);
                
            }
            
            
        }
        if (Input.GetKeyDown("space") && phrases[counterx] != null && blockNumber != 5)
        {
            selectedWord = phrases[counterx].Split(' ')[0];
            completedBlockImage.SetActive(false);
            completedBlockText.SetActive(false);
            LeapHandController.SetActive(true);
            leapHandsActive = true;
            var hand_pool = FindObjectOfType<LeapHandController>();
            hand_pool.GetComponent<HandPool>().EnableGroup("Graphics_Hands");
            alignHandsCounter = 0;
        }
    }
}
