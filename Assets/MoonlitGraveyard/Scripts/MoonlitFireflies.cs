using UnityEngine;

// หิ่งห้อยลอยและกะพริบเหนือเนินสุสาน
public class MoonlitFireflies : MonoBehaviour
{
    public Sprite glow;
    public Material additive;
    public MoonlitParallax parallax;
    public int count = 30;

    class F { public Transform tr; public SpriteRenderer sr; public float x, y, ax, ay, sx, sy, ph, bl, z, r; }
    F[] fs;

    void Start()
    {
        fs = new F[count];
        for (int i = 0; i < count; i++)
        {
            var go = new GameObject("firefly");
            go.transform.SetParent(transform, false);
            var f = new F
            {
                tr = go.transform, sr = go.AddComponent<SpriteRenderer>(),
                x = Random.Range(60f, 1378f), y = Random.Range(1300f, 2350f),
                ax = Random.Range(30f, 110f), ay = Random.Range(14f, 50f),
                sx = Random.Range(0.12f, 0.4f), sy = Random.Range(0.1f, 0.35f),
                ph = Random.Range(0f, 60f), bl = Random.Range(0.5f, 1.6f),
                z = Random.Range(0.25f, 1.1f), r = Random.Range(2.2f, 4.2f)
            };
            f.sr.sprite = glow; f.sr.sharedMaterial = additive; f.sr.sortingOrder = 42;
            f.tr.localScale = Vector3.one * (f.r * 14f / 100f / 2.56f);
            fs[i] = f;
        }
    }

    void Update()
    {
        float t = Time.time;
        Vector2 cur = parallax ? parallax.Current : Vector2.zero;
        Vector2 st = parallax ? parallax.strength : Vector2.zero;
        foreach (var f in fs)
        {
            float px = f.x + Mathf.Sin(t * f.sx + f.ph) * f.ax;
            float py = f.y + Mathf.Sin(t * f.sy + f.ph * 1.7f) * f.ay;
            f.tr.localPosition = new Vector3((px - 719f) / 100f - cur.x * st.x * f.z, (1288f - py) / 100f - cur.y * st.y * f.z, 0);
            float b = Mathf.Pow(Mathf.Max(0f, Mathf.Sin(t * f.bl + f.ph)), 3f);
            f.sr.color = new Color(0.78f, 1f, 0.5f, 0.8f * b);
        }
    }
}
