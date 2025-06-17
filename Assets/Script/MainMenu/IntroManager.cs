using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private FadeController fadeController;

    [Header("Settings")]
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private DialogueData dialogueData;

    [Header("Scene Loading")]
    [SerializeField] private string persistentGameplay;
    [SerializeField] private string levelScene;

    private Queue<DialogueEntry> sentenceQueue = new Queue<DialogueEntry>();
    private int currentBlockIndex = -1;
    private bool isTyping = false;
    private int soundIndex = 0;

    private TypewriterEffect typewriterEffect;
    [SerializeField] private Image blackOverlay;

    private List<AsyncOperation> sceneToLoad = new List<AsyncOperation>();
    private Scene introScene;
    private bool isSceneLoading = false;

    private void Awake()
    { 
        introScene = SceneManager.GetSceneByName("Intro");
        typewriterEffect = GetComponent<TypewriterEffect>();
    }

    private void Start()
    {
        StartCoroutine(StartWithFadeOut());
    }

    private IEnumerator StartWithFadeOut()
    {
        blackOverlay.color = new Color(0f, 0f, 0f, 1f);
        
        yield return new WaitForSeconds(0.2f);
        
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime / fadeSpeed;
            blackOverlay.color = new Color(0f, 0f, 0f, Mathf.Clamp01(alpha));
            yield return null;
        }

        blackOverlay.color = new Color(0f, 0f, 0f, 0f);
        StartIntro();
    }


    public void StartIntro()
    {
        sentenceQueue.Clear();
        currentBlockIndex = -1;
        ShowNextBlock();
    }

    private void ShowNextBlock()
    {
        currentBlockIndex++;

        if (currentBlockIndex >= dialogueData.dialogueBlocks.Length)
        {
            EndIntro();
            return;
        }

        DialogueBlock currentBlock = dialogueData.dialogueBlocks[currentBlockIndex];

        sentenceQueue.Clear();
        foreach (var sentence in currentBlock.sentences)
        {
            sentenceQueue.Enqueue(sentence);
        }

        if (currentBlockIndex == 0 && currentBlock.backgroundImage != null)
        {
            backgroundImage.sprite = currentBlock.backgroundImage;
            ShowNextSentence();
        }
        else if (currentBlock.backgroundImage != null)
        {
            StartCoroutine(ChangeBackgroundSmoothly(currentBlock.backgroundImage, ShowNextSentence));
        }
        else
        {
            ShowNextSentence();
        }
    }

    private void ShowNextSentence()
    {
        if (sentenceQueue.Count == 0)
        {
            ShowNextBlock();
            return;
        }

        DialogueEntry currentSentence = sentenceQueue.Dequeue();
        StartCoroutine(RunTypingEffect(currentSentence.sentence));
    }

    private IEnumerator RunTypingEffect(string sentence)
    {
        isTyping = true;
        
        typewriterEffect.Run(sentence, dialogueText, "");

        while (typewriterEffect.IsRunning)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                typewriterEffect.Stop();
            }
        }

        isTyping = false;
    }


    private IEnumerator ChangeBackgroundSmoothly(Sprite newBackground, Action onComplete)
    {
        float duration = fadeSpeed;
        Color color = backgroundImage.color;
        float alpha = 1f;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime / duration;
            backgroundImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        backgroundImage.sprite = newBackground;

        alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime / duration;
            backgroundImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        onComplete?.Invoke();
    }

    private void EndIntro()
    {
        if (isSceneLoading) return; 

        isSceneLoading = true; 

        AsyncOperation persistentLoad = SceneManager.LoadSceneAsync(persistentGameplay, LoadSceneMode.Additive);
        AsyncOperation levelLoad = SceneManager.LoadSceneAsync(levelScene, LoadSceneMode.Additive);

        persistentLoad.allowSceneActivation = false;
        levelLoad.allowSceneActivation = false;

        sceneToLoad.Add(persistentLoad);
        sceneToLoad.Add(levelLoad);

        StartCoroutine(LoadScenesWithFade());
    }

    private IEnumerator LoadScenesWithFade()
    {
        while (true)
        {
            float totalProgress = 0f;

            foreach (AsyncOperation op in sceneToLoad)
            {
                totalProgress += Mathf.Clamp01(op.progress / 0.9f);
            }

            if (totalProgress / sceneToLoad.Count >= 1f)
                break;

            yield return null;
        }

        yield return StartCoroutine(fadeController.FadeIn());

        foreach (AsyncOperation op in sceneToLoad)
        {
            op.allowSceneActivation = true;
        }

        while (!SceneManager.GetSceneByName(levelScene).isLoaded ||
               !SceneManager.GetSceneByName(persistentGameplay).isLoaded)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelScene));
        
        if (introScene.IsValid() && introScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(introScene);
        }
        else
        {
            Debug.LogWarning("Intro scene is not valid or already unloaded.");
        }

        yield return StartCoroutine(fadeController.FadeOut());
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isTyping)
        {
            typewriterEffect.Stop();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !isTyping && !isSceneLoading) 
        {
            ShowNextSentence();
        }
    }
}
