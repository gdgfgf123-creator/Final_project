using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D rb;
    private Vector2 direction = Vector2.up;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(
            rb.position +
            direction * speed * Time.fixedDeltaTime
        );
    }
}