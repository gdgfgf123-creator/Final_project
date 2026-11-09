using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private GameObject bossHealthPanel;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Boss sceneBoss;
    private Boss targetBoss;

    private void Awake()
    {
        if (bossHealthPanel == null || hpSlider == null)
        {
            Debug.LogError(
                "BossHealthUI: ยังไม่ได้ใส่ Boss Health Panel หรือ HP Slider",
                this
            );

            enabled = false;
            return;
        }

        // สคริปต์ต้องอยู่บน Canvas หรือ Object นอก Panel
        if (transform.IsChildOf(bossHealthPanel.transform))
        {
            Debug.LogError(
                "ย้าย BossHealthUI ไปไว้บน Canvas ที่อยู่นอก BossHealthPanel",
                this
            );

            enabled = false;
            return;
        }

        Hide();
    }

    private void Start()
    {
        if (targetBoss == null && sceneBoss != null)
            Show(sceneBoss);
    }

    public void Show(Boss boss)
    {
        if (boss == null || bossHealthPanel == null || hpSlider == null)
        {
            Debug.LogError(
                "BossHealthUI: ตรวจ Boss, Boss Health Panel และ HP Slider",
                this
            );
            return;
        }

        targetBoss = boss;

        hpSlider.minValue = 0;
        hpSlider.maxValue = Mathf.Max(1, boss.CurrentHP);
        hpSlider.wholeNumbers = true;
        hpSlider.interactable = false;
        hpSlider.SetValueWithoutNotify(boss.CurrentHP);

        bossHealthPanel.SetActive(true);

        Debug.Log("เปิดแถบ HP ของ " + boss.name, this);
    }

    private void Update()
    {
        if (targetBoss == null)
        {
            if (bossHealthPanel != null && bossHealthPanel.activeSelf)
                Hide();

            return;
        }

        hpSlider.SetValueWithoutNotify(targetBoss.CurrentHP);

        if (targetBoss.CurrentHP <= 0)
            Hide();
    }

    private void Hide()
    {
        targetBoss = null;

        if (bossHealthPanel != null)
            bossHealthPanel.SetActive(false);
    }
}