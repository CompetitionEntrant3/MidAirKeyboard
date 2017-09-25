using Leap;
using Leap.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CollisionDataHandler : MonoBehaviour {

    public float peakVelocityThreshold;

    public static StreamWriter sr;
    public static StreamWriter sr2, sr3;
    public static string filename;
    public static string secondfilename;
    public static GameObject plane;
    public static GameObject phraseSetBar;
    public static float timeBetweenDataInput;
    private static bool newCollisionPress;
    private LeapServiceProvider leapProvider;

    public static List<TextCollisionInput> collisionInputs;
    private string positionValuesLeft, positionValuesLeft2;
    private string velocityValuesLeft, velocityValuesLeft2;
    private string positionValuesRight, positionValuesRight2;
    private string velocityValuesRight, velocityValuesRight2;
    private string directionRight, directionLeft, directionRight2, directionLeft2;
    private string palmLeft, palmRight, palmLeftVelocity, palmRightVelocity;
    private string palmLeft2, palmRight2, palmLeftVelocity2, palmRightVelocity2;
    private int thePhraseNumber;



    void Start ()
    {
        //first info collection
        plane = GameObject.Find("CollisionPlane");

        leapProvider = FindObjectOfType<LeapServiceProvider>();
        int userNumber = 1;
        filename = "user" + userNumber;
        while (File.Exists(filename))
        {
            userNumber++;
            filename = "user" + userNumber;
        }
        //sr = File.CreateText("user" + userNumber);        
        //sr.WriteLine("Time\tPosition\tVelocity\tTipDirection\tPalmCoords\tPalmVelocity\tSelectedWord\tPhraseNumber\tWordNumber\tFingerType");

        sr3 = File.CreateText("user" + userNumber);
        collisionInputs = new List<TextCollisionInput>();

        //second info collection
        secondfilename = "updatedata" + userNumber;
        //sr2 = File.CreateText("updatedata" + userNumber);
        //sr2.WriteLine("Time\tPosition\tVelocity\tTipDirection\tPalmCoords\tPalmVelocity\tSelectedWord\tPhraseNumber\tWordNumber");





    }

    // Update is called once per frame
    void Update()
    {
        if (KeyTapHandler.newPress)
        {
            KeyTapHandler.newPress = false;
            sr3.WriteLine(Time.time + "start" + NextPhraseHandler.counterx);
        }
        if (NextPhraseHandler.endPress)
        {
            NextPhraseHandler.endPress = false;
            sr3.WriteLine(Time.time + "end" + NextPhraseHandler.counterx);
        }
    }

    private void FixedUpdate()
    {
        /*
        Frame currentFrame = leapProvider.CurrentFrame;
        foreach (Hand hand in currentFrame.Hands)
        {
            if (hand.IsLeft)
            {
                positionValuesLeft2 = ""+hand.Fingers[0].TipPosition + hand.Fingers[1].TipPosition + hand.Fingers[2].TipPosition
                     + hand.Fingers[3].TipPosition + hand.Fingers[4].TipPosition;
                velocityValuesLeft2 = ""+hand.Fingers[0].TipVelocity + hand.Fingers[1].TipVelocity + hand.Fingers[2].TipVelocity
                    + hand.Fingers[3].TipVelocity + hand.Fingers[4].TipVelocity;
                directionLeft = ""+hand.Fingers[0].Direction + hand.Fingers[1].Direction + hand.Fingers[2].Direction
                    + hand.Fingers[3].Direction + hand.Fingers[4].Direction;
                palmLeft = hand.PalmPosition.ToString();
                palmLeftVelocity = hand.PalmVelocity.ToString();
            }
            else
            {
                positionValuesRight2 = "" + hand.Fingers[0].TipPosition + hand.Fingers[1].TipPosition + hand.Fingers[2].TipPosition
                     + hand.Fingers[3].TipPosition + hand.Fingers[4].TipPosition;
                velocityValuesRight2 = "" + hand.Fingers[0].TipVelocity + hand.Fingers[1].TipVelocity + hand.Fingers[2].TipVelocity
                    + hand.Fingers[3].TipVelocity + hand.Fingers[4].TipVelocity;
                directionRight = ""+hand.Fingers[0].Direction + hand.Fingers[1].Direction + hand.Fingers[2].Direction
                    + hand.Fingers[3].Direction + hand.Fingers[4].Direction;
                palmRight = hand.PalmPosition.ToString();
                palmRightVelocity = hand.PalmVelocity.ToString();

            }

        }
        
        if (NextPhraseHandler.leapHandsActive && palmRight != null && palmLeft != null && velocityValuesLeft2 != null && velocityValuesRight2 != null)
            sr2.WriteLine(Time.time + "\t" + positionValuesLeft2 + positionValuesRight2 + "\t"
                + velocityValuesLeft2 + velocityValuesRight2 + "\t"
                + directionLeft + directionRight + "\t"
                + palmLeft + palmRight + "\t" + palmLeftVelocity + palmRightVelocity + "\t" + NextPhraseHandler.selectedWord + "\t" + NextPhraseHandler.phraseNumb + "\t" + NextPhraseHandler.wordNumb);

        
           */ 
        
    }

    private void OnTriggerEnter(Collider other)
    {
    
        /*       
        float peakVelocity = other.attachedRigidbody.velocity.magnitude;

        if (peakVelocity > peakVelocityThreshold && (other.attachedRigidbody.velocity.y < 0))
        {
            //get pos and vel values
            Frame currentFrame = leapProvider.CurrentFrame;
            
            foreach (Hand hand in currentFrame.Hands)
            {
                if (hand.IsLeft)
                {
                    positionValuesLeft = "" + hand.Fingers[0].TipPosition + hand.Fingers[1].TipPosition + hand.Fingers[2].TipPosition
                     + hand.Fingers[3].TipPosition + hand.Fingers[4].TipPosition;
                    velocityValuesLeft = "" + hand.Fingers[0].TipVelocity + hand.Fingers[1].TipVelocity + hand.Fingers[2].TipVelocity
                    + hand.Fingers[3].TipVelocity + hand.Fingers[4].TipVelocity;
                    directionLeft2 = "" + hand.Fingers[0].Direction + hand.Fingers[1].Direction + hand.Fingers[2].Direction
                    + hand.Fingers[3].Direction + hand.Fingers[4].Direction;                  
                    palmLeft2 = hand.PalmPosition.ToString();
                    palmLeftVelocity2 = hand.PalmVelocity.ToString();

                }
                else
                {
                    positionValuesRight = "" + hand.Fingers[0].TipPosition + hand.Fingers[1].TipPosition + hand.Fingers[2].TipPosition
                     + hand.Fingers[3].TipPosition + hand.Fingers[4].TipPosition;
                    velocityValuesRight = "" + hand.Fingers[0].TipVelocity + hand.Fingers[1].TipVelocity + hand.Fingers[2].TipVelocity
                    + hand.Fingers[3].TipVelocity + hand.Fingers[4].TipVelocity;
                    directionRight2 = "" + hand.Fingers[0].Direction + hand.Fingers[1].Direction + hand.Fingers[2].Direction
                    + hand.Fingers[3].Direction + hand.Fingers[4].Direction;
                    palmRight2 = hand.PalmPosition.ToString();
                    palmRightVelocity2 = hand.PalmVelocity.ToString();

                }

            }
            
            //get positions
            string lrhand;
            if (other.ToString().Contains("left"))
                lrhand = "left";
            else lrhand = "right";


            if (NextPhraseHandler.leapHandsActive && palmRight2 != null && palmLeft2 != null && velocityValuesLeft != null && velocityValuesRight != null)
                sr.WriteLine(Time.time + "\t" + positionValuesLeft + positionValuesRight + "\t"
                    + velocityValuesLeft + velocityValuesRight + "\t"
                    + directionLeft2 + directionRight2 + "\t"
                    + palmLeft2 + palmRight2 + "\t" + palmLeftVelocity2 + palmRightVelocity2 + "\t" + NextPhraseHandler.selectedWord + "\t" + NextPhraseHandler.phraseNumb + "\t" + NextPhraseHandler.wordNumb
                    + "\t" + lrhand + other.gameObject.GetComponentInParent<Leap.Unity.RigidFinger>().fingerType);

            //collisionInputs.Add(new TextCollisionInput(lrhand, "" + other.gameObject.GetComponentInParent<Leap.Unity.RigidFinger>().fingerType, 
            //     positionValuesLeft + positionValuesRight,  velocityValuesLeft + velocityValuesRight, Time.time, NextPhraseHandler.phraseNumb));





        }
      */  
    }

    private void OnTriggerExit(Collider other)
    {
        /*
        
        for (int i = 0; i < collisionInputs.Count; i++)
        {
            TextCollisionInput input = collisionInputs[i];
            if ("" + other.gameObject.GetComponentInParent<Leap.Unity.RigidFinger>().fingerType == input.getFingerType()
                && (other.ToString().Contains("left") == input.getHandType().Contains("left")))
            {
                input.setEndTime(Time.time);
                input.writeToFile(sr);
                collisionInputs.Remove(input);
                i--;
            }

        }
        */
    }
    private void OnApplicationQuit()
    {
        //sr.Close();
        //sr2.Close();
        sr3.Close();
    }
}
