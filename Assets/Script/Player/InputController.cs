using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private KeyCode moveUp = KeyCode.W;
    [SerializeField] private KeyCode moveDown = KeyCode.S;
    [SerializeField] private KeyCode moveLeft = KeyCode.A;
    [SerializeField] private KeyCode moveRight = KeyCode.D;

    [SerializeField] private Player player;

    private void Update()
    {
        ReadMovementInput();
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

    public void ReadAttackInput() { }

    public void ReadPowerInput() { }
}