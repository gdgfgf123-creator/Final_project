using UnityEngine;

public class BoneSpin : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 540f;

    private void Update()
    {
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }
}