using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MoonlitParallax : MonoBehaviour
{
    [System.Serializable]
    public class Layer
    {
        public Transform t;
        public float depth = 1f;
        [HideInInspector] public Vector3 origin;
    }

    public Layer[] layers;
    public Vector2 strength = new Vector2(0.36f, 0.22f);
    public float smooth = 6f;
    public bool useInput = true;
    public float idleDelay = 3.5f;
    public Vector2 manualInput;                 // ป้อน -1..1 จากเกม

    public Vector2 Current { get { return cur; } }   // ให้สคริปต์อื่น (ใบไม้ร่วง) อ่านไปใช้

    Vector2 cur, target, lastMouse;
    float lastInput = -99f;

    void Start() { foreach (var l in layers) if (l.t) l.origin = l.t.localPosition; }

    bool ReadPointer(out Vector2 p)
    {
        p = Vector2.zero;
        Vector2 pos;
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            pos = Touchscreen.current.primaryTouch.position.ReadValue();
        else if (Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0f)
            pos = Mouse.current.position.ReadValue();
        else return false;
#else
        if (Input.touchCount > 0) pos = Input.GetTouch(0).position;
        else
        {
            Vector2 m = Input.mousePosition;
            if ((m - lastMouse).sqrMagnitude < 0.01f) { lastMouse = m; return false; }
            lastMouse = m; pos = m;
        }
#endif
        p = new Vector2(Mathf.Clamp(pos.x / Screen.width * 2f - 1f, -1f, 1f),
                        Mathf.Clamp(pos.y / Screen.height * 2f - 1f, -1f, 1f));
        return true;
    }

    void Update()
    {
        Vector2 p;
        if (useInput && ReadPointer(out p)) { target = p; lastInput = Time.time; }
        else if (manualInput != Vector2.zero) { target = manualInput; lastInput = Time.time; }
        else if (Time.time - lastInput > idleDelay)
        {
            float t = Time.time;
            target = new Vector2(Mathf.Sin(t * 0.45f) * 0.8f, Mathf.Sin(t * 0.31f) * 0.5f);
        }
        cur = Vector2.Lerp(cur, target, 1f - Mathf.Exp(-smooth * Time.deltaTime));
        foreach (var l in layers)
        {
            if (!l.t) continue;
            l.t.localPosition = l.origin + new Vector3(-cur.x * strength.x * l.depth, -cur.y * strength.y * l.depth, 0f);
        }
    }
}
