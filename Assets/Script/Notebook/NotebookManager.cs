using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotebookManager : MonoBehaviour
{
    public GameObject notebookIcon; // Иконка блокнота
    public GameObject notebookPanel; // Панель блокнота
    public TextMeshProUGUI notebookContent; // Текстовое поле для содержания блокнота
    public Button prevPageButton; // Кнопка для перехода к предыдущей странице
    public Button nextPageButton; // Кнопка для перехода к следующей странице
    public int maxCharactersPerPage = 500; // Максимальное количество символов на странице

    private Dictionary<string, string> uniqueEntries = new Dictionary<string, string>();
    private List<string> entries = new List<string>();
    private List<string> pages = new List<string>();
    private int currentPage = 0;
    private List<string> notebookEntries = new List<string>(); // Добавляем эту переменную

    void Start()
    {
        // Скрываем панель блокнота при старте
        notebookPanel.SetActive(false);

        // Назначаем обработчик нажатия на иконку блокнота
        notebookIcon.GetComponent<Button>().onClick.AddListener(ToggleNotebook);
        
        // Назначаем обработчики для кнопок перелистывания страниц
        prevPageButton.onClick.AddListener(PreviousPage);
        nextPageButton.onClick.AddListener(NextPage);

        UpdatePageButtons();
    }

    public void ToggleNotebook()
    {
        bool isActive = notebookPanel.activeSelf;
        notebookPanel.SetActive(!isActive);
    }

    public void AddEntry(string key, string entry)
    {
        if (!uniqueEntries.ContainsKey(key))
        {
            uniqueEntries[key] = entry;
            entries.Add(entry);
            UpdatePages();
            currentPage = 0; // Устанавливаем текущую страницу на первую
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