using UnityEngine;

public class Datura : Boss
{
    [SerializeField] private Player player;

    protected override Vector2 GetShootDirection()
    {
        if (player == null)
            player = FindFirstObjectByType<Player>();

        if (player == null || !player.isActiveAndEnabled || firePoint == null)
            return Vector2.down;

        return (
            (Vector2)player.transform.position -
            (Vector2)firePoint.position
        ).normalized;
    }

    protected override void Die()
    {
        Debug.Log("Datura defeated!");
        base.Die();
    }
}