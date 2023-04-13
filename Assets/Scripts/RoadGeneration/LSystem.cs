using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LSystem : MonoBehaviour
{
    private Rule[] rules;

    private int iterationLimit;

    private bool randomIgnoreRuleModifier;

    private float chanceToIgnoreRule;

    //Assigns a word from axiom
    public string GenerateSentence(string word, Rule[] rules, int iterationLimit, bool randomIgnoreRuleModifier, float chanceToIgnoreRule)
    {
        this.rules = rules;
        this.iterationLimit = iterationLimit;
        this.randomIgnoreRuleModifier= randomIgnoreRuleModifier;
        this.chanceToIgnoreRule= chanceToIgnoreRule;

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
