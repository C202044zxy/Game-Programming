using UnityEngine;
using UnityEngine.UI;

/// Interactive Solar System for Kids — assignment build.
///
/// Class concepts demonstrated (>= 4 required):
///   1. Materials  — emissive Sun/comet + per-planet colours via LoadMat helper.
///   2. Lighting   — point Light at the Sun + low ambient + faint fill directional.
///   3. Behaviors  — OrbitRotation, ClickableCelestial, CameraFocusController, ClickPop.
///   4. Audio      — procedural click "blip" played on every selection.
///   5. Cameras    — main perspective camera + a top-down minimap camera
///                   rendering to a runtime RenderTexture shown in the corner.
///   6. Prefabs    — LoadMat tries Resources.Load first, falls back to procedural.
///
/// Minimum requirements check:
///   [x] Rotation / orbit motion        — every body uses OrbitRotation.
///   [x] Camera changes on selection    — CameraFocusController.FocusOn.
///   [x] Visual response (glow + pop)   — ClickableCelestial highlight + ClickPop.
///   [x] Audio response                 — click blip plays on every selection.
///   [x] Return to main view            — Back button + ESC key both call ReturnHome.
public class SolarSystemKidsBootstrap : MonoBehaviour
{
    void Awake()
    {
        BuildLights();
        BuildStarfield();

        GameObject sun = BuildSun();
        GameObject earth = BuildPlanet(
            "Earth", sun.transform, distance: 8f, size: 1.4f,
            color: new Color(0.25f, 0.55f, 0.95f),
            orbitSpeed: 22f, spinSpeed: 35f,
            fact: "Earth is our home! It has blue oceans, green land, and white clouds.",
            highlight: new Color(0.5f, 0.9f, 1f));
        BuildMoon(
            "Moon", earth.transform, distance: 2.2f, size: 0.45f,
            color: new Color(0.85f, 0.85f, 0.88f),
            orbitSpeed: 90f,
            fact: "The Moon is Earth's best friend in the sky. Astronauts have walked on it!",
            highlight: new Color(1f, 1f, 0.8f));
        BuildPlanet(
            "Mars", sun.transform, distance: 12f, size: 1.0f,
            color: new Color(0.85f, 0.35f, 0.2f),
            orbitSpeed: 14f, spinSpeed: 30f,
            fact: "Mars is the red planet. It has the biggest volcano in the solar system!",
            highlight: new Color(1f, 0.6f, 0.4f));
        BuildPlanet(
            "Saturn", sun.transform, distance: 17f, size: 2.0f,
            color: new Color(0.95f, 0.85f, 0.55f),
            orbitSpeed: 8f, spinSpeed: 25f,
            fact: "Saturn has beautiful rings made of ice and rock. So pretty!",
            highlight: new Color(1f, 0.95f, 0.7f),
            withRings: true);

        BuildComet(sun.transform);

        Camera main = BuildMainCamera();
        BuildMinimapCamera();
        BuildUI(main);
    }

    // ───────────────────────── Lighting ─────────────────────────
    void BuildLights()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.08f, 0.08f, 0.15f);

        GameObject d = new GameObject("FillLight");
        Light l = d.AddComponent<Light>();
        l.type = LightType.Directional;
        l.color = new Color(0.7f, 0.75f, 1f);
        l.intensity = 0.25f;
        d.transform.rotation = Quaternion.Euler(50f, -40f, 0f);
    }

    void BuildStarfield()
    {
        GameObject root = new GameObject("Starfield");
        for (int i = 0; i < 200; i++)
        {
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.name = "Star";
            s.transform.SetParent(root.transform);
            s.transform.position = Random.onUnitSphere * Random.Range(60f, 90f);
            s.transform.localScale = Vector3.one * Random.Range(0.15f, 0.35f);
            DestroyImmediate(s.GetComponent<Collider>());
            s.GetComponent<Renderer>().material =
                MakeUnlitColor(Color.white * Random.Range(0.6f, 1f));
        }
    }

    // ───────────────────────── Bodies ─────────────────────────
    GameObject BuildSun()
    {
        GameObject p = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        p.name = "Sun";
        p.transform.position = Vector3.zero;
        p.transform.localScale = Vector3.one * 3.5f;
        p.GetComponent<Renderer>().material = LoadMat(
            "SunMaterial", new Color(1f, 0.85f, 0.3f),
            emissive: true, emission: new Color(1f, 0.7f, 0.1f) * 1.5f);

        GameObject lightGO = new GameObject("SunLight");
        lightGO.transform.SetParent(p.transform, false);
        Light l = lightGO.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = new Color(1f, 0.95f, 0.8f);
        l.intensity = 2.5f;
        l.range = 80f;

        var spin = p.AddComponent<OrbitRotation>();
        spin.orbitCenter = null;
        spin.selfRotationSpeed = 8f;

        AddClick(p, "Sun",
            "The Sun is a giant ball of fire! It gives us light and warmth every day.",
            new Color(1f, 0.9f, 0.4f));
        return p;
    }

    GameObject BuildPlanet(string name, Transform sun, float distance, float size,
        Color color, float orbitSpeed, float spinSpeed, string fact, Color highlight,
        bool withRings = false)
    {
        GameObject p = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        p.name = name;
        p.transform.position = sun.position + new Vector3(distance, 0f, 0f);
        p.transform.localScale = Vector3.one * size;
        p.GetComponent<Renderer>().material = LoadMat(name + "Material", color);

        var o = p.AddComponent<OrbitRotation>();
        o.orbitCenter = sun;
        o.orbitSpeed = orbitSpeed;
        o.selfRotationSpeed = spinSpeed;

        AddClick(p, name, fact, highlight);
        if (withRings) BuildRings(p.transform);
        return p;
    }

    void BuildRings(Transform planet)
    {
        // Flat ring made of a stretched, unlit cylinder primitive.
        GameObject r = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        r.name = "Rings";
        r.transform.SetParent(planet, false);
        r.transform.localScale = new Vector3(2.0f, 0.02f, 2.0f);
        r.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
        DestroyImmediate(r.GetComponent<Collider>());
        r.GetComponent<Renderer>().material = MakeUnlitColor(
            new Color(0.95f, 0.85f, 0.55f, 1f));
    }

    void BuildMoon(string name, Transform parent, float distance, float size,
        Color color, float orbitSpeed, string fact, Color highlight)
    {
        GameObject m = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m.name = name;
        m.transform.position = parent.position + new Vector3(distance, 0.4f, 0f);
        m.transform.localScale = Vector3.one * size;
        m.GetComponent<Renderer>().material = LoadMat(name + "Material", color);

        var o = m.AddComponent<OrbitRotation>();
        o.orbitCenter = parent;
        o.orbitSpeed = orbitSpeed;
        o.selfRotationSpeed = 25f;

        AddClick(m, name, fact, highlight);
    }

    void BuildComet(Transform sun)
    {
        GameObject c = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        c.name = "Comet";
        c.transform.position = sun.position + new Vector3(22f, 4f, 0f);
        c.transform.localScale = Vector3.one * 0.5f;
        c.GetComponent<Renderer>().material = LoadMat(
            "CometMaterial", new Color(0.8f, 0.95f, 1f),
            emissive: true, emission: new Color(0.6f, 0.8f, 1f) * 1.2f);

        var o = c.AddComponent<OrbitRotation>();
        o.orbitCenter = sun;
        o.orbitSpeed = 40f;
        o.orbitAxis = new Vector3(0.3f, 1f, 0.2f).normalized;
        o.selfRotationSpeed = 80f;

        var trail = c.AddComponent<TrailRenderer>();
        trail.time = 1.5f;
        trail.startWidth = 0.4f;
        trail.endWidth = 0.0f;
        trail.material = MakeUnlitColor(new Color(0.7f, 0.9f, 1f, 1f));
        var grad = new Gradient();
        grad.SetKeys(
            new[] {
                new GradientColorKey(new Color(0.8f, 0.95f, 1f), 0f),
                new GradientColorKey(new Color(0.3f, 0.5f, 1f), 1f) },
            new[] {
                new GradientAlphaKey(0.9f, 0f),
                new GradientAlphaKey(0.0f, 1f) });
        trail.colorGradient = grad;
        trail.minVertexDistance = 0.05f;

        AddClick(c, "Comet",
            "A comet is a giant snowball racing through space, leaving a glowing tail!",
            new Color(0.7f, 0.9f, 1f));
    }

    // ───────────────────────── Cameras ─────────────────────────
    Camera BuildMainCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            cam = go.AddComponent<Camera>();
            go.AddComponent<AudioListener>();
        }
        cam.transform.position = new Vector3(0f, 8f, -22f);
        cam.transform.rotation = Quaternion.Euler(18f, 0f, 0f);
        cam.backgroundColor = new Color(0.01f, 0.01f, 0.04f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.farClipPlane = 200f;
        if (cam.GetComponent<CameraFocusController>() == null)
            cam.gameObject.AddComponent<CameraFocusController>();
        return cam;
    }

    Camera BuildMinimapCamera()
    {
        GameObject go = new GameObject("MinimapCamera");
        Camera mm = go.AddComponent<Camera>();
        mm.transform.position = new Vector3(0f, 45f, 0f);
        mm.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        mm.orthographic = true;
        mm.orthographicSize = 22f;
        mm.clearFlags = CameraClearFlags.SolidColor;
        mm.backgroundColor = new Color(0.02f, 0.02f, 0.08f);
        mm.farClipPlane = 100f;

        var rt = new RenderTexture(256, 256, 16);
        rt.name = "MinimapRT";
        mm.targetTexture = rt;
        return mm;
    }

    // ───────────────────────── UI ─────────────────────────
    void BuildUI(Camera cam)
    {
        GameObject cv = new GameObject("Canvas");
        Canvas c = cv.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        var sc = cv.AddComponent<CanvasScaler>();
        sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        sc.referenceResolution = new Vector2(1920, 1080);
        cv.AddComponent<GraphicRaycaster>();

        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        MakeText(cv.transform, "Explore the Solar System!",
            new Vector2(0, -50), 60, TextAnchor.UpperCenter,
            new Color(1f, 0.95f, 0.6f));
        MakeText(cv.transform,
            "Click any planet, the Sun, the Moon, or the comet to learn about it.\n" +
            "Press ESC or tap Back to return to space.",
            new Vector2(0, 70), 26, TextAnchor.LowerCenter, Color.white);

        GameObject panel = MakePanel(cv.transform, out Text title, out Text fact);
        Button back = MakeBackButton(cv.transform);
        MakeMinimap(cv.transform);

        var ctrl = cam.GetComponent<CameraFocusController>();
        ctrl.backButton = back;

        AudioClip click = MakeClickClip();
        foreach (var cc in Object.FindObjectsByType<ClickableCelestial>(FindObjectsSortMode.None))
        {
            cc.infoPanel = panel;
            cc.titleText = title;
            cc.factText = fact;
            cc.cameraController = ctrl;
            cc.clickSound = click;
        }
    }

    void MakeMinimap(Transform parent)
    {
        Camera mm = null;
        foreach (var cam in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
            if (cam.name == "MinimapCamera") { mm = cam; break; }
        if (mm == null || mm.targetTexture == null) return;

        GameObject frame = new GameObject("MinimapFrame");
        frame.transform.SetParent(parent, false);
        var frt = frame.AddComponent<RectTransform>();
        frt.anchorMin = frt.anchorMax = frt.pivot = new Vector2(1f, 1f);
        frt.anchoredPosition = new Vector2(-30, -30);
        frt.sizeDelta = new Vector2(280, 280);
        var fimg = frame.AddComponent<Image>();
        fimg.color = new Color(0.1f, 0.15f, 0.35f, 0.85f);

        GameObject img = new GameObject("MinimapImage");
        img.transform.SetParent(frame.transform, false);
        var irt = img.AddComponent<RectTransform>();
        irt.anchorMin = Vector2.zero; irt.anchorMax = Vector2.one;
        irt.offsetMin = new Vector2(8, 8); irt.offsetMax = new Vector2(-8, -8);
        var raw = img.AddComponent<RawImage>();
        raw.texture = mm.targetTexture;

        MakeText(frame.transform, "Map", new Vector2(0, -16), 22,
            TextAnchor.UpperCenter, new Color(1f, 0.95f, 0.6f));
    }

    // ───────────────────────── Helpers ─────────────────────────
    Material LoadMat(string name, Color fallback, bool emissive = false, Color? emission = null)
    {
        // Try a real material asset in Assets/Resources first (this is the
        // "prefabs / asset" hook). Fall back to a procedural Standard material.
        var preset = Resources.Load<Material>(name);
        if (preset != null)
        {
            var inst = new Material(preset);
            if (emissive)
            {
                inst.EnableKeyword("_EMISSION");
                if (emission.HasValue && inst.HasProperty("_EmissionColor"))
                    inst.SetColor("_EmissionColor", emission.Value);
            }
            return inst;
        }
        Shader sh = Shader.Find("Standard") ?? Shader.Find("Universal Render Pipeline/Lit");
        Material m = new Material(sh);
        if (m.HasProperty("_Color")) m.color = fallback;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", fallback);
        if (emissive)
        {
            m.EnableKeyword("_EMISSION");
            if (emission.HasValue && m.HasProperty("_EmissionColor"))
                m.SetColor("_EmissionColor", emission.Value);
        }
        return m;
    }

    Material MakeUnlitColor(Color c)
    {
        Shader sh = Shader.Find("Unlit/Color") ?? Shader.Find("Standard");
        Material m = new Material(sh);
        if (m.HasProperty("_Color")) m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        return m;
    }

    void AddClick(GameObject go, string n, string fact, Color hi)
    {
        var c = go.AddComponent<ClickableCelestial>();
        c.objectName = n;
        c.funFact = fact;
        c.highlightColor = hi;
        c.highlightIntensity = 1.8f;
        go.AddComponent<ClickPop>();        // visual scale-pulse
    }

    Text MakeText(Transform parent, string content, Vector2 pos, int size,
        TextAnchor anchorPreset, Color col)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        Vector2 anc =
            anchorPreset == TextAnchor.UpperCenter ? new Vector2(0.5f, 1f) :
            anchorPreset == TextAnchor.LowerCenter ? new Vector2(0.5f, 0f) :
            new Vector2(0.5f, 0.5f);
        rt.anchorMin = anc; rt.anchorMax = anc; rt.pivot = anc;
        rt.anchoredPosition = pos; rt.sizeDelta = new Vector2(1500, 140);
        Text t = go.AddComponent<Text>();
        t.text = content;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = size; t.alignment = TextAnchor.MiddleCenter; t.color = col;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        return t;
    }

    GameObject MakePanel(Transform parent, out Text title, out Text fact)
    {
        GameObject p = new GameObject("InfoPanel");
        p.transform.SetParent(parent, false);
        var rt = p.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0, -320);
        rt.sizeDelta = new Vector2(1200, 280);
        var img = p.AddComponent<Image>();
        img.color = new Color(0.05f, 0.1f, 0.3f, 0.9f);
        title = MakeText(p.transform, "", new Vector2(0, 80), 56,
            TextAnchor.MiddleCenter, new Color(1f, 0.85f, 0.3f));
        fact = MakeText(p.transform, "", new Vector2(0, -30), 34,
            TextAnchor.MiddleCenter, Color.white);
        p.SetActive(false);
        return p;
    }

    Button MakeBackButton(Transform parent)
    {
        GameObject b = new GameObject("BackButton");
        b.transform.SetParent(parent, false);
        var rt = b.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(30, -30);
        rt.sizeDelta = new Vector2(220, 90);
        var img = b.AddComponent<Image>();
        img.color = new Color(0.3f, 0.6f, 1f, 0.95f);
        var btn = b.AddComponent<Button>();

        GameObject txt = new GameObject("Text");
        txt.transform.SetParent(b.transform, false);
        var trt = txt.AddComponent<RectTransform>();
        trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
        trt.offsetMin = trt.offsetMax = Vector2.zero;
        Text t = txt.AddComponent<Text>();
        t.text = "< Back to Space";
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = 28; t.alignment = TextAnchor.MiddleCenter; t.color = Color.white;
        return btn;
    }

    AudioClip MakeClickClip()
    {
        // Two-tone friendly "blip" with an exponential decay envelope.
        int sr = 44100; float dur = 0.18f; int n = (int)(sr * dur);
        float[] d = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t = (float)i / sr;
            float env = Mathf.Exp(-t * 18f);
            d[i] = (Mathf.Sin(2f * Mathf.PI * 587f * t) * 0.5f
                  + Mathf.Sin(2f * Mathf.PI * 880f * t) * 0.4f) * env;
        }
        var clip = AudioClip.Create("KidsClickBlip", n, 1, sr, false);
        clip.SetData(d, 0);
        return clip;
    }
}
