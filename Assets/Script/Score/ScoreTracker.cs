using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    public static ScoreTracker Instance { get; private set; }

    [SerializeField, Min(0.1f)] private float comboDuration = 3f;

    public int Score { get; private set; }
    public int Combo { get; private set; }
    public float ScoreMultiplier => 1f + (Combo / 100) * 0.25f;

    private float comboExpiresAt;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        ExpireCombo();
    }

    public void RegisterHit()
    {
        ExpireCombo();
        Combo++;
        comboExpiresAt = Time.time + comboDuration;
    }

    public void AddKillScore(int baseScore)
    {
        ExpireCombo();
        AddScore(Mathf.RoundToInt(baseScore * ScoreMultiplier));
    }

    public void AddScore(int amount)
    {
        if (amount > 0)
            Score += amount;
    }

    private void ExpireCombo()
    {
        if (Combo > 0 && Time.time >= comboExpiresAt)
            Combo = 0;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}