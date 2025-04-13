using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotebookManager : MonoBehaviour
{
    public static NotebookManager Instance;

    public GameObject notebookIcon; 
    public GameObject notebookPanel; 
    public TextMeshProUGUI notebookContent; 
    public Button prevPageButton; 
    public Button nextPageButton; 
    public int maxCharactersPerPage = 500; 

    private Dictionary<string, string> uniqueEntries = new Dictionary<string, string>();
    private List<string> entries = new List<string>();
    private List<string> pages = new List<string>();
    private int currentPage = 0;
    private List<string> notebookEntries = new List<string>(); 

    void Awake()
    {
        Time.timeScale = 1;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        notebookPanel.SetActive(false);
        
        notebookIcon.GetComponent<Button>().onClick.AddListener(ToggleNotebook);
        
        prevPageButton.onClick.AddListener(PreviousPage);
        nextPageButton.onClick.AddListener(NextPage);

        UpdatePageButtons();
    }

    public void ToggleNotebook()
    {
        bool isActive = notebookPanel.activeSelf;
        notebookPanel.SetActive(!isActive);
        Time.timeScale = 0;
    }

    public void AddEntry(string key, string entry)
    {
        if (!uniqueEntries.ContainsKey(key))
        {
            uniqueEntries[key] = entry;
            entries.Add(entry);
            UpdatePages();
            currentPage = 0; 
            UpdateNotebookContent();
        }
    }

    private void UpdatePages()
    {
        pages.Clear();
        string currentPageContent = "";

        foreach (var entry in entries)
        {
            if (currentPageContent.Length + entry.Length > maxCharactersPerPage)
            {
                pages.Add(currentPageContent);
                currentPageContent = "";
            }
            currentPageContent += entry + "\n\n";
        }

        if (!string.IsNullOrEmpty(currentPageContent))
        {
            pages.Add(currentPageContent);
        }

        currentPage = Mathf.Clamp(currentPage, 0, pages.Count - 1);
    }

    private void UpdateNotebookContent()
    {
        if (pages.Count > 0)
        {
            notebookContent.text = pages[currentPage];
        }
        else
        {
            notebookContent.text = "";
        }
        UpdatePageButtons();
    }

    private void UpdatePageButtons()
    {
        prevPageButton.interactable = currentPage > 0;
        nextPageButton.interactable = currentPage < pages.Count - 1;
    }

    private void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateNotebookContent();
        }
    }

    private void NextPage()
    {
        if (currentPage < pages.Count - 1)
        {
            currentPage++;
            UpdateNotebookContent();
        }
    }
    
    public List<string> GetNotebookEntries()
    {
        return notebookEntries;
    }

    public void SetNotebookEntries(List<string> entries)
    {
        notebookEntries = entries;
        UpdateNotebookDisplay();
    }

    private void UpdateNotebookDisplay()
    {
        entries = new List<string>(notebookEntries);
        UpdatePages();
        UpdateNotebookContent();
    }
}