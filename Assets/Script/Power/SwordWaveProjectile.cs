using UnityEngine;

public class SwordWaveProjectile : Bullet
{
    [SerializeField] private float spriteAngleOffset;

    private void Reset()
    {
        speed = 12f;
        lifeTime = 3f;
        damage = 20;
    }

    public override void SetDirection(Vector2 shootDirection)
    {
        base.SetDirection(shootDirection);

        RotateToDirection(spriteAngleOffset);
    }
}