using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int currentHP = 3;
    [SerializeField] private int maxHP = 10;
    [SerializeField] public int basicAttackDamage = 10;
    [SerializeField] private int selectedPower = 1;
    [SerializeField] private float invincibleTime = 1f;

    private float nextDamageTime;
    [SerializeField] private Movement movement;

    public void Move(Vector2 direction)
    {
        if (movement != null)
            movement.MovePlayer(direction);
    }

    [SerializeField] private BasicAttack basicAttack;
    public void BasicAttack()
    {
        if (basicAttack != null)
            basicAttack.OnClick();
    }
    public void SwitchPower(int slot) { }

    public void TakeDamage(int amount)
    {
        if (currentHP <= 0 || amount <= 0)
            return;

        if (Time.time < nextDamageTime)
            return;

        nextDamageTime = Time.time + invincibleTime;

        currentHP = Mathf.Max(0, currentHP - amount);

        Debug.Log("Player HP: " + currentHP);

        if (currentHP <= 0)
        {
            Debug.Log("Game Over");
            gameObject.SetActive(false);
        }
    }

    public void Heal(int amount)
    {
        if (currentHP <= 0 || amount <= 0)
            return;

        currentHP = Mathf.Min(maxHP, currentHP + amount);

        Debug.Log("Player HP: " + currentHP);
    }
}