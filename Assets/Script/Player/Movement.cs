using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(
            rb.position +
            moveDirection * moveSpeed * Time.fixedDeltaTime
        );
    }

    public void HandleWASD() { }

    public void MovePlayer(Vector2 direction)
    {
        moveDirection = Vector2.ClampMagnitude(direction, 1f);
    }

    private void OnDisable()
    {
        moveDirection = Vector2.zero;
    }
}