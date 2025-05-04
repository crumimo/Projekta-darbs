using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource[] backgroundMusicSources;
    public AudioSource dialogueMusicSource;

    [Header("Fade Settings")]
    public float fadeDuration = 2f; 
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        for (int i = 0; i < backgroundMusicSources.Length; i++)
        {
            if (backgroundMusicSources[i] != null)
            {
                backgroundMusicSources[i].volume = 0f;
            }
        }
    }
    
    private void Start()
    {
        for (int i = 0; i < backgroundMusicSources.Length; i++)
        {
            if (backgroundMusicSources[i] != null && backgroundMusicSources[i].clip != null)
            {
                backgroundMusicSources[i].Play();
            }
        }
    
        StartCoroutine(FadeInAudioSources(backgroundMusicSources, fadeDuration));
    }
    
    public void StartDialogue(AudioClip dialogueClip)
    {
        StartCoroutine(DoStartDialogue(dialogueClip));
    }
    
    private IEnumerator DoStartDialogue(AudioClip dialogueClip)
    {
        yield return StartCoroutine(FadeOutAudioSources(backgroundMusicSources, fadeDuration));
        
        dialogueMusicSource.clip = dialogueClip;
        dialogueMusicSource.volume = 0f;
        dialogueMusicSource.Play();
        
        yield return StartCoroutine(FadeInAudioSource(dialogueMusicSource, fadeDuration));
    }
    
    public void EndDialogue()
    {
        StartCoroutine(DoEndDialogue());
    }
    
    private IEnumerator DoEndDialogue()
    {
        yield return StartCoroutine(FadeOutAudioSource(dialogueMusicSource, fadeDuration));
        
        yield return StartCoroutine(FadeInAudioSources(backgroundMusicSources, fadeDuration));
    }
    
    private IEnumerator FadeOutAudioSources(AudioSource[] sources, float duration)
    {
        float timer = 0f;
        float[] startVolumes = new float[sources.Length];
        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i] != null)
                startVolumes[i] = sources[i].volume;
        }
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            for (int i = 0; i < sources.Length; i++)
            {
                if (sources[i] != null)
                    sources[i].volume = Mathf.Lerp(startVolumes[i], 0f, t);
            }
            yield return null;
        }
        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i] != null)
                sources[i].volume = 0f;
        }
    }
    
    private IEnumerator FadeInAudioSource(AudioSource source, float duration)
    {
        float timer = 0f;
        float startVol = source.volume;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            source.volume = Mathf.Lerp(startVol, 1f, t);
            yield return null;
        }
        source.volume = 1f;
    }
    
    private IEnumerator FadeInAudioSources(AudioSource[] sources, float duration)
    {
        float timer = 0f;
        float[] startVolumes = new float[sources.Length];
        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i] != null)
                startVolumes[i] = sources[i].volume;
        }
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            for (int i = 0; i < sources.Length; i++)
            {
                if (sources[i] != null)
                    sources[i].volume = Mathf.Lerp(startVolumes[i], 1f, t);
            }
            yield return null;
        }
        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i] != null)
                sources[i].volume = 1f;
        }
    }
    
    private IEnumerator FadeOutAudioSource(AudioSource source, float duration)
    {
        float timer = 0f;
        float startVol = source.volume;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            source.volume = Mathf.Lerp(startVol, 0f, t);
            yield return null;
        }
        source.volume = 0f;
        source.Stop();
    }
}
