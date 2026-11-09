using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int CurrentHP => currentHP;

    [Header("Health")]
    [SerializeField] private int currentHP = 3;
    [SerializeField] private int maxHP = 10;
    [SerializeField, Min(0f)] private float invincibleTime = 1f;

    [Header("Attack")]
    public int basicAttackDamage = 10;
    [SerializeField] private int selectedPower = 1;
    [SerializeField] private BasicAttack basicAttack;

    [Header("Movement")]
    [SerializeField] private Movement movement;

    [Header("Invincibility Effect")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField, Min(0.02f)] private float blinkInterval = 0.1f;

    private float nextDamageTime;
    private Coroutine blinkCoroutine;
    private Color normalColor;

    private void Awake()
    {
        if (playerSprite == null)
            playerSprite = GetComponent<SpriteRenderer>();

        if (playerSprite != null)
            normalColor = playerSprite.color;
    }

    public void Move(Vector2 direction)
    {
        if (movement != null)
            movement.MovePlayer(direction);
    }

    public void BasicAttack()
    {
        if (basicAttack != null)
            basicAttack.OnClick();
    }

    public void SwitchPower(int slot)
    {
    }

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
            return;
        }

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        if (playerSprite != null && invincibleTime > 0f)
            blinkCoroutine = StartCoroutine(BlinkWhileInvincible());
    }

    private IEnumerator BlinkWhileInvincible()
    {
        Color fadedColor = normalColor;
        fadedColor.a *= 0.25f;

        bool faded = true;
        float interval = Mathf.Max(0.02f, blinkInterval);

        while (Time.time < nextDamageTime)
        {
            playerSprite.color = faded ? fadedColor : normalColor;
            faded = !faded;

            float remainingTime = nextDamageTime - Time.time;

            yield return new WaitForSeconds(
                Mathf.Min(interval, remainingTime)
            );
        }

        playerSprite.color = normalColor;
        blinkCoroutine = null;
    }

    public void Heal(int amount)
    {
        if (currentHP <= 0 || amount <= 0)
            return;

        currentHP = Mathf.Min(maxHP, currentHP + amount);

        Debug.Log("Player HP: " + currentHP);
    }

    private void OnDisable()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        if (playerSprite != null)
            playerSprite.color = normalColor;
    }
}