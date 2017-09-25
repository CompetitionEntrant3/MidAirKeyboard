using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
class TrieNode
{
    private Dictionary<Char, TrieNode> children;
    private String text;  // Maybe omit for space
    private int frequency;
    private bool isWord;

    /** Create a new TrieNode */
    public TrieNode()
    {
        children = new Dictionary<Char, TrieNode>();
        text = "";
        isWord = false;
    }

    /** Create a new TrieNode given a text String to store in it */
    public TrieNode(String text)
    {
        children = new Dictionary<Char, TrieNode>();

        isWord = false;
        this.text = text;
    }


    public TrieNode getChild(Char c)
    {
        if (children.ContainsKey(c))
            return children[c];
        else          
            return null;
        
    }

    public TrieNode insert(Char c)
    {
        if (children.ContainsKey(c))
        {
            return null;
        }

        TrieNode next = new TrieNode(text + c.ToString());
        children[c] = next;
        return next;
    }

    /** Return the text string at this node */
    public String getText()
    {
        return text;
    }

    public void setFrequency(int freq)
    {
        frequency = freq;
    }

	public int getFrequency (){
		return frequency;
	}

    /** Set whether or not this node ends a word in the trie. */
    public void setEndsWord(bool b)
    {
        isWord = b;
    }

    /** Return whether or not this node ends a word in the trie. */
    public bool endsWord()
    {
        return isWord;
    }

    /** Return the set of characters that have links from this node */
    public List<char> getValidNextCharacters()
    {
        return new List<char>(children.Keys);
    }
    /*
    char[] result = new char[children.Count];
    children.Keys.CopyTo(result, 0)
        return result;
*/
    public String toString()
    {
        return text;
    }

}