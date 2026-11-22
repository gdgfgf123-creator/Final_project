using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelManager : MonoBehaviour
{
    [Header("Stage Transition")]
    [SerializeField] private GameObject currentBackground;
    [SerializeField] private GameObject stageTwoBackground;
    [SerializeField, Min(0f)] private float backgroundTransitionDuration = 1.5f;
    [SerializeField] private PowerUnlock powerUnlock;
    public int CurrentStage { get; private set; } = 1;

    private void Awake()
    {
        SetBackgroundBehind(currentBackground);
        SetBackgroundBehind(stageTwoBackground);

        if (currentBackground != null)
            currentBackground.SetActive(true);

        if (stageTwoBackground != null)
        {
            SetBackgroundAlpha(stageTwoBackground, 0f);
            stageTwoBackground.SetActive(false);
        }
    }

    public IEnumerator WaitForStageOneClear(Boss boss)
    {
        if (CurrentStage != 1)
            yield break;

        while (boss != null)
            yield return null;

        if (powerUnlock != null)
            powerUnlock.CheckPowers();

        CurrentStage = 2;

        yield return CrossfadeToStageTwoBackground();

        Debug.Log("Stage 2 started", this);
    }

    private IEnumerator CrossfadeToStageTwoBackground()
    {
        if (currentBackground == null || stageTwoBackground == null)
        {
            Debug.LogWarning(
                "LevelManager: Assign both background GameObjects in the Inspector.",
                this
            );
            yield break;
        }

        currentBackground.SetActive(true);
        SetBackgroundAlpha(currentBackground, 1f);

        SetBackgroundAlpha(stageTwoBackground, 0f);
        stageTwoBackground.SetActive(true);

        if (backgroundTransitionDuration > 0f)
        {
            float elapsed = 0f;

            while (elapsed < backgroundTransitionDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(
                    elapsed / backgroundTransitionDuration
                );

                SetBackgroundAlpha(currentBackground, 1f - progress);
                SetBackgroundAlpha(stageTwoBackground, progress);

                yield return null;
            }
        }

        SetBackgroundAlpha(currentBackground, 0f);
        currentBackground.SetActive(false);

        SetBackgroundAlpha(stageTwoBackground, 1f);
        stageTwoBackground.SetActive(true);
    }

    private void SetBackgroundAlpha(GameObject background, float alpha)
    {
        SpriteRenderer[] renderers =
            background.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer spriteRenderer in renderers)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }

    private void SetBackgroundBehind(GameObject background)
    {
        if (background == null)
            return;

        SortingGroup sortingGroup = background.GetComponent<SortingGroup>();

        if (sortingGroup == null)
            sortingGroup = background.AddComponent<SortingGroup>();

        sortingGroup.sortingLayerName = "Default";
        sortingGroup.sortingOrder = -1000;
    }
}