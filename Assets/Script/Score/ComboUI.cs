using TMPro;
using UnityEngine;

public class ComboUI : MonoBehaviour
{
    [SerializeField] private TMP_Text comboText;

    private int displayedCombo = -1;

    private void LateUpdate()
    {
        ScoreTracker tracker = ScoreTracker.Instance;

        if (tracker == null || comboText == null)
            return;

        int combo = tracker.Combo;

        if (combo == displayedCombo)
            return;

        displayedCombo = combo;
        comboText.text = combo > 0
            ? $"Combo {combo}  x{tracker.ScoreMultiplier:0.00}"
            : string.Empty;
    }
}