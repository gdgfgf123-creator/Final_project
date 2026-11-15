using UnityEngine;

public class RewardScoreSystem : MonoBehaviour
{
    [SerializeField, Min(0)] private int scoreReward = 100;

    public void GiveReward()
    {
        ScoreTracker tracker = ScoreTracker.Instance;

        if (tracker != null)
            tracker.AddScore(scoreReward);
    }
}