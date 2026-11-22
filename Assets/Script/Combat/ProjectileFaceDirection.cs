using UnityEngine;

public class ProjectileFaceDirection : MonoBehaviour
{
    [SerializeField] private Transform visual;
    [SerializeField] private float angleOffset = -90f;

    private Vector2 lastPosition;

    private void OnEnable()
    {
        lastPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector2 currentPosition = transform.position;
        Vector2 direction = currentPosition - lastPosition;

        if (direction.sqrMagnitude > 0.000001f)
            FaceDirection(direction);

        lastPosition = currentPosition;
    }

    public void SetDirection(Vector2 direction)
    {
        FaceDirection(direction);
    }

    public void FaceDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.000001f)
            return;

        Transform target = visual != null ? visual : transform;

        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        target.rotation = Quaternion.Euler(
            0f,
            0f,
            angle + angleOffset
        );
    }
}