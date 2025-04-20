using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

public class CombinationIconManager : MonoBehaviour
{
    [Header("Combination Icon Settings")]
    public Image combinationImage; 
    private Coroutine imageFadeCoroutine; 
    private HashSet<string> usedCombinations = new(); 

    
    private void Start()
    {
        if (combinationImage != null)
        {
            combinationImage.gameObject.SetActive(false);
        }
    }
    public void DisplayCombinationIcon(string word1, string word2)
    {
        string combinationKey = GetCombinationKey(word1, word2);

        if (string.IsNullOrEmpty(combinationKey) || !usedCombinations.Contains(combinationKey))
        {
            Debug.Log($"Combination {combinationKey} has not been used yet or is invalid.");
            ResetCombinationIcon(); 
            return;
        }

        Sprite combinationSprite = WordManager.Instance.GetCombinationSprite(word1, word2);
        if (combinationSprite != null)
        {
            combinationImage.sprite = combinationSprite;

            if (imageFadeCoroutine != null)
            {
                StopCoroutine(imageFadeCoroutine);
            }

            imageFadeCoroutine = StartCoroutine(FadeInImage(combinationImage));
        }
        else
        {
            Debug.LogWarning($"No sprite found for combination: {word1} + {word2}");
            ResetCombinationIcon(); 
        }
    }

    
    public void MarkCombinationAsUsed(string word1, string word2)
    {
        string combinationKey = GetCombinationKey(word1, word2);
        if (!usedCombinations.Contains(combinationKey))
        {
            usedCombinations.Add(combinationKey);
        }
    }

    
    private string GetCombinationKey(string word1, string word2)
    {
        if (string.IsNullOrEmpty(word1) || string.IsNullOrEmpty(word2))
        {
            return null; 
        }

        return word1.CompareTo(word2) < 0 ? $"{word1}+{word2}" : $"{word2}+{word1}";
    }
    
    public void ResetCombinationIcon()
    {
        if (imageFadeCoroutine != null)
        {
            StopCoroutine(imageFadeCoroutine);
        }

        if (combinationImage != null)
        {
            combinationImage.gameObject.SetActive(false); 
            combinationImage.sprite = null; 
        }
    }

    private IEnumerator FadeInImage(Image image, float duration = 0.5f)
    {
        image.gameObject.SetActive(true);
        Color color = image.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            color.a = Mathf.Clamp01(elapsedTime / duration);
            image.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = 1f;
        image.color = color;
        imageFadeCoroutine = null;
    }

    private IEnumerator FadeOutImage(Image image, float duration = 0.5f)
    {
        Color color = image.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            color.a = 1f - Mathf.Clamp01(elapsedTime / duration);
            image.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = 0f;
        image.color = color;
        image.gameObject.SetActive(false);
        imageFadeCoroutine = null;
    }
}