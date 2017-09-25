using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap;
using Leap.Unity;
using System.IO;

public class CubeTapHandler : MonoBehaviour
{
    //Modify this to "false" if you don't want touch Typing 
    public static bool isTouchTyping = true;

    public string symbol;
    public CapsuleCollider fingerEnd, thumbl, indexl, middlel, ringl, pinkyl, thumbr, indexr, middler, ringr, pinkyr;

    public float peakVelocityThreshold;
    public Leap.Finger.FingerType supportedFinger;

    public static GameObject inputBar;

    public static float atimeBetweenKeyInput;
    private static bool newKeyPress;
    public static List<List<KeyTapData>> keyList = new List<List<KeyTapData>>();
    
    public static string inputString = "";

    public static int removeAmount;
    public static GameObject keyboard, collisionplane;
    public LeapServiceProvider leapProvider;

    

    void Start()
    {
        //array = FindObjectOfType<UHProvider>() as UHProvider; //associate the array
        //provider = FindObjectOfType<LeapProvider>() as LeapProvider; //associate leap motion
        inputBar = GameObject.Find("InputBar");
        inputBar.GetComponent<Text>().text = "";
        symbol = "cube";
        keyboard = GameObject.Find("Keyboard");
        collisionplane = GameObject.Find("CollisionPlane");
        leapProvider = FindObjectOfType<LeapServiceProvider>();
        Vector3 qPos = GameObject.Find("Q").transform.position;
        Vector3 wPos = GameObject.Find("W").transform.position;
        Vector3 ePos = GameObject.Find("E").transform.position;
        Vector3 rPos = GameObject.Find("R").transform.position;
        Vector3 tPos = GameObject.Find("T").transform.position;
        Vector3 yPos = GameObject.Find("Y").transform.position;
        Vector3 uPos = GameObject.Find("U").transform.position;
        Vector3 iPos = GameObject.Find("I").transform.position;
        Vector3 oPos = GameObject.Find("O").transform.position;
        Vector3 pPos = GameObject.Find("P").transform.position;
        Vector3 aPos = GameObject.Find("A").transform.position;
        Vector3 sPos = GameObject.Find("S").transform.position;
        Vector3 dPos = GameObject.Find("D").transform.position;
        Vector3 fPos = GameObject.Find("F").transform.position;
        Vector3 gPos = GameObject.Find("G").transform.position;
        Vector3 hPos = GameObject.Find("H").transform.position;
        Vector3 jPos = GameObject.Find("J").transform.position;
        Vector3 kPos = GameObject.Find("K").transform.position;
        Vector3 lPos = GameObject.Find("L").transform.position;
        Vector3 zPos = GameObject.Find("Z").transform.position;
        Vector3 xPos = GameObject.Find("X").transform.position;
        Vector3 cPos = GameObject.Find("C").transform.position;
        Vector3 vPos = GameObject.Find("V").transform.position;
        Vector3 bPos = GameObject.Find("B").transform.position;
        Vector3 nPos = GameObject.Find("N").transform.position;
        Vector3 mPos = GameObject.Find("M").transform.position;


    }


    void Update()
    {
        //only process these if the list is completed AND a new key is pressed
        if (atimeBetweenKeyInput > 0.15 && newKeyPress == true)
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
    }

    private void FixedUpdate()
    {
        Frame currentFrame = leapProvider.CurrentFrame;
        foreach (Hand hand in currentFrame.Hands)
        {
           
        Vector3 vector = new Vector3(keyboard.transform.position.x, hand.PalmPosition.y, keyboard.transform.position.z);
        keyboard.transform.position = vector;  
        Vector3 vector1 = new Vector3(collisionplane.transform.position.x, hand.PalmPosition.y, collisionplane.transform.position.z);
        collisionplane.transform.position = vector1;
        }
    }


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
        float lowestKeyInputDistance = 0f;
        float lowestSpatialScore = 100f;
        for (int i = 0; i < keyList[keyList.Count - 1].Count; i++)
        {
            //find spatialScore of each KeyTapData
            keyList[keyList.Count - 1][i].spatialScore = keyList[keyList.Count - 1][i].distanceToKeyCenter;

            //find highestSpatialScore from all distance values in current list
            if (keyList[keyList.Count - 1][i].spatialScore < lowestSpatialScore)
                lowestSpatialScore = keyList[keyList.Count - 1][i].spatialScore;

            
        }
        for (int i = 0; i < keyList[keyList.Count - 1].Count; i++)
        {
            //delete KeyTapData in current list if its spatialScore is less than the highestSpatialScore
            if (keyList[keyList.Count - 1].Count > 1 && keyList[keyList.Count - 1][i].spatialScore > lowestSpatialScore)
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
        if (isTouchTyping)
      //  if(isTouchTyping)
        {
            Collider fingercollider;
            float peakVelocity = other.attachedRigidbody.velocity.magnitude;
            
            if (peakVelocity > peakVelocityThreshold && (other.attachedRigidbody.velocity.y < 0))
            {

                m.color = new Color(0, 1, 0, 1);
                fingercollider = other;
                
                

                KeyTapData CurrentData1 = new KeyTapData();                
                CurrentData1.input = "a";
                //CurrentData1.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData1.collider = fingercollider;
                CurrentData1.findDistanceToKeyCenter2(this,"a");
                Debug.Log("sdafvrbvreavgawervgwae");
                KeyTapData CurrentData2 = new KeyTapData();
                CurrentData2.input = "b";
                //CurrentData2.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData2.collider = fingercollider;
                CurrentData2.findDistanceToKeyCenter2(this, "b");

                KeyTapData CurrentData3 = new KeyTapData();
                CurrentData3.input = "c";
                //CurrentData3.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData3.collider = fingercollider;
                CurrentData3.findDistanceToKeyCenter2(this, "c");

                KeyTapData CurrentData4 = new KeyTapData();
                CurrentData4.input = "d";
                //CurrentData4.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData4.collider = fingercollider;
                CurrentData4.findDistanceToKeyCenter2(this, "d");

                KeyTapData CurrentData5 = new KeyTapData();
                CurrentData5.input = "e";
                //CurrentData5.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData5.collider = fingercollider;
                CurrentData5.findDistanceToKeyCenter2(this, "e");

                KeyTapData CurrentData6 = new KeyTapData();
                CurrentData6.input = "f";
                //CurrentData6.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData6.collider = fingercollider;
                CurrentData6.findDistanceToKeyCenter2(this, "f");

                KeyTapData CurrentData7 = new KeyTapData();
                CurrentData7.input = "g";
                //CurrentData7.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData7.collider = fingercollider;
                CurrentData7.findDistanceToKeyCenter2(this, "g");

                KeyTapData CurrentData8 = new KeyTapData();
                CurrentData8.input = "h";
                //CurrentData8.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData8.collider = fingercollider;
                CurrentData8.findDistanceToKeyCenter2(this, "h");

                KeyTapData CurrentData9 = new KeyTapData();
                CurrentData9.input = "i";
                //CurrentData9.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData9.collider = fingercollider;
                CurrentData9.findDistanceToKeyCenter2(this, "i");

                KeyTapData CurrentData10 = new KeyTapData();
                CurrentData10.input = "j";
                //CurrentData10.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData10.collider = fingercollider;
                CurrentData10.findDistanceToKeyCenter2(this, "j");

                KeyTapData CurrentData11 = new KeyTapData();
                CurrentData11.input = "k";
                //CurrentData11.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData11.collider = fingercollider;
                CurrentData11.findDistanceToKeyCenter2(this, "k");

                KeyTapData CurrentData12 = new KeyTapData();
                CurrentData12.input = "l";
                //CurrentData12.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData12.collider = fingercollider;
                CurrentData12.findDistanceToKeyCenter2(this, "l");

                KeyTapData CurrentData13 = new KeyTapData();
                CurrentData13.input = "m";
                //CurrentData13.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData13.collider = fingercollider;
                CurrentData13.findDistanceToKeyCenter2(this, "m");

                KeyTapData CurrentData14 = new KeyTapData();
                CurrentData14.input = "n";
                //CurrentData14.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData14.collider = fingercollider;
                CurrentData14.findDistanceToKeyCenter2(this, "n");

                KeyTapData CurrentData15 = new KeyTapData();
                CurrentData15.input = "o";
                //CurrentData15.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData15.collider = fingercollider;
                CurrentData15.findDistanceToKeyCenter2(this, "o");
                
                KeyTapData CurrentData16 = new KeyTapData();
                CurrentData16.input = "p";
                //CurrentData16.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData16.collider = fingercollider;
                CurrentData16.findDistanceToKeyCenter2(this, "p");

                KeyTapData CurrentData17 = new KeyTapData();
                CurrentData17.input = "q";
                //CurrentData17.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData17.collider = fingercollider;
                CurrentData17.findDistanceToKeyCenter2(this, "q");

                KeyTapData CurrentData18 = new KeyTapData();
                CurrentData18.input = "r";
                //CurrentData18.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData18.collider = fingercollider;
                CurrentData18.findDistanceToKeyCenter2(this, "r");

                KeyTapData CurrentData19 = new KeyTapData();
                CurrentData19.input = "s";
                //CurrentData19.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData19.collider = fingercollider;
                CurrentData19.findDistanceToKeyCenter2(this, "s");

                KeyTapData CurrentData20 = new KeyTapData();
                CurrentData20.input = "t";
                //CurrentData20.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData20.collider = fingercollider;
                CurrentData20.findDistanceToKeyCenter2(this, "t");

                KeyTapData CurrentData21 = new KeyTapData();
                CurrentData21.input = "u";
                //CurrentData21.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData21.collider = fingercollider;
                CurrentData21.findDistanceToKeyCenter2(this, "u");

                KeyTapData CurrentData22 = new KeyTapData();
                CurrentData22.input = "v";
                //CurrentData22.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData22.collider = fingercollider;
                CurrentData22.findDistanceToKeyCenter2(this, "v");

                KeyTapData CurrentData23 = new KeyTapData();
                CurrentData23.input = "w";
                //CurrentData23.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData23.collider = fingercollider;
                CurrentData23.findDistanceToKeyCenter2(this, "w");

                KeyTapData CurrentData24 = new KeyTapData();
                CurrentData24.input = "x";
                //CurrentData24.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData24.collider = fingercollider;
                CurrentData24.findDistanceToKeyCenter2(this, "x");

                KeyTapData CurrentData25 = new KeyTapData();
                CurrentData25.input = "y";
                //CurrentData25.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData25.collider = fingercollider;
                CurrentData25.findDistanceToKeyCenter2(this, "y");

                KeyTapData CurrentData26 = new KeyTapData();
                CurrentData26.input = "z";
                //CurrentData26.SetFingerVelocity(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>());
                CurrentData26.collider = fingercollider;
                CurrentData26.findDistanceToKeyCenter2(this, "z");


                //if more than 0.2s delay, make a new list in the 2D list and add a value to that list
                if (atimeBetweenKeyInput > 0.2)
                {
                    
                    //if (supportedFinger == CurrentData.fastestFinger(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>()))
                    //{                        
                        keyList.Add(new List<KeyTapData>());                       
                        keyList[keyList.Count - 1].Add(CurrentData1);
                        keyList[keyList.Count - 1].Add(CurrentData2);
                        keyList[keyList.Count - 1].Add(CurrentData3);
                        keyList[keyList.Count - 1].Add(CurrentData4);
                        keyList[keyList.Count - 1].Add(CurrentData5);
                        keyList[keyList.Count - 1].Add(CurrentData6);
                        keyList[keyList.Count - 1].Add(CurrentData7);
                        keyList[keyList.Count - 1].Add(CurrentData8);
                        keyList[keyList.Count - 1].Add(CurrentData9);
                        keyList[keyList.Count - 1].Add(CurrentData10);
                        keyList[keyList.Count - 1].Add(CurrentData11);
                        keyList[keyList.Count - 1].Add(CurrentData12);
                        keyList[keyList.Count - 1].Add(CurrentData13);
                        keyList[keyList.Count - 1].Add(CurrentData14);
                        keyList[keyList.Count - 1].Add(CurrentData15);
                        keyList[keyList.Count - 1].Add(CurrentData16);
                        keyList[keyList.Count - 1].Add(CurrentData17);
                        keyList[keyList.Count - 1].Add(CurrentData18);
                        keyList[keyList.Count - 1].Add(CurrentData19);
                        keyList[keyList.Count - 1].Add(CurrentData20);
                        keyList[keyList.Count - 1].Add(CurrentData21);
                        keyList[keyList.Count - 1].Add(CurrentData22);
                        keyList[keyList.Count - 1].Add(CurrentData23);
                        keyList[keyList.Count - 1].Add(CurrentData24);
                        keyList[keyList.Count - 1].Add(CurrentData25);
                        keyList[keyList.Count - 1].Add(CurrentData26);
                        
                        atimeBetweenKeyInput = 0;
                        newKeyPress = true;

                        SuggestionTapHandler.wordCounter = 1;
                        SuggestionTapHandler.currentlySelectedWord = GameObject.Find("Suggestion1").GetComponentInChildren<TextMesh>().text;
                        GameObject.Find("Suggestion1").GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
                        GameObject.Find("Suggestion2").GetComponent<Renderer>().material.color = new Color(0, 0, 0.5f, 1);
                        GameObject.Find("Suggestion3").GetComponent<Renderer>().material.color = new Color(0, 0, 0.5f, 1);
                        GameObject.Find("Suggestion4").GetComponent<Renderer>().material.color = new Color(0, 0, 0.5f, 1);
                        GameObject.Find("Suggestion5").GetComponent<Renderer>().material.color = new Color(0, 0, 0.5f, 1);



                    //}
                        

                }
                //if less than 0.2s has passed, append value to the current list in the 2D list
                else
                {                    
                    
                    if (!(keyList[keyList.Count - 1].Contains(CurrentData1)))
                    {
                        //if (supportedFinger == CurrentData.fastestFinger(other.gameObject.GetComponentInParent<Leap.Unity.RigidHand>()))
                            //keyList[keyList.Count - 1].Add(CurrentData);
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


