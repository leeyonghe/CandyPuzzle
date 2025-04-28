using UnityEngine;
using UnityEngine.EventSystems;

public enum CandyType
{
    None,
    Red,
    Blue,
    Green,
    Yellow,
    Purple
}

public class CandyTile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] protected Sprite[] candySprites;
    protected SpriteRenderer spriteRenderer;
    protected CandyType type;
    protected int x, y;
    protected Vector2 startPosition;
    protected bool isDragging = false;
    protected static CandyTile selectedCandy;

    public CandyType Type => type;
    public int X => x;
    public int Y => y;

    private void Awake()
    {
        Debug.Log($"CandyTile Awake - GameObject: {gameObject.name}");
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"SpriteRenderer component not found on {gameObject.name}! Adding one...");
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        else
        {
            Debug.Log($"SpriteRenderer found on {gameObject.name}");
        }

        if (candySprites == null)
        {
            Debug.LogError($"CandySprites array is null on {gameObject.name}");
        }
        else
        {
            Debug.Log($"CandySprites array length: {candySprites.Length} on {gameObject.name}");
            for (int i = 0; i < candySprites.Length; i++)
            {
                Debug.Log($"Sprite {i}: {(candySprites[i] == null ? "null" : candySprites[i].name)}");
            }
        }
    }

    public void Initialize(int x, int y)
    {
        Debug.Log($"Initializing candy at position ({x}, {y}) - GameObject: {gameObject.name}");
        this.x = x;
        this.y = y;
        SetRandomType();
    }

    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetType(CandyType newType)
    {
        Debug.Log($"Setting candy type to {newType} on {gameObject.name}");
        type = newType;
        if (type != CandyType.None)
        {
            if (candySprites == null)
            {
                Debug.LogError($"Candy sprites array is null on {gameObject.name}!");
                return;
            }
            
            if (candySprites.Length == 0)
            {
                Debug.LogError($"Candy sprites array is empty on {gameObject.name}!");
                return;
            }

            int spriteIndex = (int)type - 1;
            if (spriteIndex < 0 || spriteIndex >= candySprites.Length)
            {
                Debug.LogError($"Invalid sprite index: {spriteIndex} for type {type} on {gameObject.name}");
                return;
            }

            if (candySprites[spriteIndex] == null)
            {
                Debug.LogError($"Sprite at index {spriteIndex} is null on {gameObject.name}!");
                return;
            }

            spriteRenderer.sprite = candySprites[spriteIndex];
            spriteRenderer.enabled = true;
            Debug.Log($"Successfully set sprite for type {type} on {gameObject.name}");
        }
        else
        {
            spriteRenderer.enabled = false;
            Debug.Log($"Disabled sprite renderer for type None on {gameObject.name}");
        }
    }

    public void SetRandomType()
    {
        CandyType randomType = (CandyType)Random.Range(1, System.Enum.GetValues(typeof(CandyType)).Length);
        Debug.Log($"Setting random type: {randomType} on {gameObject.name}");
        SetType(randomType);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (selectedCandy == null)
        {
            selectedCandy = this;
            startPosition = transform.position;
            isDragging = true;
            Debug.Log($"Started dragging {gameObject.name}");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;
            if (selectedCandy == this)
            {
                selectedCandy = null;
                Debug.Log($"Stopped dragging {gameObject.name}");
            }
        }
    }
} 