using UnityEngine;

// ใบไม้ร่วง 3 ระยะ (ไกล/กลาง/ใกล้) ทำด้วย SpriteRenderer วนใช้ซ้ำ
public class CourtyardLeaves : MonoBehaviour
{
    public Sprite sprite;
    public CourtyardParallax parallax;
    public int count = 46;

    class P { public Transform tr; public SpriteRenderer sr; public float x, y, z, s, vy, sw, swa, ph, rot, vr, wind, a; public Color c; }
    P[] ps;
    static readonly Color[] pal = {
        new Color(205/255f, 30/255f, 42/255f), new Color(160/255f, 22/255f, 34/255f),
        new Color(230/255f, 70/255f, 55/255f), new Color(120/255f, 16/255f, 30/255f) };

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

    // หน่วยในโค้ดนี้เป็นพิกเซลของภาพต้นฉบับ 1024x1536 (แปลงเป็น world ตอนวาง)
    void Reset(P p, bool init)
    {
        float z = Random.value; bool near = z > 0.72f, far = z < 0.35f;
        p.x = init ? Random.Range(0f, 1024f) : Random.Range(-60f, 700f);
        p.y = init ? Random.Range(-100f, 1536f) : Random.Range(-80f, -10f);
        p.z = near ? 1.4f : far ? 0.25f : 0.6f;
        p.s = near ? Random.Range(14f, 24f) : far ? Random.Range(4f, 8f) : Random.Range(7f, 13f);
        p.vy = near ? Random.Range(70f, 110f) : far ? Random.Range(20f, 34f) : Random.Range(38f, 62f);
        p.sw = Random.Range(0.6f, 1.6f); p.swa = Random.Range(18f, 48f);
        p.ph = Random.Range(0f, 6.28f); p.rot = Random.Range(0f, 360f); p.vr = Random.Range(-90f, 90f);
        p.wind = Random.Range(8f, 26f);
        p.a = near ? 0.95f : far ? 0.55f : 0.85f;
        p.c = pal[Random.Range(0, pal.Length)]; p.c.a = p.a;
        p.sr.color = p.c;
        p.sr.sortingOrder = near ? 55 : far ? 8 : 25;
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
            if (p.y > 1600f || p.x > 1100f) { Reset(p, false); continue; }
            float wx = (p.x - 512f) / 100f - cur.x * st.x * p.z;
            float wy = (768f - p.y) / 100f - cur.y * st.y * p.z;
            p.tr.localPosition = new Vector3(wx, wy, 0);
            float fl = Mathf.Abs(Mathf.Cos(p.ph * 1.7f));
            p.tr.localRotation = Quaternion.Euler(0, 0, -p.rot);
            var sc = p.tr.localScale; sc.y = (p.s / 32f) * (0.35f + 0.65f * fl); p.tr.localScale = sc;
        }
    }
}
