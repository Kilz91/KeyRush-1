using UnityEngine;

public class ShrinkingWall : MonoBehaviour
{
    [Header("Vitesse du mur")]
    public float speed = 0.1f;
    public float expandSpeed = 0.2f;

    [Header("Côté du mur")]
    public bool fromTop = false;
    public bool fromBottom = false;
    public bool fromLeft = false;
    public bool fromRight = false;

    [Header("Game Over")]
    public GameOverManager gameOverManager; // assign the GameOverManager in the Inspector

    private Vector3 moveDirection;

    void Start()
    {
        if (fromTop)
            moveDirection = Vector3.down;
        else if (fromBottom)
            moveDirection = Vector3.up;
        else if (fromLeft)
            moveDirection = Vector3.right;
        else if (fromRight)
            moveDirection = Vector3.left;
        else
            moveDirection = Vector3.zero;
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;

        Vector3 newScale = transform.localScale;

        if (fromTop || fromBottom)
            newScale.y += expandSpeed * Time.deltaTime;
        else if (fromLeft || fromRight)
            newScale.x += expandSpeed * Time.deltaTime;

        transform.localScale = newScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandlePlayerHit(collision.gameObject, "OnCollisionEnter2D");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandlePlayerHit(other.gameObject, "OnTriggerEnter2D");
    }

    private void HandlePlayerHit(GameObject hitObject, string source)
    {
        if (!hitObject.CompareTag("Player"))
            return;

        Debug.Log($"ShrinkingWall hit player via {source}");

        // delegate to manager (keeps ShrinkingWall simple)
        if (gameOverManager != null)
        {
            gameOverManager.TriggerGameOver(hitObject);
        }
        else if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.TriggerGameOver(hitObject);
        }
        else
        {
            Debug.LogWarning("ShrinkingWall: no GameOverManager assigned and no Instance found.");
        }
    }
}