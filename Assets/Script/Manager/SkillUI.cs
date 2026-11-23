using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUI : MonoBehaviour
{
    [Header("Power")]
    [SerializeField] private Power power;
    [SerializeField] private string keyName = "E";

    [Header("UI")]
    [SerializeField] private Image skillIcon;
    [SerializeField] private Image cooldownFill;
    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text keyText;

    [Header("Colors")]
    [SerializeField]
    private Color lockedColor =
        new Color(0.3f, 0.3f, 0.3f, 1f);

    [SerializeField] private Color readyColor = Color.white;

    private void Update()
    {
        if (power == null)
            return;

        if (keyText != null)
            keyText.text = keyName;

        if (!power.IsUnlocked)
        {
            ShowLocked();
        }
        else if (power.IsOnCooldown)
        {
            ShowCooldown();
        }
        else
        {
            ShowReady();
        }
    }

    private void ShowLocked()
    {
        skillIcon.color = lockedColor;

        cooldownFill.fillAmount = 0f;
        cooldownText.text = "";

        int score = ScoreTracker.Instance != null
            ? ScoreTracker.Instance.Score
            : 0;

        statusText.text =
            $"{score:N0}/{power.ScoreRequirement:N0}";
    }

    private void ShowCooldown()
    {
        skillIcon.color = readyColor;

        cooldownFill.fillAmount = power.CooldownPercent;

        cooldownText.text =
            Mathf.CeilToInt(power.CooldownRemaining).ToString();

        statusText.text = "Cooldown";
    }

    private void ShowReady()
    {
        skillIcon.color = readyColor;

        cooldownFill.fillAmount = 0f;
        cooldownText.text = "";

        statusText.text = "Ready";
    }
}