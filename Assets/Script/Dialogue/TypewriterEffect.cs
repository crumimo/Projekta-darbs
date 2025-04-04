using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float typewriterSpeed = 50f;
    [SerializeField] private int lettersPerSound = 4;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private CharacterVoice[] characterVoices;

    public bool IsRunning { get; private set; }

    private readonly List<Punctuation> punctuations = new List<Punctuation>()
    {
        new Punctuation(new HashSet<char>() {'.', '!', '?'}, 0.6f),
        new Punctuation(new HashSet<char>() {',', ';', ':'}, 0.3f)
    };

    private Coroutine typingCoroutine;
    private TMP_Text textLabel;
    private string textToType;
    private string speakerName;
    private Dictionary<string, (AudioClip[] clips, int index)> voiceDictionary;

    private void Start()
    {
        voiceDictionary = new Dictionary<string, (AudioClip[], int)>();
        foreach (var voice in characterVoices)
        {
            voiceDictionary[voice.characterName] = (voice.voiceClips, 0);
        }
    }

    public void Run(string textToType, TMP_Text textLabel, string speakerName)
    {
        this.textToType = textToType;
        this.textLabel = textLabel;
        this.speakerName = speakerName;

        typingCoroutine = StartCoroutine(TypeText());
    }

    public void Stop()
    {
        if (!IsRunning) return;

        StopCoroutine(typingCoroutine);
        OnTypingCompleted();
    }

    private IEnumerator TypeText()
    {
        IsRunning = true;

        textLabel.maxVisibleCharacters = 0;
        textLabel.text = textToType;

        float t = 0;
        int charIndex = 0;

        while (charIndex < textToType.Length)
        {
            int lastCharIndex = charIndex;

            t += Time.deltaTime * typewriterSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            for (int i = lastCharIndex; i < charIndex; i++)
            {
                bool isLast = i >= textToType.Length - 1;
                textLabel.maxVisibleCharacters = i + 1;

                if (i % lettersPerSound == 0 && voiceDictionary.ContainsKey(speakerName))
                {
                    PlayVoiceSound(speakerName);
                }

                if (IsPunctuation(textToType[i], out float waitTime) && !isLast && !IsPunctuation(textToType[i + 1], out _))
                {
                    yield return new WaitForSeconds(waitTime);
                }
            }

            yield return null;
        }

        OnTypingCompleted();
    }

    private void PlayVoiceSound(string speakerName)
    {
        if (voiceDictionary.TryGetValue(speakerName, out var voiceData) && voiceData.clips.Length > 0 && audioSource != null)
        {
            // Воспроизводим звук по порядку
            AudioClip clipToPlay = voiceData.clips[voiceData.index];
            audioSource.PlayOneShot(clipToPlay);

            // Увеличиваем индекс и сбрасываем его, если он выходит за границы массива
            int nextIndex = (voiceData.index + 1) % voiceData.clips.Length;
            voiceDictionary[speakerName] = (voiceData.clips, nextIndex);
        }
    }

    private void OnTypingCompleted()
    {
        IsRunning = false;
        textLabel.maxVisibleCharacters = textToType.Length;
    }

    private bool IsPunctuation(char character, out float waitTime)
    {
        foreach (Punctuation punctuationCategory in punctuations)
        {
            if (punctuationCategory.Punctuations.Contains(character))
            {
                waitTime = punctuationCategory.WaitTime;
                return true;
            }
        }

        waitTime = default;
        return false;
    }

    private readonly struct Punctuation
    {
        public readonly HashSet<char> Punctuations;
        public readonly float WaitTime;

        public Punctuation(HashSet<char> punctuations, float waitTime)
        {
            Punctuations = punctuations;
            WaitTime = waitTime;
        }
    }
}

[Serializable]
public class CharacterVoice
{
    public string characterName;
    public AudioClip[] voiceClips;
}