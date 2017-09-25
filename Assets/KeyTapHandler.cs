using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap;
using Leap.Unity;
using System.IO;

public class KeyTapHandler : MonoBehaviour
{
    //Modify this to "false" if you don't want touch Typing 
    public static bool isTouchTyping = true;

    public string symbol;
    public CapsuleCollider fingerEnd;

    public float peakVelocityThreshold;
    public Leap.Finger.FingerType supportedFinger, supportedFinger2;

    public static GameObject inputBar;

    public static float timeBetweenKeyInput;
    private static bool newKeyPress;
    public static List<List<KeyTapData>> keyList = new List<List<KeyTapData>>();
    
    public static string inputString = "";

    public static int removeAmount;
    public static GameObject keyboard, collisionplane;
    public LeapServiceProvider leapProvider;
    public static int lengthcount;
    public static bool newPress= false;

    void Start()
    {
        //array = FindObjectOfType<UHProvider>() as UHProvider; //associate the array
        //provider = FindObjectOfType<LeapProvider>() as LeapProvider; //associate leap motion
        inputBar = GameObject.Find("InputBar");
        inputBar.GetComponent<Text>().text = "";
        symbol = gameObject.GetComponentInChildren<TextMesh>().text.ToLower();
        keyboard = GameObject.Find("Keyboard");
        collisionplane = GameObject.Find("CollisionPlane");
        leapProvider = FindObjectOfType<LeapServiceProvider>();

    }


    void Update()
    {
        //only process these if the list is completed AND a new key is pressed
        if (timeBetweenKeyInput > 0.15 && newKeyPress == true)
        {
            newKeyPress = false;
            if (symbol != " ")
                processSpatialModel();
            printList();
            if (keyList[keyList.Count - 1][0].ToString() == " ")
                keyList.Clear();
            KeyTapData[] keyData = new KeyTapData[keyList.Count];
            for(int i=0; i < keyData.Length; ++i)
            {
                keyData[i] = keyList[i].ToArray()[0];
            }

            processSuggestions(keyData);

            
        }
        if (Input.GetKeyDown("space"))
            print("");
        //if (lengthcount >= NextPhraseHandler.phrases[NextPhraseHandler.counterx].Length)
        //{
            //lengthcount = 0;
            //endPress = true;
        //}
    }
    /*

    private void FixedUpdate()
    {
        Frame currentFrame = leapProvider.CurrentFrame;
        Vector3 vector;


        if (currentFrame.Hands[0].PalmPosition.y > currentFrame.Hands[1].PalmPosition.y)
        {
            vector = new Vector3(keyboard.transform.position.x, currentFrame.Hands[1].PalmPosition.y, keyboard.transform.position.z);
        }
        else
        {
            vector = new Vector3(keyboard.transform.position.x, currentFrame.Hands[0].PalmPosition.y, keyboard.transform.position.z);
        }

        keyboard.transform.position = vector;

     
    }*/


	private void printList()    
    {
        KeyTapData[] arr = keyList[keyList.Count - 1].ToArray();
        string combinedString = ""; 
        for (int i = 0; i < arr.Length; i++)
        {
            combinedString += arr[i].ToString();
        }
        //Debug.Log("combinedString: "+combinedString);
        inputString += combinedString;
        inputBar.GetComponent<Text>().text += combinedString;

        return;
       
    }

	public static void processSuggestions(KeyTapData[] keyData)
    {
        string[] parts = inputBar.GetComponent<Text>().text.Split(' ');
        string lastWord = parts[parts.Length - 1];
        removeAmount = lastWord.Length;


		List<string> options = StoreInput.tr.predictCompletions(keyData, 100);
    /*    List<List<string>> letters = new List<List<string>>();
        for (int i = 0; i < lastWord.Length; i++)
        { 
        letters.Add(StoreInput.getAdjacentKeys(lastWord[i].ToString()));        
        }*/
      //  List<string> combinations = StoreInput.getCombinations(letters);

        GameObject.Find("Suggestion1").GetComponentInChildren<TextMesh>().text = options[0];
        GameObject.Find("Suggestion2").GetComponentInChildren<TextMesh>().text = options[1];
        GameObject.Find("Suggestion3").GetComponentInChildren<TextMesh>().text = options[2];
        GameObject.Find("Suggestion4").GetComponentInChildren<TextMesh>().text = options[3];
        GameObject.Find("Suggestion5").GetComponentInChildren<TextMesh>().text = options[4];
        
    }

    private void processSpatialModel()
    {
        float highestKeyInputDistance = 0f;
        float highestSpatialScore = 0f;
        for (int i = 0; i < keyList[keyList.Count - 1].Count; i++)
        {
            //find highestKeyInputDistance from all distance values in current list
            if (keyList[keyList.Count - 1][i].distanceToKeyCenter > highestKeyInputDistance)
                highestKeyInputDistance = keyList[keyList.Count - 1][i].distanceToKeyCenter;
        }
        for (int i = 0; i < keyList[keyList.Count - 1].Count; i++)
        {
            //find spatialScore of each KeyTapData
            keyList[keyList.Count - 1][i].spatialScore = keyList[keyList.Count - 1][i].distanceToKeyCenter / highestKeyInputDistance;

            //find highestSpatialScore from all distance values in current list
            if (keyList[keyList.Count - 1][i].spatialScore > highestSpatialScore)
                highestSpatialScore = keyList[keyList.Count - 1][i].spatialScore;

            
        }
        for (int i = 0; i < keyList[keyList.Count - 1].Count; i++)
        {
            //delete KeyTapData in current list if its spatialScore is less than the highestSpatialScore
            if (keyList[keyList.Count - 1].Count > 1 && keyList[keyList.Count - 1][i].spatialScore < highestSpatialScore)
            {
                keyList[keyList.Count - 1].Remove(keyList[keyList.Count - 1][i]);
                i--;
            }
        }
           

    }


    private void OnTriggerEnter(Collider other)
    {
        Material m = GetComponent<Renderer>().material;
        // The key stroke will be triggered if the finger object's contacting velocity higher than a threshold
        if (isTouchTyping && (other.gameObject.GetComponentInParent<Leap.Unity.RigidFinger>().fingerType == supportedFinger
            || other.gameObject.GetComponentInParent<Leap.Unity.RigidFinger>().fingerType == supportedFinger2))
        //if (isTouchTyping && other.gameObject.GetComponentInParent<Leap.Unity.RigidFinger>().fingerType == supportedFinger)
            //  if(isTouchTyping)
            {
            float peakVelocity = other.attachedRigidbody.velocity.magnitude;
           
            if (peakVelocity > peakVelocityThreshold && (other.attachedRigidbody.velocity.y < 0))
            {

                
                m.color = new Color(0, 1, 0, 1);
                KeyTapData CurrentData = new KeyTapData();
                CurrentData.input = symbol;
                CurrentData.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData.collider = other;
                CurrentData.findDistanceToKeyCenter(this);

                //if more than 0.2s delay, make a new list in the 2D list and add a value to that list
                if (timeBetweenKeyInput > 0.1)
                {
                                            
                    if (supportedFinger == CurrentData.fastestFinger(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>())
                        || supportedFinger2 == CurrentData.fastestFinger(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>()))
                    {

                        if (NextPhraseHandler.newPhraseSet == true)
                        {
                            NextPhraseHandler.newPhraseSet = false;
                            newPress = true;
                        }

                        lengthcount++;
                        keyList.Add(new List<KeyTapData>());                       
                        keyList[keyList.Count - 1].Add(CurrentData);
                        timeBetweenKeyInput = 0;
                        newKeyPress = true;

                        SuggestionTapHandler.wordCounter = 1;
                        SuggestionTapHandler.currentlySelectedWord = GameObject.Find("Suggestion1").GetComponentInChildren<TextMesh>().text;
                        GameObject.Find("Suggestion1").GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
                        GameObject.Find("Suggestion2").GetComponent<Renderer>().material.color = new Color(0, 0, 0.5f, 1);
                        GameObject.Find("Suggestion3").GetComponent<Renderer>().material.color = new Color(0, 0, 0.5f, 1);
                        GameObject.Find("Suggestion4").GetComponent<Renderer>().material.color = new Color(0, 0, 0.5f, 1);
                        GameObject.Find("Suggestion5").GetComponent<Renderer>().material.color = new Color(0, 0, 0.5f, 1);



                    }
                        

                }
                //if less than 0.2s has passed, append value to the current list in the 2D list
                else
                {                    
                    
                    if (!(keyList[keyList.Count - 1].Contains(CurrentData)))
                    {
                        if (supportedFinger == CurrentData.fastestFinger(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>()))
                            keyList[keyList.Count - 1].Add(CurrentData);
                    }

                }


                //returns distance between 1. the closest point on the finger to the center of the key and 2. center of the key
                // Debug.Log(Vector3.Distance(other.ClosestPoint(GameObject.Find(this.name).transform.position),
                //    GameObject.Find(this.name).transform.position));
           

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

   

    /*
    private void computeFDCurve_intensities()
    {
        for (int i = 0; i < travel_sampling_rate; i++)
        {
            float _temp_offset = offset_travel * travel_sampling_rate;
            float _temp_switching = switching_travel * travel_sampling_rate;
            float _temp_trigger = trigger_travel * travel_sampling_rate;

            if (i < _temp_offset)
            {
                _force_intensities[i] = initial_force;
            }
            else if (i < _temp_switching)
            {
                _force_intensities[i] = ((initial_force - switching_force) / (_temp_offset - _temp_switching)) * (i - _temp_offset) + initial_force;
            }
            else if (i < _temp_trigger)
            {
                _force_intensities[i] = ((final_force - switching_force) / (_temp_trigger - _temp_switching)) * (i - _temp_switching) + switching_force;
            }
            else
            {
                _force_intensities[i] = final_force;
            }
        }
    }
    */

}


