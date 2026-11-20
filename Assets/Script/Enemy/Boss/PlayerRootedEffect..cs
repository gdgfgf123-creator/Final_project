using UnityEngine;

public class PlayerRootedEffect : MonoBehaviour
{
    private Movement movement;
    private float lockedUntil;
    private bool movementWasEnabled;

    private void Awake()
    {
        movement = GetComponent<Movement>();
    }

    public void LockFor(float duration)
    {
        if (movement == null)
            movement = GetComponent<Movement>();

        if (movement == null || duration <= 0f)
            return;

        if (Time.time >= lockedUntil)
            movementWasEnabled = movement.enabled;

        lockedUntil = Mathf.Max(lockedUntil, Time.time + duration);
        movement.enabled = false;
    }

    private void Update()
    {
        if (movement == null || lockedUntil <= 0f || Time.time < lockedUntil)
            return;

        movement.enabled = movementWasEnabled;
        lockedUntil = 0f;
    }
}