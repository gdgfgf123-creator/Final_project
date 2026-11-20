using UnityEngine;

// แกว่งธง / ใบไม้ รอบจุดหมุน (ตัวออบเจกต์นี้ต้องอยู่ที่จุดยึดด้านบน)
public class CourtyardSway : MonoBehaviour
{
    public float amplitude = 1.5f;   // องศา
    public float speed = 0.8f;
    float phase;
    void Awake() { phase = Random.value * 6.28f; }
    void Update()
    {
        float t = Time.time * speed + phase;
        float r = Mathf.Sin(t) * amplitude + Mathf.Sin(t * 2.3f) * amplitude * 0.25f;
        transform.localRotation = Quaternion.Euler(0, 0, r);
    }
}
