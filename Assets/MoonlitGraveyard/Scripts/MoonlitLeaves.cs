using UnityEngine;

// ใบไม้ร่วง 3 ระยะ (ไกล/กลาง/ใกล้) หน่วยภายในเป็นพิกเซลของภาพต้นฉบับ 1438x2576
public class MoonlitLeaves : MonoBehaviour
{
    public Sprite sprite;
    public MoonlitParallax parallax;
    public int count = 40;

    class P { public Transform tr; public SpriteRenderer sr; public float x, y, z, s, vy, sw, swa, ph, rot, vr, wind; public Color c; }
    P[] ps;
    static readonly Color[] pal = {
        new Color(110/255f, 90/255f, 55/255f), new Color(76/255f, 69/255f, 61/255f),
        new Color(140/255f, 110/255f, 60/255f), new Color(55/255f, 48/255f, 36/255f) };

    void Start()
    {
        ps = new P[count];
        for (int i = 0; i < count; i++)
        {
            var go = new GameObject("leaf");
            go.transform.SetParent(transform, false);
            var p = new P { tr = go.transform, sr = go.AddComponent<SpriteRenderer>() };
            p.sr.sprite = sprite;
            Reset(p, true);
            ps[i] = p;
        }
    }

    void Reset(P p, bool init)
    {
        float z = Random.value; bool near = z > 0.72f, far = z < 0.35f;
        p.x = init ? Random.Range(0f, 1438f) : Random.Range(-80f, 820f);
        p.y = init ? Random.Range(-100f, 2576f) : Random.Range(-100f, -10f);
        p.z = near ? 1.4f : far ? 0.25f : 0.6f;
        p.s = near ? Random.Range(18f, 32f) : far ? Random.Range(6f, 11f) : Random.Range(10f, 18f);
        p.vy = near ? Random.Range(100f, 150f) : far ? Random.Range(28f, 46f) : Random.Range(52f, 84f);
        p.sw = Random.Range(0.6f, 1.6f); p.swa = Random.Range(24f, 64f);
        p.ph = Random.Range(0f, 6.28f); p.rot = Random.Range(0f, 360f); p.vr = Random.Range(-90f, 90f);
        p.wind = Random.Range(10f, 34f);
        float a = near ? 0.95f : far ? 0.6f : 0.88f;
        p.c = pal[Random.Range(0, pal.Length)]; p.c.a = a;
        p.sr.color = p.c;
        p.sr.sortingOrder = near ? 55 : 43;
        p.tr.localScale = Vector3.one * (p.s / 32f);
    }

    void Update()
    {
        float dt = Time.deltaTime;
        Vector2 cur = parallax ? parallax.Current : Vector2.zero;
        Vector2 st = parallax ? parallax.strength : Vector2.zero;
        foreach (var p in ps)
        {
            p.y += p.vy * dt; p.ph += p.sw * dt;
            p.x += (p.wind + Mathf.Sin(p.ph) * p.swa * 0.5f) * dt;
            p.rot += p.vr * dt;
            if (p.y > 2640f || p.x > 1520f) { Reset(p, false); continue; }
            float wx = (p.x - 719f) / 100f - cur.x * st.x * p.z;
            float wy = (1288f - p.y) / 100f - cur.y * st.y * p.z;
            p.tr.localPosition = new Vector3(wx, wy, 0);
            float fl = Mathf.Abs(Mathf.Cos(p.ph * 1.7f));
            p.tr.localRotation = Quaternion.Euler(0, 0, -p.rot);
            var sc = p.tr.localScale; sc.y = (p.s / 32f) * (0.35f + 0.65f * fl); p.tr.localScale = sc;
        }
    }
}
