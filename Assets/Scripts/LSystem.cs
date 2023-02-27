using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LSystem : MonoBehaviour
{
    public Rule[] rules;
    public string axiom;

    [Range(0,10)]
    public int iterationLimit = 1;

    public bool randomIgnoreRuleModifier = true;
    [Range(0, 1)]
    public float chanceToIgnoreRule = 0.3f;

    private void Start()
    {
        Debug.Log(GenerateSentence());
    }

    //Assigns a word from axiom
    public string GenerateSentence(string word = null)
    {
        if (word == null)
        {
            word = axiom;
        }
        return GrowRecursive(word);
    }

    //Recursive algorithm to grow sentence of rules
    private string GrowRecursive(string word, int currentIteration = 0)
    {
        if(currentIteration >= iterationLimit)
        {
            return word;
        }
        StringBuilder newWord = new StringBuilder();

        foreach(char c in word)
        {
            newWord.Append(c);
            ProcessRule(newWord, c, currentIteration);
        }

        return newWord.ToString();
    }

    private void ProcessRule(StringBuilder newWord, char c, int currentIteration)
    {
        foreach(Rule rule in rules)
        {
            if(rule.letter == c.ToString())
            {
                if (randomIgnoreRuleModifier && currentIteration > 1)
                {
                    if(UnityEngine.Random.value < chanceToIgnoreRule)
                    {
                        return;
                    }
                }
                newWord.Append(GrowRecursive(rule.GetResult(), currentIteration + 1));
            }
        }
    }
}
