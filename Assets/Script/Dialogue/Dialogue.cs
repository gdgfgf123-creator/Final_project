using UnityEngine;
using UnityEngine.UI;

public abstract class Dialogue : MonoBehaviour
{
    [Header("Shared Dialogue UI")]
    [SerializeField] protected GameObject dialoguePanel;
    [SerializeField] protected Text characterNameText;
    [SerializeField] protected Button nextButton;
    [SerializeField] protected Button skipButton;

    [Header("Character Portraits")]
    [SerializeField] protected Image playerImage;
    [SerializeField] protected Image characterImage;
    [SerializeField] protected Sprite playerPortrait;

    [Header("Speaking Effect")]
    [SerializeField, Range(0f, 1f)]
    private float inactiveBrightness = 0.5f;

    private float previousTimeScale = 1f;

    protected bool IsOpen { get; private set; }

    public bool IsPlaying => IsOpen;

    protected virtual void Awake()
    {
        if (nextButton != null)
            nextButton.onClick.AddListener(NextLine);

        if (skipButton != null)
            skipButton.onClick.AddListener(Skip);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public virtual void ShowDialogue()
    {
        if (IsOpen)
            return;

        if (dialoguePanel == null)
        {
            Debug.LogError(
                "Dialogue Panel is not assigned.",
                this
            );

            return;
        }

        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        IsOpen = true;

        dialoguePanel.SetActive(true);

        SetNextVisible(true);
        SetSkipVisible(true);
    }

    public virtual void CloseDialogue()
    {
        if (!IsOpen)
            return;

        IsOpen = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        Time.timeScale = previousTimeScale;
    }

    public abstract void NextLine();

    public virtual void Skip()
    {
        if (!IsOpen)
            return;

        CloseDialogue();
    }

    protected void SetNextVisible(bool visible)
    {
        if (nextButton != null)
            nextButton.gameObject.SetActive(visible);
    }

    protected void SetSkipVisible(bool visible)
    {
        if (skipButton != null)
            skipButton.gameObject.SetActive(visible);
    }

    protected void SetupPortraits(Sprite bossPortrait)
    {
        if (playerImage != null)
        {
            playerImage.sprite = playerPortrait;
            playerImage.enabled = playerPortrait != null;
            playerImage.color = Color.white;
        }

        if (characterImage != null)
        {
            characterImage.sprite = bossPortrait;
            characterImage.enabled = bossPortrait != null;
            characterImage.color = Color.white;
        }
    }

    protected void UpdatePortraits(
        string characterName,
        bool showPlayer,
        bool showCharacter,
        bool isPlayerSpeaking,
        Sprite portrait,
        Sprite fallbackPortrait)
    {
        string speaker = characterName?.Trim();

        bool isZom =
            isPlayerSpeaking ||
            string.Equals(
                speaker,
                "Zom",
                System.StringComparison.OrdinalIgnoreCase
            ) ||
            string.Equals(
                speaker,
                "Player",
                System.StringComparison.OrdinalIgnoreCase
            ) ||
            speaker == "«ÍÁ";

        if (playerImage != null)
        {
            playerImage.sprite = playerPortrait;

            playerImage.enabled =
                showPlayer && playerPortrait != null;

            playerImage.color = isZom
                ? Color.white
                : GetInactiveColor();
        }

        if (characterImage != null)
        {
            if (portrait != null)
            {
                characterImage.sprite = portrait;
            }
            else if (speaker == "{BOSS}")
            {
                characterImage.sprite = fallbackPortrait;
            }
            else if (characterImage.sprite == null)
            {
                characterImage.sprite = fallbackPortrait;
            }

            characterImage.enabled =
                showCharacter &&
                characterImage.sprite != null;

            characterImage.color = isZom
                ? GetInactiveColor()
                : Color.white;
        }
    }

    private Color GetInactiveColor()
    {
        return new Color(
            inactiveBrightness,
            inactiveBrightness,
            inactiveBrightness,
            1f
        );
    }

    protected virtual void OnDestroy()
    {
        if (nextButton != null)
            nextButton.onClick.RemoveListener(NextLine);

        if (skipButton != null)
            skipButton.onClick.RemoveListener(Skip);

        if (IsOpen)
            Time.timeScale = previousTimeScale;
    }
}