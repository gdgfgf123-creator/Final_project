using UnityEngine;

public class InputController : MonoBehaviour
{
    [Header("Movement Input")]
    [SerializeField] private KeyCode moveUp = KeyCode.W;
    [SerializeField] private KeyCode moveDown = KeyCode.S;
    [SerializeField] private KeyCode moveLeft = KeyCode.A;
    [SerializeField] private KeyCode moveRight = KeyCode.D;

    [Header("Wind Sword")]
    [SerializeField] private KeyCode windSwordKey = KeyCode.E;
    [SerializeField] private WindSword windSword;

    [Header("Grave Shield")]
    [SerializeField] private KeyCode graveShieldKey = KeyCode.Q;
    [SerializeField] private GraveShield graveShield;

    [Header("Flame Bow")]
    [SerializeField] private KeyCode flameBowKey = KeyCode.R;
    [SerializeField] private FlameBow flameBow;

    [SerializeField] private Player player;

    private void Awake()
    {
        if (player == null)
            player = GetComponent<Player>();

        if (windSword == null)
            windSword = GetComponent<WindSword>();

        if (graveShield == null)
            graveShield = GetComponent<GraveShield>();

        if (flameBow == null)
            flameBow = GetComponent<FlameBow>();
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        ReadMovementInput();
        ReadAttackInput();
        ReadPowerInput();
    }

    public void ReadMovementInput()
    {
        Vector2 direction = Vector2.zero;

        if (Input.GetKey(moveUp)) direction.y += 1;
        if (Input.GetKey(moveDown)) direction.y -= 1;
        if (Input.GetKey(moveLeft)) direction.x -= 1;
        if (Input.GetKey(moveRight)) direction.x += 1;

        if (player != null)
            player.Move(direction.normalized);
    }

    public void ReadAttackInput()
    {
        if (Input.GetMouseButton(0) && player != null)
            player.BasicAttack();
    }

    public void ReadPowerInput()
    {
        ReadPower(windSwordKey, windSword);
        ReadPower(graveShieldKey, graveShield);
        ReadPower(flameBowKey, flameBow);
    }

    private void ReadPower(KeyCode key, Power power)
    {
        if (Input.GetKeyDown(key) && power != null)
            power.Use();
    }
}