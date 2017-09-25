using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryLoader
{
    public static void loadDictionary(AutoCompleteDictionaryTrie d, string filename)
    {
        using (System.IO.StreamReader sr = System.IO.File.OpenText(filename))
        {
            string nextWord;
            
            
 
            while ((nextWord = sr.ReadLine()) != null)
            {
                string[] split = nextWord.Split('\t');
                d.addWord(split[0], Int32.Parse(split[1]));
            }
        }
    }

    public static List<string> loadPhraseSets(string filename)
    {
        using (System.IO.StreamReader sr = System.IO.File.OpenText(filename))
        {
            List<string> phraseSets = new List<string>();
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                phraseSets.Add(line);
            }
            return phraseSets;
        }
    }

}