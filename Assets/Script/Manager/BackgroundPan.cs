using UnityEngine;

public class BackgroundPan : MonoBehaviour
{
    [Header("Range")]
    [SerializeField, Min(0f)] private float horizontalRange = 0.25f;
    [SerializeField, Min(0f)] private float verticalRange = 0.15f;

    [Header("Speed")]
    [SerializeField, Min(0f)] private float horizontalSpeed = 0.15f;
    [SerializeField, Min(0f)] private float verticalSpeed = 0.12f;

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float x = Mathf.Sin(Time.time * horizontalSpeed) * horizontalRange;
        float y = Mathf.Sin(Time.time * verticalSpeed) * verticalRange;

        transform.position = startPosition + new Vector3(x, y, 0f);
    }
}