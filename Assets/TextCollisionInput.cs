using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextCollisionInput : MonoBehaviour {

    string handType;
    string fingerType;
    string position;
    string velocity;
    double startTime;
    double endTime;
    double phraseNum;

	// Use this for initialization
	void Start () {
		
	}

    public TextCollisionInput(string lr, string type, string pos, string vel, double start, int pn)
    {
        handType = lr;
        fingerType = type;
        position = pos;
        velocity = vel;
        startTime = start;
        phraseNum = pn;
        
    }

    public bool filledOut()
    {
        if (handType != null && fingerType != null && position != null && velocity != null && startTime.ToString() != "" && endTime.ToString() != "")
            return true;
        else return false;
    }
	
    public void writeToFile(StreamWriter writer)
    {
        writer.WriteLine(handType + fingerType + "\t" + position + "\t" + velocity + "\t" + startTime + "\t" + endTime + "\t" + phraseNum);
    }


    public string getFingerType()
    {
        return fingerType;
    }

    public string getHandType()
    {
        return handType;
    }

    public void setEndTime(double time)
    {
        endTime = time;
    }

}
