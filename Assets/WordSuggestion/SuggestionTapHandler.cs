using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap;
using Leap.Unity;

public class SuggestionTapHandler : MonoBehaviour
{

    public Leap.Finger.FingerType supportedFinger;
    //Modify this to "false" if you don't want touch Typing 
    public static bool isTouchTyping = true;
    public float peakVelocityThreshold;
    public bool isBackspace;
    public bool isSelectWord;

    //text that is displayed on the key and output on keypress
    public static float timeBetweenSuggestionInput;
    private static bool newKeyPress;

    public static GameObject suggestion1;
    public static GameObject suggestion2;
    public static GameObject suggestion3;
    public static GameObject suggestion4;
    public static GameObject suggestion5;
    public static string currentlySelectedWord;
    public static int wordCounter = 1;
    public static GameObject inputBar;



    void Start()
    {
        suggestion1 = GameObject.Find("Suggestion1");
        suggestion2 = GameObject.Find("Suggestion2");
        suggestion3 = GameObject.Find("Suggestion3");
        suggestion4 = GameObject.Find("Suggestion4");
        suggestion5 = GameObject.Find("Suggestion5");
        suggestion1.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
        inputBar = GameObject.Find("InputBar");
    }


    void Update()
    {
        //only process these if the list is completed AND a new key is pressed
        if (timeBetweenSuggestionInput > 0.12 && newKeyPress == true)
        {

            newKeyPress = false;
          
            
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        Material m = GetComponent<Renderer>().material;
        // The key stroke will be triggered if the finger object's contacting velocity higher than a threshold
        if (isTouchTyping)
        {
            float peakVelocity = other.attachedRigidbody.velocity.magnitude;

            if (peakVelocity > peakVelocityThreshold && (other.attachedRigidbody.velocity.y < 0))
            {

                m.color = new Color(0, 1, 0, 1);
          

                //if more than 0.2s delay
                if (timeBetweenSuggestionInput > 0.15 && other.gameObject.GetComponentInParent<Leap.Unity.RigidFinger>().fingerType == supportedFinger)
                {
                    if (!isBackspace)
                    {
                        if (wordCounter == 1)
                            currentlySelectedWord = GameObject.Find("Suggestion1").GetComponentInChildren<TextMesh>().text;
                        timeBetweenSuggestionInput = 0;
                        newKeyPress = true;
                        KeyTapHandler.inputBar.GetComponent<Text>().text =
                            KeyTapHandler.inputBar.GetComponent<Text>().text
                            .Substring(0, KeyTapHandler.inputBar.GetComponent<Text>().text.Length - KeyTapHandler.removeAmount);
                        //KeyTapHandler.removeAmount = gameObject.GetComponentInChildren<TextMesh>().text.Length;
                        KeyTapHandler.removeAmount = currentlySelectedWord.Length + 1;
                        //KeyTapHandler.inputBar.GetComponent<Text>().text += gameObject.GetComponentInChildren<TextMesh>().text;
                        KeyTapHandler.inputBar.GetComponent<Text>().text += currentlySelectedWord+" ";
                        KeyTapHandler.keyList.Clear();
                    }
                    else
                    {
                        if (other.gameObject.GetComponentInParent<Leap.Unity.RigidFinger>().fingerType == supportedFinger && !isSelectWord)
                        {
                            timeBetweenSuggestionInput = 0;
                            newKeyPress = true;
                            string[] parts = inputBar.GetComponent<Text>().text.Split(' ');
                            string lastWord;
                            int removeAmount;
                            if (parts[parts.Length - 1] != "")
                            {
                                lastWord = parts[parts.Length - 1];
                                removeAmount = lastWord.Length;
                            }
                            else
                            {
                                lastWord = parts[parts.Length - 2];
                                removeAmount = lastWord.Length + 1;
                            }
                            
                            
                            KeyTapHandler.inputBar.GetComponent<Text>().text =
                                KeyTapHandler.inputBar.GetComponent<Text>().text
                                .Substring(0, KeyTapHandler.inputBar.GetComponent<Text>().text.Length - removeAmount);

                            KeyTapHandler.keyList.RemoveRange(KeyTapHandler.keyList.Count - removeAmount, removeAmount);
                            

                            KeyTapData[] keyData = new KeyTapData[KeyTapHandler.keyList.Count];
                            for (int i = 0; i < keyData.Length; ++i)
                            {
                                keyData[i] =KeyTapHandler.keyList[i].ToArray()[0];
                            }
                            KeyTapHandler.processSuggestions(keyData);
                            
                        } else if (other.gameObject.GetComponentInParent<Leap.Unity.RigidFinger>().fingerType == supportedFinger)
                        {
                            timeBetweenSuggestionInput = 0;
                            newKeyPress = true;

                            
                            switch (wordCounter)
                            {
                                case 1:
                                    currentlySelectedWord = suggestion1.GetComponentInChildren<TextMesh>().text;
                                    suggestion1.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
                                    suggestion5.GetComponent<Renderer>().material.color = new Color(0, 0, 0.5f, 1);
                                    break;
                                case 2:
                                    currentlySelectedWord = suggestion2.GetComponentInChildren<TextMesh>().text;
                                    suggestion2.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
                                    suggestion1.GetComponent<Renderer>().material.color = new Color(0, 0, 0.5f, 1);
                                    break;
                                case 3:
                                    currentlySelectedWord = suggestion3.GetComponentInChildren<TextMesh>().text;
                                    suggestion3.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
                                    suggestion2.GetComponent<Renderer>().material.color = new Color(0, 0, 0.5f, 1);
                                    break;
                                case 4:
                                    currentlySelectedWord = suggestion4.GetComponentInChildren<TextMesh>().text;
                                    suggestion4.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
                                    suggestion3.GetComponent<Renderer>().material.color = new Color(0, 0, 0.5f, 1);
                                    break;
                                case 5:
                                    currentlySelectedWord = suggestion5.GetComponentInChildren<TextMesh>().text;
                                    suggestion5.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
                                    suggestion4.GetComponent<Renderer>().material.color = new Color(0, 0, 0.5f, 1);
                                    break;  
                                   
                            }
                            wordCounter++;
                            if (wordCounter >= 6)
                                wordCounter = 1;


                        }
                    }
                }
                //if less than 0.2s has passed
                else
                {

                }
            }
            else
                m.color = new Color(1, 0, 0, 1);
        }
        else
        {
            m.color = new Color(1, 0, 0, 1);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        Material m = GetComponent<Renderer>().material;

        m.color = new Color(0, 0, 0.5f, 1);

    }
}

