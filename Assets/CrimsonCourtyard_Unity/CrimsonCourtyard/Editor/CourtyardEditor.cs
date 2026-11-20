using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

// ตั้งค่า import ของ sprite ในโฟลเดอร์นี้อัตโนมัติ
public class CourtyardImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (!assetPath.Contains("CrimsonCourtyard/Sprites")) return;
        var ti = (TextureImporter)assetImporter;
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Single;
        ti.spritePixelsPerUnit = 100;
        ti.mipmapEnabled = false;
        ti.alphaIsTransparency = true;
        ti.maxTextureSize = 2048;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
        ti.filterMode = FilterMode.Bilinear;
    }
}

public static class CourtyardBuilder
{
    const string Dir = "Assets/CrimsonCourtyard/";
    const float PPU = 100f;
    const int W = 1024, H = 1536;

    static Vector3 Px(float x, float y) { return new Vector3((x - W / 2f) / PPU, (H / 2f - y) / PPU, 0f); }
    static Sprite Spr(string n) { return AssetDatabase.LoadAssetAtPath<Sprite>(Dir + "Sprites/" + n + ".png"); }

    [MenuItem("Tools/Crimson Courtyard/Build Scene")]
    static void Build()
    {
        var root = new GameObject("CrimsonCourtyard");
        var ps = root.AddComponent<CourtyardParallax>();
        var layers = new List<CourtyardParallax.Layer>();

        var baseSp = Spr("01_base");
        var glowSp = Spr("glow"); var leafSp = Spr("leaf");
        if (baseSp == null || glowSp == null || leafSp == null) { Debug.LogError("ไม่พบ sprite ใน Assets/CrimsonCourtyard/Sprites"); Object.DestroyImmediate(root); return; }

        // ฉากหลัง
        var bg = new GameObject("01_base"); bg.transform.SetParent(root.transform, false);
        var bsr = bg.AddComponent<SpriteRenderer>(); bsr.sprite = baseSp; bsr.sortingOrder = 0;
        layers.Add(new CourtyardParallax.Layer { t = bg.transform, depth = 0.25f });

        // ชิ้นที่แกว่งได้: ชื่อ, x, y, w, h, จุดหมุน(0=ซ้าย .5=กลาง 1=ขวา), depth, องศา, ความเร็ว, order
        object[][] parts = {
            new object[]{"03_banner1",   0, 502,  75, 387, .5f, .25f, 1.6f, .8f, 10},
            new object[]{"03_banner2", 322, 275,  70, 317, .5f, .25f, 1.3f, .9f, 10},
            new object[]{"03_banner3", 719, 559,  60, 234, .5f, .25f, 1.4f, .75f, 10},
            new object[]{"03_banner4", 346, 657,  45, 171, .5f, .25f, 2.0f, 1.0f, 10},
            new object[]{"02_vineL",    72, 517, 101, 187, .5f, .6f, .8f, .6f, 20},
            new object[]{"02_vineR",   897, 167, 127, 431, 1f,  .6f, .7f, .5f, 20},
            new object[]{"02_canopy",    0,   0, 618, 442, 0f,  .6f, .5f, .45f, 30},
        };
        foreach (var p in parts)
        {
            string n = (string)p[0]; float x0 = System.Convert.ToSingle(p[1]), y0 = System.Convert.ToSingle(p[2]);
            float w = System.Convert.ToSingle(p[3]), h = System.Convert.ToSingle(p[4]), ox = (float)p[5];
            var sp = Spr(n); if (sp == null) { Debug.LogWarning("ไม่พบ " + n); continue; }
            var pivot = new GameObject(n + "_pivot"); pivot.transform.SetParent(root.transform, false);
            pivot.transform.localPosition = Px(x0 + w * ox, y0);
            var child = new GameObject(n); child.transform.SetParent(pivot.transform, false);
            child.transform.localPosition = new Vector3((w * (0.5f - ox)) / PPU, -h / 2f / PPU, 0);
            var sr = child.AddComponent<SpriteRenderer>(); sr.sprite = sp; sr.sortingOrder = (int)p[9];
            var sw = pivot.AddComponent<CourtyardSway>(); sw.amplitude = (float)p[7]; sw.speed = (float)p[8];
            layers.Add(new CourtyardParallax.Layer { t = pivot.transform, depth = (float)p[6] });
        }

        // วัสดุแสงแบบ additive
        var mat = AssetDatabase.LoadAssetAtPath<Material>(Dir + "GlowAdditive.mat");
        if (mat == null)
        {
            var sh = Shader.Find("Courtyard/SpriteAdditive");
            if (sh == null) { Debug.LogError("ไม่พบ shader Courtyard/SpriteAdditive"); return; }
            mat = new Material(sh); AssetDatabase.CreateAsset(mat, Dir + "GlowAdditive.mat");
        }

        var lightsRoot = new GameObject("Lights"); lightsRoot.transform.SetParent(root.transform, false);
        layers.Add(new CourtyardParallax.Layer { t = lightsRoot.transform, depth = 0.25f });
        var warm = new Color(1f, .65f, .27f); var red = new Color(1f, .31f, .37f);
        var emberSrc = new List<Vector3>();
        // x, y, radius(px), alpha, speed, amount
        float[][] ls = {
            new float[]{320,617,62,.5f,7,.18f}, new float[]{525,663,62,.5f,7,.18f}, new float[]{138,865,62,.5f,7,.18f},
            new float[]{357,863,62,.5f,7,.18f}, new float[]{549,846,62,.5f,7,.18f}, new float[]{743,846,62,.5f,7,.18f},
            new float[]{51,1269,135,.5f,6,.16f}, new float[]{960,1257,145,.5f,6.5f,.16f},
            new float[]{691,789,30,.5f,8,.2f}, new float[]{831,760,48,.5f,8,.2f},
            new float[]{59,947,26,.55f,12,.25f}, new float[]{229,922,26,.55f,12,.25f}, new float[]{248,927,26,.55f,12,.25f},
            new float[]{474,904,26,.55f,12,.25f}, new float[]{491,908,26,.55f,12,.25f}, new float[]{926,802,26,.55f,12,.25f},
            new float[]{971,790,26,.55f,12,.25f}, new float[]{994,967,26,.55f,12,.25f}, new float[]{958,977,26,.55f,12,.25f},
            new float[]{880,780,190,.16f,1.3f,.2f}, new float[]{816,240,62,.38f,.9f,.3f}, new float[]{700,185,240,.14f,.5f,.3f},
        };
        foreach (var l in ls)
        {
            bool isMoon = l[2] == 240f;
            AddGlow(lightsRoot.transform, glowSp, mat, Px(l[0], l[1]), l[2], isMoon ? red : warm, l[3], l[4], l[5]);
            if (l[2] >= 26 && l[2] <= 62 && l[0] != 816) emberSrc.Add(Px(l[0], l[1]));
        }

        // หมอก
        var fogRoot = new GameObject("Fog"); fogRoot.transform.SetParent(root.transform, false);
        layers.Add(new CourtyardParallax.Layer { t = fogRoot.transform, depth = 0.25f });
        float[][] fogs = { new float[]{200,1010,620,90,.05f,.09f}, new float[]{700,1130,760,110,.045f,-.06f}, new float[]{300,1380,820,120,.05f,.05f}, new float[]{600,900,520,70,.04f,-.04f} };
        foreach (var f in fogs)
        {
            var go = new GameObject("fog"); go.transform.SetParent(fogRoot.transform, false);
            go.transform.localPosition = Px(f[0], f[1]);
            go.transform.localScale = new Vector3(f[2] / 128f * 0.5f * 1.0f, f[3] / 128f * 0.5f * 1.2f, 1) * 1f;
            var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = glowSp; sr.sharedMaterial = mat; sr.sortingOrder = 6;
            sr.color = new Color(.85f, .38f, .43f, f[4] * 3f);
            var fd = go.AddComponent<CourtyardFog>(); fd.speed = f[5];
        }

        // ประกายไฟ + ใบไม้ร่วง
        var emb = new GameObject("Embers"); emb.transform.SetParent(root.transform, false);
        var er = emb.AddComponent<CourtyardEmbers>(); er.glow = glowSp; er.additive = mat; er.parallax = ps; er.sources = emberSrc.ToArray();
        var lf = new GameObject("FallingLeaves"); lf.transform.SetParent(root.transform, false);
        var lfs = lf.AddComponent<CourtyardLeaves>(); lfs.sprite = leafSp; lfs.parallax = ps;

        ps.layers = layers.ToArray();
        root.transform.localScale = Vector3.one * 1.07f;   // กันขอบโผล่ตอนขยับกล้อง

        var cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true; cam.orthographicSize = H / 2f / PPU;
            cam.transform.position = new Vector3(0, 0, -10); cam.backgroundColor = new Color(.07f, .02f, .04f);
        }
        Undo.RegisterCreatedObjectUndo(root, "Build Crimson Courtyard");
        Selection.activeGameObject = root;
    }

    static void AddGlow(Transform parent, Sprite sp, Material mat, Vector3 pos, float rPx, Color c, float a, float speed, float amount)
    {
        var go = new GameObject("glow"); go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localScale = Vector3.one * (2f * rPx / PPU / 2.56f);
        var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = sp; sr.sharedMaterial = mat; sr.sortingOrder = 40; sr.color = c;
        var f = go.AddComponent<CourtyardGlow>(); f.baseAlpha = a; f.speed = speed; f.amount = amount;
    }
}
