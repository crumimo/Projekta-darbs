using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EffectApplicationManager : MonoBehaviour 
{
    public static EffectApplicationManager Instance;

    [Header("References")]
    public Transform playerTransform;
    public Movement playerMovement;
    public EnemyManager enemyManager;
    public CombinationIconManager combinationIconManager;
    public AudioSource audioSource;
    
    [Header("Effect Settings")]
    public float effectRadius = 10f;

    private void Awake() 
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public bool ApplyCombinationEffect(List<string> selectedWords)
    {
        if (selectedWords == null || selectedWords.Count != 2)
        {
            Debug.LogError("Invalid number of words for applying the effect.");
            ShowFailHint();
            return false;
        }

        string word1 = selectedWords[0];
        string word2 = selectedWords[1];

        combinationIconManager.MarkCombinationAsUsed(word1, word2);

        EffectBase effect = WordManager.Instance.GetEffect(word1, word2);
        AudioClip effectSound = WordManager.Instance.GetEffectSound(word1, word2);

        if (effect == null)
        {
            Debug.LogError("Effect not found for the given combination.");
            ShowFailHint();
            return false;
        }

        bool effectApplied = false;

        if (effect is SpikeCircleEffect || effect is GaleStride)
        {
            ApplyEffectToPlayer(effect);
            PlayEffectSound(effectSound);
            effectApplied = true;
        }
        else
        {
            effectApplied = ApplyEffectToObjects(effect);
            if (effectApplied) PlayEffectSound(effectSound);
        }

        if (!effectApplied)
        {
            Debug.LogWarning("The effect was not applied.");
            ShowFailHint();
        }

        combinationIconManager.ResetCombinationIcon();
        return effectApplied;
    }

    
    private void ApplyEffectToPlayer(EffectBase effect)
    {
        effect.Apply(playerTransform.gameObject);
        Debug.Log($"Effect {effect.name} applied to the player");
        // If needed, add fade animation or additional logic here.
    }
    
    private bool ApplyEffectToObjects(EffectBase effect)
    {
        var effectables = FindObjectsOfType<MonoBehaviour>().OfType<IEffectable>();
        bool effectApplied = false;

        foreach (var effectable in effectables)
        {
            MonoBehaviour mb = effectable as MonoBehaviour;
            if (effect is HollowNest && mb.GetComponent<HideAndSeekEnemyEye>() != null)
            {
                Debug.Log($"{mb.name} is an enemy eye, skipping HollowNest effect.");
                continue;
            }
            if (effectable.CanReceiveEffect(playerTransform.position, effectRadius, effect))
            {
                bool appliedSuccessfully = effectable.ApplyEffect(effect);
                if (appliedSuccessfully)
                {
                    Debug.Log($"{effect.GetType().Name} applied to {mb.name}");
                    effectApplied = true;
                }
                else
                {
                    Debug.Log($"{mb.name} did not accept the effect, even though it's in range.");
                }
            }
            else
            {
                Debug.Log($"{mb.name} is out of range (using its own logic).");
            }
        }

        if (!effectApplied)
            Debug.LogWarning("The effect was not applied to any objects.");

        return effectApplied;
    }
    
    private bool AnyObjectInRange()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(playerTransform.position, effectRadius);
        foreach (var collider in colliders)
        {
            if (collider.GetComponent<IEffectable>() != null)
                return true;
        }
        return false;
    }
    
    private void PlayEffectSound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    private void ShowFailHint()
    {
        HintPanelController hint = FindObjectOfType<HintPanelController>();
        if (hint != null)
        {
            hint.Show("Nothing happened...");
            StartCoroutine(HideHintAfterDelay(hint, 2f));
        }
    }

    private IEnumerator HideHintAfterDelay(HintPanelController hint, float delay)
    {
        yield return new WaitForSeconds(delay);
        hint.Hide();
    }

}
