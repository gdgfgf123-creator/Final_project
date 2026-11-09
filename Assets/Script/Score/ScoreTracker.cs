using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    public static ScoreTracker Instance { get; private set; }

    public int Score { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Score = 0;
    }

    public void AddScore(int amount)
    {
        if (amount <= 0)
            return;

        Score += amount;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}