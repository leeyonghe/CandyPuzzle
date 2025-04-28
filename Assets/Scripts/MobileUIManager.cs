using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MobileUIManager : MonoBehaviour
{
    public static MobileUIManager Instance { get; private set; }

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button closeTutorialButton;
    [SerializeField] private GameObject[] tutorialPages;
    [SerializeField] private Button nextTutorialButton;
    [SerializeField] private Button prevTutorialButton;

    private int currentTutorialPage = 0;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Set up button listeners
        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);
        homeButton.onClick.AddListener(GoToHome);
        tutorialButton.onClick.AddListener(ShowTutorial);
        closeTutorialButton.onClick.AddListener(CloseTutorial);
        nextTutorialButton.onClick.AddListener(NextTutorialPage);
        prevTutorialButton.onClick.AddListener(PreviousTutorialPage);

        // Initialize UI
        pausePanel.SetActive(false);
        tutorialPanel.SetActive(false);
        ShowTutorialPage(0);
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    private void GoToHome()
    {
        Time.timeScale = 1;
        // Load home scene or main menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        ShowTutorialPage(0);
    }

    private void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    private void NextTutorialPage()
    {
        if (currentTutorialPage < tutorialPages.Length - 1)
        {
            ShowTutorialPage(currentTutorialPage + 1);
        }
    }

    private void PreviousTutorialPage()
    {
        if (currentTutorialPage > 0)
        {
            ShowTutorialPage(currentTutorialPage - 1);
        }
    }

    private void ShowTutorialPage(int pageIndex)
    {
        currentTutorialPage = pageIndex;
        for (int i = 0; i < tutorialPages.Length; i++)
        {
            tutorialPages[i].SetActive(i == pageIndex);
        }

        // Update button states
        prevTutorialButton.interactable = pageIndex > 0;
        nextTutorialButton.interactable = pageIndex < tutorialPages.Length - 1;
    }

    public bool IsPaused()
    {
        return isPaused;
    }
} 