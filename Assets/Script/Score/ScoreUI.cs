using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    private int displayedScore = -1;

    private void LateUpdate()
    {
        ScoreTracker tracker = ScoreTracker.Instance;
        int score = tracker != null ? tracker.Score : 0;

        if (scoreText == null || displayedScore == score)
            return;

        displayedScore = score;
        scoreText.text = $"Score: {score}";
    }
}