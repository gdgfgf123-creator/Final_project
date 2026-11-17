// Put this file in an "Editor" folder. Menu: Tools > Build BoneFont
// Needs BoneFont_atlas.png imported into the project (Texture Type: Default, sRGB on, Alpha Is Transparency on, no compression recommended).
using UnityEngine;
using UnityEditor;

public static class BoneFontBuilder
{
    struct G { public int id,x,y,w,h,yoff,adv;
        public G(int id,int x,int y,int w,int h,int yoff,int adv){this.id=id;this.x=x;this.y=y;this.w=w;this.h=h;this.yoff=yoff;this.adv=adv;} }

    const int AtlasW = 2064, AtlasH = 2064, Base = 322, LineHeight = 333;
    static readonly G[] Glyphs = new G[] {
        new G(65,2,2,303,325,-1,289),
        new G(97,2,2,303,325,-1,289),
        new G(66,346,2,262,325,-1,248),
        new G(98,346,2,262,325,-1,248),
        new G(67,690,2,248,327,-2,234),
        new G(99,690,2,248,327,-2,234),
        new G(68,1034,2,267,323,0,253),
        new G(100,1034,2,267,323,0,253),
        new G(69,1378,2,232,325,-1,218),
        new G(101,1378,2,232,325,-1,218),
        new G(70,1722,2,229,325,-1,215),
        new G(102,1722,2,229,325,-1,215),
        new G(71,2,346,270,326,-1,256),
        new G(103,2,346,270,326,-1,256),
        new G(72,346,346,259,323,0,245),
        new G(104,346,346,259,323,0,245),
        new G(73,690,346,139,323,0,125),
        new G(105,690,346,139,323,0,125),
        new G(74,1034,346,237,326,-1,223),
        new G(106,1034,346,237,326,-1,223),
        new G(75,1378,346,272,329,-2,258),
        new G(107,1378,346,272,329,-2,258),
        new G(76,1722,346,232,325,1,218),
        new G(108,1722,346,232,325,1,218),
        new G(77,2,690,318,324,0,304),
        new G(109,2,690,318,324,0,304),
        new G(78,346,690,298,326,0,284),
        new G(110,346,690,298,326,0,284),
        new G(79,690,690,298,329,-2,284),
        new G(111,690,690,298,329,-2,284),
        new G(80,1034,690,253,328,-2,239),
        new G(112,1034,690,253,328,-2,239),
        new G(81,1378,690,301,334,-1,287),
        new G(113,1378,690,301,334,-1,287),
        new G(82,1722,690,259,330,-2,245),
        new G(114,1722,690,259,330,-2,245),
        new G(83,2,1034,252,329,-2,238),
        new G(115,2,1034,252,329,-2,238),
        new G(84,346,1034,286,324,1,272),
        new G(116,346,1034,286,324,1,272),
        new G(85,690,1034,291,325,3,277),
        new G(117,690,1034,291,325,3,277),
        new G(86,1034,1034,296,323,1,282),
        new G(118,1034,1034,296,323,1,282),
        new G(87,1378,1034,342,323,2,328),
        new G(119,1378,1034,342,323,2,328),
        new G(88,1722,1034,290,330,-2,276),
        new G(120,1722,1034,290,330,-2,276),
        new G(89,2,1378,286,325,-2,272),
        new G(121,2,1378,286,325,-2,272),
        new G(90,346,1378,258,320,2,244),
        new G(122,346,1378,258,320,2,244),
        new G(48,690,1378,267,326,-2,253),
        new G(49,1034,1378,182,320,1,168),
        new G(50,1378,1378,235,321,-2,221),
        new G(51,1722,1378,237,326,-2,223),
        new G(52,2,1722,253,317,2,239),
        new G(53,346,1722,237,322,2,223),
        new G(54,690,1722,247,326,-2,233),
        new G(55,1034,1722,241,318,2,227),
        new G(56,1378,1722,255,326,-2,241),
        new G(57,1722,1722,248,325,-2,234),
        new G(32,0,0,0,0,0,110),
    };

    [MenuItem("Tools/Build BoneFont")]
    static void Build()
    {
        string[] guids = AssetDatabase.FindAssets("BoneFont_atlas t:Texture2D");
        if (guids.Length == 0) { Debug.LogError("BoneFont_atlas.png not found in project"); return; }
        string texPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        string dir = System.IO.Path.GetDirectoryName(texPath);
        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);

        Material mat = new Material(Shader.Find("UI/Default"));
        mat.mainTexture = tex;
        AssetDatabase.CreateAsset(mat, dir + "/BoneFont.mat");

        Font font = new Font("BoneFont");
        AssetDatabase.CreateAsset(font, dir + "/BoneFont.fontsettings");
        font.material = mat;

        CharacterInfo[] infos = new CharacterInfo[Glyphs.Length];
        for (int i = 0; i < Glyphs.Length; i++)
        {
            G g = Glyphs[i];
            float u0 = (float)g.x / AtlasW, u1 = (float)(g.x + g.w) / AtlasW;
            float vTop = 1f - (float)g.y / AtlasH, vBot = 1f - (float)(g.y + g.h) / AtlasH;
            CharacterInfo ci = new CharacterInfo();
            ci.index = g.id;
            ci.uvBottomLeft = new Vector2(u0, vBot);
            ci.uvBottomRight = new Vector2(u1, vBot);
            ci.uvTopLeft = new Vector2(u0, vTop);
            ci.uvTopRight = new Vector2(u1, vTop);
            ci.minX = 0; ci.maxX = g.w;
            ci.maxY = Base - g.yoff; ci.minY = ci.maxY - g.h;
            ci.advance = g.adv;
            ci.size = Base;
            infos[i] = ci;
        }
        font.characterInfo = infos;
        EditorUtility.SetDirty(font);
        AssetDatabase.SaveAssets();
        Debug.Log("BoneFont created at " + dir);
    }
}
