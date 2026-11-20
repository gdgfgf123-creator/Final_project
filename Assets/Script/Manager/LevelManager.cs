using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Stage Transition")]
    [SerializeField] private SpriteRenderer currentBackground;
    [SerializeField] private SpriteRenderer stageTwoBackground;
    [SerializeField, Min(0f)] private float backgroundTransitionDuration = 1.5f;

    public int CurrentStage { get; private set; } = 1;

    private void Awake()
    {
        if (stageTwoBackground != null)
        {
            Color color = stageTwoBackground.color;
            color.a = 0f;
            stageTwoBackground.color = color;
        }
    }

    public IEnumerator WaitForStageOneClear(Boss boss)
    {
        if (CurrentStage != 1)
            yield break;

        // รอจนกว่าบอส Level 1 จะถูกทำลาย
        while (boss != null)
            yield return null;

        CurrentStage = 2;

        // เปลี่ยนภาพพื้นหลัง แล้ว WaveSpawner จะเริ่ม Wave ของ Level 2 ต่อ
        yield return CrossfadeToStageTwoBackground();

        Debug.Log("Stage 2 started", this);
    }

    private IEnumerator CrossfadeToStageTwoBackground()
    {
        if (currentBackground == null || stageTwoBackground == null)
        {
            Debug.LogWarning(
                "LevelManager: Assign both background SpriteRenderers to fade between stages.",
                this
            );
            yield break;
        }

        Color currentColor = currentBackground.color;
        Color nextColor = stageTwoBackground.color;

        currentBackground.enabled = true;
        stageTwoBackground.enabled = true;

        currentColor.a = 1f;
        nextColor.a = 0f;

        currentBackground.color = currentColor;
        stageTwoBackground.color = nextColor;

        if (backgroundTransitionDuration > 0f)
        {
            float elapsed = 0f;

            while (elapsed < backgroundTransitionDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                float progress = Mathf.Clamp01(
                    elapsed / backgroundTransitionDuration
                );

                currentColor.a = 1f - progress;
                nextColor.a = progress;

                currentBackground.color = currentColor;
                stageTwoBackground.color = nextColor;

                yield return null;
            }
        }

        currentColor.a = 0f;
        currentBackground.color = currentColor;
        currentBackground.enabled = false;

        nextColor.a = 1f;
        stageTwoBackground.color = nextColor;
    }
}