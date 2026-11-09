using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreNumberUI : MonoBehaviour
{
    [Header("Number Sprites 0-9")]
    [SerializeField] private Sprite[] numberSprites = new Sprite[10];

    [Header("UI")]
    [SerializeField] private Image digitPrefab;
    [SerializeField] private Transform digitContainer;

    private readonly List<Image> digits = new List<Image>();
    private int displayedScore = -1;

    private void Update()
    {
        if (ScoreTracker.Instance == null)
            return;

        int score = ScoreTracker.Instance.Score;

        if (score != displayedScore)
            DisplayScore(score);
    }

    private void DisplayScore(int score)
    {
        if (digitPrefab == null || digitContainer == null ||
            numberSprites == null || numberSprites.Length != 10)
            return;

        string scoreText = score.ToString();

        while (digits.Count < scoreText.Length)
        {
            Image digit = Instantiate(digitPrefab, digitContainer);
            digit.preserveAspect = true;
            digit.raycastTarget = false;

            digits.Add(digit);
        }

        for (int i = 0; i < digits.Count; i++)
        {
            bool visible = i < scoreText.Length;
            digits[i].gameObject.SetActive(visible);

            if (visible)
            {
                int number = scoreText[i] - '0';
                digits[i].sprite = numberSprites[number];
            }
        }

        displayedScore = score;
    }
}