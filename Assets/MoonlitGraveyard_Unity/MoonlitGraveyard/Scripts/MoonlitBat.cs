using UnityEngine;

// ค้างคาวบินผ่านท้องฟ้าทุก 8-16 วินาที
public class MoonlitBat : MonoBehaviour
{
    public Sprite sprite;
    public MoonlitParallax parallax;
    Transform tr; SpriteRenderer sr;
    float x, baseY, vx, amp, flap, s, next = 4f;
    bool active;

    void Start()
    {
        var go = new GameObject("bat"); go.transform.SetParent(transform, false);
        tr = go.transform; sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite; sr.sortingOrder = 3; sr.enabled = false;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        Vector2 cur = parallax ? parallax.Current : Vector2.zero;
        Vector2 st = parallax ? parallax.strength : Vector2.zero;
        if (!active)
        {
            if (Time.time > next)
            {
                active = true; sr.enabled = true;
                x = -120f; baseY = Random.Range(180f, 480f); vx = Random.Range(170f, 250f);
                amp = Random.Range(25f, 60f); flap = Random.Range(8f, 13f); s = Random.Range(0.9f, 1.3f);
            }
            return;
        }
        x += vx * dt;
        float y = baseY + Mathf.Sin(x * 0.012f) * amp;
        tr.localPosition = new Vector3((x - 719f) / 100f - cur.x * st.x * 0.1f, (1288f - y) / 100f - cur.y * st.y * 0.1f, 0);
        float f = 0.55f + 0.45f * Mathf.Abs(Mathf.Sin(Time.time * flap));
        tr.localScale = new Vector3(s * 1.0f, s * f, 1);
        if (x > 1560f) { active = false; sr.enabled = false; next = Time.time + Random.Range(8f, 16f); }
    }
}
