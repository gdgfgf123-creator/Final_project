using UnityEngine;

// ประกายไฟลอยขึ้นจากโคมไฟ/เทียน
public class CourtyardEmbers : MonoBehaviour
{
    public Sprite glow;
    public Material additive;
    public CourtyardParallax parallax;
    public Vector3[] sources;     // local position ของแหล่งกำเนิด
    public int count = 34;

    class E { public Transform tr; public SpriteRenderer sr; public Vector3 p; public float vx, vy, life, max, r, ph; }
    E[] es;

    void Start()
    {
        es = new E[count];
        for (int i = 0; i < count; i++)
        {
            var go = new GameObject("ember");
            go.transform.SetParent(transform, false);
            var e = new E { tr = go.transform, sr = go.AddComponent<SpriteRenderer>() };
            e.sr.sprite = glow; e.sr.sharedMaterial = additive; e.sr.sortingOrder = 41;
            Respawn(e); e.life = Random.Range(0f, e.max);
            es[i] = e;
        }
    }

    void Respawn(E e)
    {
        var s = sources[Random.Range(0, sources.Length)];
        e.p = s + new Vector3(Random.Range(-0.08f, 0.08f), Random.Range(-0.06f, 0.04f), 0);
        e.vx = Random.Range(-0.08f, 0.08f); e.vy = Random.Range(0.14f, 0.38f);
        e.life = 0; e.max = Random.Range(2.5f, 5f); e.r = Random.Range(0.8f, 1.8f); e.ph = Random.Range(0f, 6.28f);
    }

    void Update()
    {
        float dt = Time.deltaTime;
        Vector2 cur = parallax ? parallax.Current : Vector2.zero;
        Vector2 st = parallax ? parallax.strength : Vector2.zero;
        foreach (var e in es)
        {
            e.life += dt;
            if (e.life > e.max) Respawn(e);
            e.p.x += (e.vx + Mathf.Sin(Time.time * 2f + e.ph) * 0.1f) * dt;
            e.p.y += e.vy * dt;
            float k = 1f - e.life / e.max;
            e.tr.localPosition = e.p + new Vector3(-cur.x * st.x * 0.25f, -cur.y * st.y * 0.25f, 0);
            e.tr.localScale = Vector3.one * e.r * 0.012f;
            e.sr.color = new Color(1f, (150 + 60 * k) / 255f, 70 / 255f, 0.8f * k);
        }
    }
}
