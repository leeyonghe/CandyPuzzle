using UnityEngine;
using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour
{
    private CandyTile selectedCandy;
    private Vector2 touchStartPosition;
    private float minSwipeDistance = 50f;
    private bool isDragging = false;

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Check if touch is over UI
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    HandleTouchBegan(touch.position);
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                        HandleTouchMoved(touch.position);
                    break;

                case TouchPhase.Ended:
                    if (isDragging)
                        HandleTouchEnded(touch.position);
                    break;
            }
        }
    }

    private void HandleTouchBegan(Vector2 touchPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touchPosition), Vector2.zero);
        if (hit.collider != null)
        {
            selectedCandy = hit.collider.GetComponent<CandyTile>();
            if (selectedCandy != null)
            {
                touchStartPosition = touchPosition;
                isDragging = true;
                selectedCandy.OnPointerDown(new PointerEventData(EventSystem.current));
            }
        }
    }

    private void HandleTouchMoved(Vector2 touchPosition)
    {
        Vector2 direction = touchPosition - touchStartPosition;
        if (direction.magnitude > minSwipeDistance)
        {
            HandleTouchEnded(touchPosition);
        }
    }

    private void HandleTouchEnded(Vector2 touchPosition)
    {
        if (selectedCandy != null)
        {
            Vector2 direction = touchPosition - touchStartPosition;
            if (direction.magnitude > minSwipeDistance)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                CandyTile neighbor = GetNeighborCandy(angle);
                
                if (neighbor != null)
                {
                    GameManager.Instance.SwapCandies(selectedCandy, neighbor);
                }
            }
            selectedCandy.OnPointerUp(new PointerEventData(EventSystem.current));
        }
        
        isDragging = false;
        selectedCandy = null;
    }

    private CandyTile GetNeighborCandy(float angle)
    {
        if (selectedCandy == null) return null;

        int x = selectedCandy.X;
        int y = selectedCandy.Y;

        if (Mathf.Abs(angle) < 45f) // Right
        {
            if (x < GameManager.Instance.boardWidth - 1)
                return GameManager.Instance.GetTile(x + 1, y);
        }
        else if (Mathf.Abs(angle - 180f) < 45f) // Left
        {
            if (x > 0)
                return GameManager.Instance.GetTile(x - 1, y);
        }
        else if (Mathf.Abs(angle - 90f) < 45f) // Up
        {
            if (y < GameManager.Instance.boardHeight - 1)
                return GameManager.Instance.GetTile(x, y + 1);
        }
        else if (Mathf.Abs(angle + 90f) < 45f) // Down
        {
            if (y > 0)
                return GameManager.Instance.GetTile(x, y - 1);
        }

        return null;
    }
} 