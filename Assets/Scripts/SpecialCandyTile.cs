using UnityEngine;
using UnityEngine.EventSystems;

public class SpecialCandyTile : CandyTile
{
    [SerializeField] private Sprite specialSprite;
    [SerializeField] private float explosionRadius = 1.5f;

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
        Explode();
    }

    private void Explode()
    {
        // Get all candies within explosion radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        
        foreach (Collider2D collider in colliders)
        {
            CandyTile candy = collider.GetComponent<CandyTile>();
            if (candy != null && candy != this)
            {
                candy.SetType(CandyType.None);
            }
        }

        // Add score for explosion
        UIManager.Instance.AddScore(50);
        LevelManager.Instance.AddSpecialCandy();
        
        // Destroy this special candy
        SetType(CandyType.None);
    }

    public void SetSpecialType()
    {
        spriteRenderer.sprite = specialSprite;
        type = CandyType.Purple; // Use Purple as special type
    }
} 