using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public int boardWidth = 8;
    [SerializeField] public int boardHeight = 8;
    [SerializeField] private GameObject candyPrefab;
    [SerializeField] private float candySize = 1f;
    [SerializeField] private GameObject specialCandyPrefab;
    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip swapSound;
    [SerializeField] private ParticleSystem matchEffect;

    private CandyTile[,] board;
    private bool isSwapping = false;
    private AudioSource audioSource;

    private void Awake()
    {
        Debug.Log("GameManager Awake");
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("GameManager Instance created");
        }
        else
        {
            Debug.Log("Destroying duplicate GameManager");
            Destroy(gameObject);
        }

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        Debug.Log("GameManager Start");
        if (candyPrefab == null)
        {
            Debug.LogError("Candy Prefab is not assigned!");
            return;
        }

        if (specialCandyPrefab == null)
        {
            Debug.LogError("Special Candy Prefab is not assigned!");
            return;
        }

        // Create a parent object for all candies
        GameObject boardParent = new GameObject("Board");
        boardParent.transform.position = Vector3.zero;

        InitializeBoard(boardParent.transform);
        FillBoard();
    }

    private void Update()
    {
        Debug.Log("GameManager Update");
    }

    public void RestartGame()
    {
        ClearBoard();
        InitializeBoard(null);
        FillBoard();
    }

    private void ClearBoard()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (board[x, y] != null)
                {
                    Destroy(board[x, y].gameObject);
                }
            }
        }
    }

    private void InitializeBoard(Transform parent)
    {
        Debug.Log("Initializing board");
        board = new CandyTile[boardWidth, boardHeight];

        // Calculate the starting position to center the board
        float startX = -(boardWidth * candySize) / 2f + candySize / 2f;
        float startY = -(boardHeight * candySize) / 2f + candySize / 2f;

        Debug.Log($"Board start position: ({startX}, {startY})");

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                Vector3 position = new Vector3(
                    startX + x * candySize,
                    startY + y * candySize,
                    0
                );

                Debug.Log($"Creating candy at position: {position}");
                
                GameObject candy = Instantiate(candyPrefab, position, Quaternion.identity, parent);
                if (candy == null)
                {
                    Debug.LogError($"Failed to instantiate candy at position ({x}, {y})");
                    continue;
                }
                
                board[x, y] = candy.GetComponent<CandyTile>();
                if (board[x, y] == null)
                {
                    Debug.LogError($"CandyTile component not found on instantiated candy at ({x}, {y})");
                    continue;
                }
                
                board[x, y].Initialize(x, y);
                Debug.Log($"Created candy at position ({x}, {y})");
            }
        }
    }

    private void FillBoard()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (board[x, y].Type == CandyType.None)
                {
                    board[x, y].SetRandomType();
                }
            }
        }
    }

    public void SwapCandies(CandyTile candy1, CandyTile candy2)
    {
        if (isSwapping) return;

        isSwapping = true;
        UIManager.Instance.AddMove();
        audioSource.PlayOneShot(swapSound);
        StartCoroutine(SwapCandiesCoroutine(candy1, candy2));
    }

    private System.Collections.IEnumerator SwapCandiesCoroutine(CandyTile candy1, CandyTile candy2)
    {
        // Swap positions
        Vector3 pos1 = candy1.transform.position;
        Vector3 pos2 = candy2.transform.position;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            candy1.transform.position = Vector3.Lerp(pos1, pos2, t);
            candy2.transform.position = Vector3.Lerp(pos2, pos1, t);
            yield return null;
        }

        // Swap in board array
        int x1 = candy1.X;
        int y1 = candy1.Y;
        int x2 = candy2.X;
        int y2 = candy2.Y;

        board[x1, y1] = candy2;
        board[x2, y2] = candy1;

        candy1.SetPosition(x2, y2);
        candy2.SetPosition(x1, y1);

        // Check for matches
        if (!CheckMatches())
        {
            // If no matches, swap back
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SwapCandiesCoroutine(candy1, candy2));
        }
        else
        {
            isSwapping = false;
            RemoveMatches();
        }
    }

    private bool CheckMatches()
    {
        bool hasMatches = false;
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (CheckMatchAt(x, y))
                {
                    hasMatches = true;
                }
            }
        }
        return hasMatches;
    }

    private bool CheckMatchAt(int x, int y)
    {
        CandyType type = board[x, y].Type;
        int horizontalMatches = 1;
        int verticalMatches = 1;

        // Check horizontal
        for (int i = x + 1; i < boardWidth; i++)
        {
            if (board[i, y].Type == type) horizontalMatches++;
            else break;
        }

        // Check vertical
        for (int i = y + 1; i < boardHeight; i++)
        {
            if (board[x, i].Type == type) verticalMatches++;
            else break;
        }

        return horizontalMatches >= 3 || verticalMatches >= 3;
    }

    private void RemoveMatches()
    {
        List<Vector2Int> matchedPositions = new List<Vector2Int>();
        int totalMatches = 0;

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (CheckMatchAt(x, y))
                {
                    matchedPositions.Add(new Vector2Int(x, y));
                    totalMatches++;
                }
            }
        }

        // Calculate score
        int score = totalMatches * 10;
        if (totalMatches >= 4)
        {
            score *= 2; // Bonus for 4 or more matches
            CreateSpecialCandy(matchedPositions[0]);
            LevelManager.Instance.AddSpecialCandy();
        }
        
        // Add combo
        UIManager.Instance.AddCombo();
        UIManager.Instance.AddScore(score);
        LevelManager.Instance.AddMatch();

        // Play effects
        audioSource.PlayOneShot(matchSound);
        foreach (Vector2Int pos in matchedPositions)
        {
            Instantiate(matchEffect, board[pos.x, pos.y].transform.position, Quaternion.identity);
            board[pos.x, pos.y].SetType(CandyType.None);
        }

        StartCoroutine(FillEmptySpaces());
    }

    private void CreateSpecialCandy(Vector2Int position)
    {
        GameObject specialCandy = Instantiate(specialCandyPrefab, 
            new Vector3(position.x * candySize, position.y * candySize, 0), 
            Quaternion.identity);
        board[position.x, position.y] = specialCandy.GetComponent<CandyTile>();
        board[position.x, position.y].Initialize(position.x, position.y);
    }

    private System.Collections.IEnumerator FillEmptySpaces()
    {
        yield return new WaitForSeconds(0.5f);
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (board[x, y].Type == CandyType.None)
                {
                    board[x, y].SetRandomType();
                }
            }
        }
        if (CheckMatches())
        {
            RemoveMatches();
        }
        else
        {
            isSwapping = false;
        }
    }

    public CandyTile GetTile(int x, int y)
    {
        if (x >= 0 && x < boardWidth && y >= 0 && y < boardHeight)
        {
            return board[x, y];
        }
        return null;
    }
} 