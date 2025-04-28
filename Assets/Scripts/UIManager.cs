using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private GameObject comboEffect;
    [SerializeField] private AudioClip comboSound;

    private int currentScore = 0;
    private int currentMoves = 0;
    private float timeLeft = 60f;
    private bool isGameOver = false;
    private int currentCombo = 0;
    private float comboResetTime = 2f;
    private float lastMatchTime;
    private AudioSource audioSource;

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

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        UpdateUI();
        restartButton.onClick.AddListener(RestartGame);
    }

    private void Update()
    {
        if (!isGameOver)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timeLeft = 0;
                GameOver();
            }

            // Check combo reset
            if (Time.time - lastMatchTime > comboResetTime && currentCombo > 0)
            {
                currentCombo = 0;
                comboText.gameObject.SetActive(false);
            }

            UpdateUI();
        }
    }

    public void AddScore(int points)
    {
        currentScore += points * (1 + currentCombo / 10);
        UpdateUI();
    }

    public void AddMove()
    {
        currentMoves++;
        UpdateUI();
    }

    public void AddCombo()
    {
        currentCombo++;
        lastMatchTime = Time.time;
        
        // Show combo effect
        comboText.gameObject.SetActive(true);
        comboText.text = $"Combo x{1 + currentCombo / 10}";
        Instantiate(comboEffect, comboText.transform.position, Quaternion.identity);
        audioSource.PlayOneShot(comboSound);
        
        UpdateUI();
    }

    public void ResetCombo()
    {
        currentCombo = 0;
        UpdateUI();
    }

    public void SetTimeLimit(float time)
    {
        timeLeft = time;
        UpdateUI();
    }

    public int GetScore()
    {
        return currentScore;
    }

    private void UpdateUI()
    {
        scoreText.text = $"Score: {currentScore}";
        movesText.text = $"Moves: {currentMoves}";
        timeText.text = $"Time: {Mathf.CeilToInt(timeLeft)}";
        comboText.text = $"Combo: x{1 + currentCombo / 10}";
    }

    private void GameOver()
    {
        isGameOver = true;
        finalScoreText.text = $"Final Score: {currentScore}";
        gameOverPanel.SetActive(true);
    }

    private void RestartGame()
    {
        currentScore = 0;
        currentMoves = 0;
        currentCombo = 0;
        isGameOver = false;
        gameOverPanel.SetActive(false);
        UpdateUI();
        GameManagerTemp.Instance.RestartGame();
    }
} 