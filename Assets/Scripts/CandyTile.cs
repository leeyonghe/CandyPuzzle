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
        Debug.Log("CandyTile Awake");
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found!");
        }
    }

    public void Initialize(int x, int y)
    {
        Debug.Log($"Initializing candy at position ({x}, {y})");
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
        Debug.Log($"Setting candy type to {newType}");
        type = newType;
        if (type != CandyType.None)
        {
            if (candySprites == null || candySprites.Length == 0)
            {
                Debug.LogError("Candy sprites array is not set up!");
                return;
            }
            spriteRenderer.sprite = candySprites[(int)type - 1];
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }

    public void SetRandomType()
    {
        Debug.Log("Setting random candy type");
        if (candySprites == null || candySprites.Length == 0)
        {
            Debug.LogError("Candy sprites array is not set up!");
            return;
        }
        CandyType randomType = (CandyType)Random.Range(1, System.Enum.GetValues(typeof(CandyType)).Length);
        SetType(randomType);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPosition = eventData.position;
        isDragging = true;
        selectedCandy = this;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
        Vector2 direction = (eventData.position - startPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        CandyTile neighbor = null;

        if (Mathf.Abs(angle) < 45f) // Right
        {
            if (x < GameManagerTemp.Instance.boardWidth - 1)
                neighbor = GameManagerTemp.Instance.GetTile(x + 1, y);
        }
        else if (Mathf.Abs(angle - 180f) < 45f) // Left
        {
            if (x > 0)
                neighbor = GameManagerTemp.Instance.GetTile(x - 1, y);
        }
        else if (Mathf.Abs(angle - 90f) < 45f) // Up
        {
            if (y < GameManagerTemp.Instance.boardHeight - 1)
                neighbor = GameManagerTemp.Instance.GetTile(x, y + 1);
        }
        else if (Mathf.Abs(angle + 90f) < 45f) // Down
        {
            if (y > 0)
                neighbor = GameManagerTemp.Instance.GetTile(x, y - 1);
        }

        if (neighbor != null)
        {
            GameManagerTemp.Instance.SwapCandies(this, neighbor);
        }
    }
} 