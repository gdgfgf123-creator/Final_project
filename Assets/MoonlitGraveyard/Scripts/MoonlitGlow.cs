using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MoonlitGlow : MonoBehaviour
{
    public float baseAlpha = 0.5f;
    public float speed = 9f;
    public float amount = 0.2f;
    public float noise = 0.08f;
    SpriteRenderer sr;
    float seed;

    void Awake() { sr = GetComponent<SpriteRenderer>(); seed = Random.value * 100f; }

    void Update()
    {
        float t = Time.time + seed;
        float f = 1f - amount + amount * Mathf.Sin(t * speed) + 0.5f * amount * Mathf.Sin(t * speed * 2.6f)
                  + noise * Random.value;
        var c = sr.color;
        c.a = Mathf.Clamp01(baseAlpha * f);
        sr.color = c;
    }
}
