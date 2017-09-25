using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap;
using System;
using System.Text;

public class StoreInput : MonoBehaviour
{

    public static AutoCompleteDictionaryTrie tr;
    

    private void Start()
    {
        GenerateAutoCompleteDictionary();
		var t1 = Time.realtimeSinceStartup;
		KeyTapData[] keys = new KeyTapData[4];
		for(int i=0;i<keys.Length;++i)
			keys [i] = new KeyTapData ();
		keys[0].input="s";
		keys[1].input="h";
		keys[2].input="a";
		keys[3].input="r";

		keys [0].fingerTipPosition = GameObject.Find ("S").transform.position;
		keys [1].fingerTipPosition = GameObject.Find ("H").transform.position;
		keys [2].fingerTipPosition = GameObject.Find ("A").transform.position;
		keys [3].fingerTipPosition = GameObject.Find ("R").transform.position;
	//	keys [4].fingerTipPosition = GameObject.Find ("E").transform.position;

		var res=tr.predictCompletions (keys,2000);
		//Debug.Log((Time.realtimeSinceStartup - t1).ToString());


        
    }

    void Update()
    {

        KeyTapHandler.timeBetweenKeyInput += Time.deltaTime;
        CubeTapHandler.atimeBetweenKeyInput += Time.deltaTime;
        SuggestionTapHandler.timeBetweenSuggestionInput += Time.deltaTime;
        CollisionDataHandler.timeBetweenDataInput += Time.deltaTime;
        NextPhraseHandler.timeBetweenPhraseInput += Time.deltaTime;

        
    }


    private void GenerateAutoCompleteDictionary()
    {
        tr = new AutoCompleteDictionaryTrie();
        DictionaryLoader.loadDictionary(tr, "Assets/corpus.txt");
           
    }
    public static List<string> getAdjacentKeys(string key)
    {
        switch (key)
        {
            case "q":
                return new List<string> { "w", "a","s" };
            case "w":
                return new List<string> { "q", "s", "e", "a","d" };
            case "e":
                return new List<string> { "w", "d", "r", "s","f" };
            case "r":
                return new List<string> { "e", "f", "t", "d","g"};
            case "t":
                return new List<string> { "r", "g", "y", "f","h" };
            case "y":
                return new List<string> { "t", "h", "u", "g" ,"j"};
            case "u":
                return new List<string> { "y", "j", "i", "h" ,"k"};
            case "i":
                return new List<string> { "u", "j", "k", "o" ,"l"};
            case "o":
                return new List<string> { "i", "k", "l", "p" };
            case "p":
                return new List<string> { "o", "l" };
            case "a":
                return new List<string> { "q", "z", "s","w","x" };
            case "s":
                return new List<string> { "w", "x", "a", "d" ,"z","q","e"};
            case "d":
                return new List<string> { "s", "f", "c", "e","x" };
            case "f":
                return new List<string> { "d", "g", "r", "t", "v" };
            case "g":
                return new List<string> { "b", "t", "f", "h","v" };
            case "h":
                return new List<string> { "n", "j", "y", "g","b" };
            case "j":
                return new List<string> { "u", "h", "k", "m","n","i","y" };
            case "k":
                return new List<string> { "i", "l" ,"j","m"};
            case "l":
                return new List<string> { "o", "p", "k","i" };
            case "z":
                return new List<string> { "x", "a","s" };
            case "x":
                return new List<string> { "z", "c", "s","d" };
            case "c":
                return new List<string> { "v", "x", "d","f" };
            case "v":
                return new List<string> { "f", "c", "b", "g" };
            case "b":
                return new List<string> { "v", "n", "g","h" };
            case "n":
                return new List<string> { "m", "h", "b","j" };
            case "m":
                return new List<string> { "n", "j","k" };   

        }
        return null;
    }
    public static List<string> getCombinations(List<List<string>> twoDimStringArray)
        {
            // keep track of the size of each inner String array
            int[] sizeArray = new int[twoDimStringArray.Count];

            // keep track of the index of each inner String array which will be used
            // to make the next combination
            int[] counterArray = new int[twoDimStringArray.Count];

            // Discover the size of each inner array and populate sizeArray.
            // Also calculate the total number of combinations possible using the
            // inner String array sizes.
            int totalCombinationCount = 1;
            for (int i = 0; i < twoDimStringArray.Count; ++i)
            {
                sizeArray[i] = twoDimStringArray[i].Count;
                totalCombinationCount *= twoDimStringArray[i].Count;
            }

            // Store the combinations in a List of String objects
            List<string> combinationList = new List<string>(totalCombinationCount);

        System.Text.StringBuilder sb;  // more efficient than String for concatenation

            for (int countdown = totalCombinationCount; countdown > 0; --countdown)
            {
                // Run through the inner arrays, grabbing the member from the index
                // specified by the counterArray for each inner array, and build a
                // combination string.
                sb = new StringBuilder();
                for (int i = 0; i < twoDimStringArray.Count; ++i)
                {
                    sb.Append(twoDimStringArray[i][counterArray[i]]);
                
                }

            combinationList.Add(sb.ToString());// add new combination to list

            // Now we need to increment the counterArray so that the next
            // combination is taken on the next iteration of this loop.
            for (int incIndex = twoDimStringArray.Count - 1; incIndex >= 0; --incIndex)
                {
                    if (counterArray[incIndex] + 1 < sizeArray[incIndex])
                    {
                        ++counterArray[incIndex];
                        // None of the indices of higher significance need to be
                        // incremented, so jump out of this for loop at this point.
                        break;
                    }
                    // The index at this position is at its max value, so zero it
                    // and continue this loop to increment the index which is more
                    // significant than this one.
                    counterArray[incIndex] = 0;
                }
            }
            return combinationList;
        }
}




