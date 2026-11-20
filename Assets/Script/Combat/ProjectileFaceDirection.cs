using UnityEngine;

public class ProjectileFaceDirection : MonoBehaviour
{
    [SerializeField] private Transform visual;
    [SerializeField] private float angleOffset = -90f;

    public void SetDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
            return;

        if (visual == null)
            visual = transform;

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg
            + angleOffset;

        visual.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}