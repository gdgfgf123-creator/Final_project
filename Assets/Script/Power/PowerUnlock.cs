using UnityEngine;

public class PowerUnlock : MonoBehaviour
{
    [SerializeField] private Power[] powers;

    private void Update()
    {
        CheckPowers();
    }

    public void CheckPowers()
    {
        if (ScoreTracker.Instance == null)
            return;

        int score = ScoreTracker.Instance.Score;

        foreach (Power power in powers)
        {
            if (power != null)
                power.CheckUnlock(score);
        }
    }
}