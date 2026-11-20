using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

// ตั้งค่า import ของ sprite ในโฟลเดอร์นี้อัตโนมัติ
public class MoonlitImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (!assetPath.Contains("MoonlitGraveyard/Sprites")) return;
        var ti = (TextureImporter)assetImporter;
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Single;
        ti.spritePixelsPerUnit = 100;
        ti.mipmapEnabled = false;
        ti.alphaIsTransparency = true;
        ti.maxTextureSize = 4096;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
        ti.filterMode = FilterMode.Bilinear;
    }
}

public static class MoonlitBuilder
{
    const string Dir = "Assets/MoonlitGraveyard/";
    const float PPU = 100f;
    const int W = 1438, H = 2576;

    static Vector3 Px(float x, float y) { return new Vector3((x - W / 2f) / PPU, (H / 2f - y) / PPU, 0f); }
    static Sprite Spr(string n) { return AssetDatabase.LoadAssetAtPath<Sprite>(Dir + "Sprites/" + n + ".png"); }

    [MenuItem("Tools/Moonlit Graveyard/Build Scene")]
    static void Build()
    {
        var sky = Spr("01_sky"); var hill = Spr("02_hill"); var tree = Spr("03_tree");
        var glowSp = Spr("glow"); var leafSp = Spr("leaf"); var batSp = Spr("bat");
        if (sky == null || hill == null || tree == null || glowSp == null || leafSp == null || batSp == null)
        { Debug.LogError("ไม่พบ sprite ใน Assets/MoonlitGraveyard/Sprites (ตรวจว่าโฟลเดอร์ชื่อ MoonlitGraveyard อยู่ใต้ Assets โดยตรง)"); return; }

        var root = new GameObject("MoonlitGraveyard");
        var ps = root.AddComponent<MoonlitParallax>();
        var layers = new List<MoonlitParallax.Layer>();

        Add(root, layers, "01_sky", sky, 0, 0.08f);
        Add(root, layers, "02_hill", hill, 10, 0.3f);
        Add(root, layers, "03_tree", tree, 30, 0.8f);

        var mat = AssetDatabase.LoadAssetAtPath<Material>(Dir + "GlowAdditive.mat");
        if (mat == null)
        {
            var sh = Shader.Find("Moonlit/SpriteAdditive");
            if (sh == null) { Debug.LogError("ไม่พบ shader Moonlit/SpriteAdditive"); return; }
            mat = new Material(sh); AssetDatabase.CreateAsset(mat, Dir + "GlowAdditive.mat");
        }

        var warm = new Color(1f, .67f, .27f); var moonC = new Color(.59f, .88f, 1f);
        var gTree = Group(root, layers, "Lights_Tree", 0.8f);
        var gHill = Group(root, layers, "Lights_Hill", 0.3f);
        var gSky = Group(root, layers, "Lights_Sky", 0.08f);
        // x, y, radius(px), alpha, speed, amount
        AddGlow(gTree, glowSp, mat, 278, 1160, 200, .62f, 6, .18f, warm);
        AddGlow(gHill, glowSp, mat, 1160, 1150, 125, .55f, 2, .15f, warm);
        AddGlow(gHill, glowSp, mat, 1155, 1028, 48, .5f, 3, .15f, warm);
        AddGlow(gHill, glowSp, mat, 1270, 1135, 48, .5f, 3, .15f, warm);
        AddGlow(gHill, glowSp, mat, 1318, 1135, 48, .5f, 3, .15f, warm);
        float[][] candles = { new float[]{172,1588}, new float[]{215,1632}, new float[]{1356,1640}, new float[]{1402,1608}, new float[]{1412,1632} };
        var emberSrc = new List<Vector3>();
        foreach (var c in candles) { AddGlow(gHill, glowSp, mat, c[0], c[1], 40, .6f, 11, .25f, warm); emberSrc.Add(Px(c[0], c[1] - 10)); }
        AddGlow(gHill, glowSp, mat, 85, 2130, 160, .5f, 6, .16f, warm);
        AddGlow(gHill, glowSp, mat, 1355, 2125, 170, .5f, 6.5f, .16f, warm);
        AddGlow(gSky, glowSp, mat, 1002, 357, 380, .17f, .5f, .3f, moonC);
        emberSrc.Add(Px(278, 1160)); emberSrc.Add(Px(85, 2110)); emberSrc.Add(Px(1355, 2105));

        // หมอก: x, y, กว้าง, สูง, alpha, ความเร็ว (px/s)
        var fogRoot = Group(root, layers, "Fog", 0.3f);
        float[][] fogs = { new float[]{300,1480,900,110,.05f,9}, new float[]{900,1780,1000,130,.05f,-7},
                           new float[]{500,2020,1100,140,.045f,6}, new float[]{1000,2330,1200,150,.045f,-5} };
        foreach (var f in fogs)
        {
            var go = new GameObject("fog"); go.transform.SetParent(fogRoot.transform, false);
            go.transform.localPosition = Px(f[0], f[1]);
            go.transform.localScale = new Vector3(f[2] / 256f, f[3] / 256f * 1.3f, 1);
            var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = glowSp; sr.sharedMaterial = mat; sr.sortingOrder = 12;
            sr.color = new Color(.67f, .8f, .88f, f[4] * 3.5f);
            var fd = go.AddComponent<MoonlitFog>(); fd.speed = f[5] / 100f; fd.range = 19f;
        }

        var emb = new GameObject("Embers"); emb.transform.SetParent(root.transform, false);
        var er = emb.AddComponent<MoonlitEmbers>(); er.glow = glowSp; er.additive = mat; er.parallax = ps; er.sources = emberSrc.ToArray();
        var ff = new GameObject("Fireflies"); ff.transform.SetParent(root.transform, false);
        var ffs = ff.AddComponent<MoonlitFireflies>(); ffs.glow = glowSp; ffs.additive = mat; ffs.parallax = ps;
        var bt = new GameObject("Bat"); bt.transform.SetParent(root.transform, false);
        var bts = bt.AddComponent<MoonlitBat>(); bts.sprite = batSp; bts.parallax = ps;
        var lf = new GameObject("FallingLeaves"); lf.transform.SetParent(root.transform, false);
        var lfs = lf.AddComponent<MoonlitLeaves>(); lfs.sprite = leafSp; lfs.parallax = ps;

        ps.layers = layers.ToArray();
        ps.strength = new Vector2(0.36f, 0.22f);
        root.transform.localScale = Vector3.one * 1.07f;

        var cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true; cam.orthographicSize = H / 2f / PPU;
            cam.transform.position = new Vector3(0, 0, -10); cam.backgroundColor = new Color(.02f, .04f, .09f);
        }
        Undo.RegisterCreatedObjectUndo(root, "Build Moonlit Graveyard");
        Selection.activeGameObject = root;
    }

    static void Add(GameObject root, List<MoonlitParallax.Layer> layers, string name, Sprite sp, int order, float depth)
    {
        var go = new GameObject(name); go.transform.SetParent(root.transform, false);
        var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = sp; sr.sortingOrder = order;
        layers.Add(new MoonlitParallax.Layer { t = go.transform, depth = depth });
    }

    static Transform Group(GameObject root, List<MoonlitParallax.Layer> layers, string name, float depth)
    {
        var go = new GameObject(name); go.transform.SetParent(root.transform, false);
        layers.Add(new MoonlitParallax.Layer { t = go.transform, depth = depth });
        return go.transform;
    }

    static void AddGlow(Transform parent, Sprite sp, Material mat, float x, float y, float rPx, float a, float speed, float amount, Color c)
    {
        var go = new GameObject("glow"); go.transform.SetParent(parent, false);
        go.transform.localPosition = Px(x, y);
        go.transform.localScale = Vector3.one * (2f * rPx / PPU / 2.56f);
        var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = sp; sr.sharedMaterial = mat; sr.sortingOrder = 40; sr.color = c;
        var f = go.AddComponent<MoonlitGlow>(); f.baseAlpha = a; f.speed = speed; f.amount = amount;
    }
}
