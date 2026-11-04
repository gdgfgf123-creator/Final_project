using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int currentHP = 3;
    [SerializeField] private int maxHP = 10;
    [SerializeField] private int basicAttackDamage = 10;
    [SerializeField] private int selectedPower = 1;

    [SerializeField] private Movement movement;

    public void Move(Vector2 direction)
    {
        if (movement != null)
            movement.MovePlayer(direction);
    }

    public void BasicAttack() { }

    public void SwitchPower(int slot) { }

    public void TakeDamage(int amount) { }

    public void Heal(int amount) { }
}