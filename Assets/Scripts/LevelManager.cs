using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public int targetScore;
    public float timeLimit;
    public int requiredMatches;
    public int specialCandyCount;
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private List<LevelData> levels;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI targetScoreText;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private TextMeshProUGUI levelCompleteText;
    [SerializeField] private Button nextLevelButton;

    private int currentLevel = 0;
    private int currentMatches = 0;
    private int currentSpecialCandies = 0;

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
        LoadLevel(0);
        nextLevelButton.onClick.AddListener(LoadNextLevel);
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= levels.Count)
        {
            // Game completed
            levelCompleteText.text = "Congratulations! You've completed all levels!";
            levelCompletePanel.SetActive(true);
            return;
        }

        currentLevel = levelIndex;
        currentMatches = 0;
        currentSpecialCandies = 0;

        LevelData level = levels[levelIndex];
        UIManager.Instance.SetTimeLimit(level.timeLimit);
        
        UpdateUI();
        GameManager.Instance.RestartGame();
    }

    public void AddMatch()
    {
        currentMatches++;
        CheckLevelCompletion();
    }

    public void AddSpecialCandy()
    {
        currentSpecialCandies++;
        CheckLevelCompletion();
    }

    private void CheckLevelCompletion()
    {
        LevelData level = levels[currentLevel];
        if (currentMatches >= level.requiredMatches && 
            currentSpecialCandies >= level.specialCandyCount &&
            UIManager.Instance.GetScore() >= level.targetScore)
        {
            levelCompleteText.text = $"Level {currentLevel + 1} Complete!\nScore: {UIManager.Instance.GetScore()}";
            levelCompletePanel.SetActive(true);
        }
    }

    private void LoadNextLevel()
    {
        levelCompletePanel.SetActive(false);
        LoadLevel(currentLevel + 1);
    }

    private void UpdateUI()
    {
        LevelData level = levels[currentLevel];
        levelText.text = $"Level: {currentLevel + 1}";
        targetScoreText.text = $"Target: {level.targetScore}\nMatches: {currentMatches}/{level.requiredMatches}\nSpecial: {currentSpecialCandies}/{level.specialCandyCount}";
    }
} 