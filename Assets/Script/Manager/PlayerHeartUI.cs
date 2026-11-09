using UnityEngine;

public class PlayerHeartUI : MonoBehaviour
{
    [SerializeField] private Player player;

    [Tooltip("ใส่หัวใจตามลำดับที่จะให้แสดง")]
    [SerializeField] private GameObject[] hearts;

    private int previousHP = -1;

    private void LateUpdate()
    {
        if (player == null)
            return;

        int hp = Mathf.Max(0, player.CurrentHP);

        if (hp == previousHP)
            return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                hearts[i].SetActive(i < hp);
        }

        previousHP = hp;
    }
}