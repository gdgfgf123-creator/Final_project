using UnityEngine;

public class RewardScoreSystem : MonoBehaviour
{
    [SerializeField, Min(0)] private int scoreReward = 100;

    public void GiveReward()
    {
        if (ScoreTracker.Instance != null)
            ScoreTracker.Instance.AddKillScore(scoreReward);
    }
}