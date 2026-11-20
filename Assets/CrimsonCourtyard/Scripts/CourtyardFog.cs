using UnityEngine;

// หมอกลอยแนวนอนแล้ววนกลับ
public class CourtyardFog : MonoBehaviour
{
    public float speed = 0.09f;      // หน่วย world ต่อวินาที (ติดลบ = ไปทางซ้าย)
    public float range = 14f;
    float startX;
    void Start() { startX = transform.localPosition.x; }
    void Update()
    {
        float x = Mathf.Repeat(startX + speed * Time.time + range / 2f, range) - range / 2f;
        var p = transform.localPosition; p.x = x; transform.localPosition = p;
    }
}
