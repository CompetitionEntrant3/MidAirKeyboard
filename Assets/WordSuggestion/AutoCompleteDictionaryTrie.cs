using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AutoCompleteDictionaryTrie {


	private Dictionary<Char,Vector3> keyPos;

	public class Gaussian3D{
		public Vector3 mean;
		public Vector3 stdDev;

		public double prob(Vector3 pos){
			// Assume stdDev for all dimension are the same
			double normConstant = 1.0/Math.Pow(2*Math.PI,1.5)*Math.Pow(stdDev.x,3);
			return normConstant * Math.Exp((-(pos-mean).sqrMagnitude) / (2* stdDev.x * stdDev.x));
		}
	}


	public struct WordScore{
		public string text;
		public double score;
		public int freq;
	}
    private TrieNode root;
    private int triesize = 0;

	private List<WordScore> completionsList;


public AutoCompleteDictionaryTrie()
{
    root = new TrieNode();
		completionsList = new List<WordScore> ();

		keyPos=new Dictionary<char, Vector3>();
		string letters="ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower();
		foreach (Char c in letters) {
			keyPos[c] = GameObject.Find (c.ToString ().ToUpper()).transform.position;
		}
}



public bool addWord(string word, int freq)
{
    char[] c = word.ToLower().ToCharArray();
    TrieNode curr = root;
    bool isANewWord = false;
    bool newNodeAdded = false;
    foreach (char x in c)
    {
        TrieNode y = curr.insert(x);
        if (y != null)
            newNodeAdded = true;
        curr = curr.getChild(x);
    }

    if (newNodeAdded)
        isANewWord = true;
    else
    {
        if (!curr.endsWord())
            isANewWord = true;
    }
    curr.setEndsWord(true);

    if (curr.endsWord())
        curr.setFrequency(freq);
    if (isANewWord)
        triesize++;
    return isANewWord;
}
     

public int size()
{

    return triesize;
}



public bool isWord(string s)
{
    s = s.ToLower();
    TrieNode curr = root;
    for (int i = 0; i < s.Length; i++)
    {
        curr = curr.getChild(s[i]);
        if (curr == null)
        {
            return false;
        }
    }
    return curr.endsWord();

}


public List<string> predictCompletions(KeyTapData[] keyData, int numCompletions)
{

		string prefix = "";
		foreach (var k in keyData)
			prefix += k.ToString ();
        //Debug.Log("Prefix: " + prefix);
    prefix = prefix.ToLower();  
 //   List<string> completionsList = new List<string>();
		completionsList.Clear();
    List<char> validCharacters;

    LinkedList<TrieNode> queue = new LinkedList<TrieNode>();

    TrieNode stem = root;

    if (root.getValidNextCharacters().Count == 0)
    {
			return completionsList.Select( x =>{return x.text;} ).ToList();
    }

	List<TrieNode> possibleStems= new List<TrieNode>();
		possibleStems.Add (root);
	List<TrieNode> possibleStemsTmp= new List<TrieNode>();
	foreach (Char c in prefix.ToCharArray())
	{
			possibleStemsTmp.Clear ();
			var adjKeys = StoreInput.getAdjacentKeys (c.ToString ());
			adjKeys.Add (c.ToString ());
			foreach (var s in possibleStems){
				foreach (var adjKey in adjKeys){
				//	Debug.Log (adjKey [0]);
					var child = s.getChild (adjKey[0]);
					if (child != null) {
						possibleStemsTmp.Add(child);

					}
				}
			}
			possibleStems.Clear ();
			foreach (var item in possibleStemsTmp) {
			//	Debug.Log ("Current Stem: " + item.getText ());
				possibleStems.Add (item);
			}
	}
	if (possibleStems.Count == 0)
			return completionsList.Select( x =>{return x.text;} ).ToList();
      //  else return completionsList;
	
		foreach (var s in possibleStems) {
	//		Debug.Log ("Stems: "+s.getText());
		
			queue.AddFirst (s);
    

			while (queue.Count != 0 && completionsList.Count < numCompletions) {
        
				TrieNode curr = queue.First.Value;
				queue.RemoveFirst ();

				if (curr != null) {

					if (curr.endsWord ()) {
						if (curr.getFrequency () > 100) {
							WordScore w = new WordScore ();
							w.text = curr.getText ();
							w.score = Math.Log(curr.getFrequency ());
						//	w.score = 1;
							w.freq = curr.getFrequency ();

//							Debug.Log (w.text);
							for (int i=0;i<keyData.Length;++i) {
								Gaussian3D gauss = new Gaussian3D ();
								gauss.mean = keyPos [w.text[i]];
								gauss.stdDev = new Vector3 (0.6f, 0.6f, 0.6f);
								w.score*=gauss.prob (keyData [i].fingerTipPosition);
							}
							w.score *= ((float)keyData.Length / w.text.Length);
							completionsList.Add (w);
						}
					}

					if (curr.getValidNextCharacters ().Count > 0) {

						validCharacters = curr.getValidNextCharacters ();
						foreach (char c in validCharacters) {
							queue.AddLast (curr.getChild (c));
						}
					}
				}
			}
		}
	
		completionsList.Sort ((x,y)=> {return -x.score.CompareTo(y.score);});
		foreach (var word in completionsList) {
			//Debug.Log (word.text+" "+word.freq);
		}
		return completionsList.Select( x =>{return x.text;} ).ToList();
}
     
}
