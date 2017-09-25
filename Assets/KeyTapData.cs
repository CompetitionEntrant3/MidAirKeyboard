using Leap;
using Leap.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyTapData : IEquatable<KeyTapData>
{

    public float[] fingerVelocity = new float[5];
	public Vector3 fingerTipPosition;
    public string input;
    public float distanceToKeyCenter;
    public Collider collider;
    public float spatialScore;

    public bool Equals(KeyTapData other)
    {
        if (this.input == other.input)
            return true;
        else
            return false;
    }

    public override string ToString()
    {
        return input;
    }
    public void SetFingerVelocity(Leap.Unity.RigidHand hand)
    {


        Leap.Unity.FingerModel[] fingers = hand.fingers;

        for (int i = 0; i < fingers.Length; i++)
        {
            fingerVelocity[i] = fingers[i].GetLeapFinger().TipVelocity.Magnitude;
        }

    }

    public Finger.FingerType fastestFinger(Leap.Unity.RigidHand hand)
    {
        Leap.Unity.FingerModel[] fingers = hand.fingers;
        float velocity = 0;
        int finger = -1;

        for (int i = 0; i < fingers.Length; i++)
        {
            if (fingerVelocity[i] > velocity)
            {
                velocity = fingerVelocity[i];
                finger = i;
            }
        }
        switch (finger)
        {
            case 0:
                return Finger.FingerType.TYPE_THUMB;
            case 1:
                return Finger.FingerType.TYPE_INDEX;
            case 2:
                return Finger.FingerType.TYPE_MIDDLE;
            case 3:
                return Finger.FingerType.TYPE_RING;
            case 4:
                return Finger.FingerType.TYPE_PINKY;
        }

        return Finger.FingerType.TYPE_UNKNOWN;
        
    }

    public double findDistanceToKeyCenter(KeyTapHandler key)
    {
        Vector3 colliderPosition = new Vector3(collider.gameObject.transform.position.x, collider.gameObject.transform.position.y,
            collider.gameObject.transform.position.z);

        Vector3 keyPosition = new Vector3(GameObject.Find(key.name).transform.position.x, GameObject.Find(key.name).transform.position.y,
            GameObject.Find(key.name).transform.position.z);

        distanceToKeyCenter = Vector3.Distance(colliderPosition, keyPosition);
        return distanceToKeyCenter;
        
    }
    public double findDistanceToKeyCenter2(CubeTapHandler key, string s)
    {
        Vector3 colliderPosition = new Vector3(collider.gameObject.transform.position.x, collider.gameObject.transform.position.y,
            collider.gameObject.transform.position.z);
        Vector3 keyPosition = new Vector3(GameObject.Find(s.ToUpper()).transform.position.x, GameObject.Find(s.ToUpper()).transform.position.y,
            GameObject.Find(s.ToUpper()).transform.position.z);        
        distanceToKeyCenter = Vector3.Distance(colliderPosition, keyPosition);
        //Debug.Log(s + ": " + colliderPosition + "   " + keyPosition);
        distanceToKeyCenter = getDistance(collider.gameObject.transform.position.x, collider.gameObject.transform.position.y, collider.gameObject.transform.position.z,
            GameObject.Find(s.ToUpper()).transform.position.x, GameObject.Find(s.ToUpper()).transform.position.y, GameObject.Find(s.ToUpper()).transform.position.z);
        Debug.Log(distanceToKeyCenter);
        return distanceToKeyCenter;

    }

    public static float getDistance(float x1, float y1, float z1, float x2, float y2, float z2)
    {
        float dx = x1 - x2;
        float dy = y1 - y2;
        float dz = z1 - z2;

        // We should avoid Math.pow or Math.hypot due to perfomance reasons
        return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public Vector3 getCoordinates()
    {
        Vector3 colliderPosition = new Vector3(collider.gameObject.transform.position.x, collider.gameObject.transform.position.y,
            collider.gameObject.transform.position.z);
        return colliderPosition;
    }




}
