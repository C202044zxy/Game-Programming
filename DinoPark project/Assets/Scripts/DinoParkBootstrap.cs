using UnityEngine;
using UnityEngine.UI;

/// Builds the whole Interactive Dinosaur Park scene at runtime.
/// Drop this on an empty GameObject in any scene and press Play.
public class DinoParkBootstrap : MonoBehaviour
{
    void Awake()
    {
        BuildLights();
        BuildEnvironment();
        BuildCreatures();
        Camera cam = BuildCamera();
        BuildUI(cam);
    }

    void BuildLights()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.55f, 0.5f, 0.35f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.9f, 0.8f, 0.6f);
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 20f;
        RenderSettings.fogEndDistance = 80f;

        GameObject dirGO = new GameObject("Sunlight");
        Light dir = dirGO.AddComponent<Light>();
        dir.type = LightType.Directional;
        dir.color = new Color(1f, 0.92f, 0.7f);
        dir.intensity = 1.3f;
        dirGO.transform.rotation = Quaternion.Euler(50f, -25f, 0f);
    }

    void BuildEnvironment()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = new Vector3(0f, -2f, 0f);
        ground.transform.localScale = new Vector3(6f, 1f, 6f);
        ground.GetComponent<Renderer>().material = MakeMaterial(new Color(0.35f, 0.55f, 0.2f));

        // A few trees in the background
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * 14f, -1f, Mathf.Sin(angle) * 10f + 6f);
            SpawnTree(pos);
        }
    }

    void SpawnTree(Vector3 pos)
    {
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "TreeTrunk";
        trunk.transform.position = pos;
        trunk.transform.localScale = new Vector3(0.5f, 2f, 0.5f);
        trunk.GetComponent<Renderer>().material = MakeMaterial(new Color(0.4f, 0.25f, 0.12f));
        Destroy(trunk.GetComponent<Collider>());

        GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaves.name = "TreeLeaves";
        leaves.transform.SetParent(trunk.transform, false);
        leaves.transform.localPosition = new Vector3(0f, 1.2f, 0f);
        leaves.transform.localScale = new Vector3(2.5f, 1.7f, 2.5f);
        leaves.GetComponent<Renderer>().material = MakeMaterial(new Color(0.2f, 0.5f, 0.2f));
        Destroy(leaves.GetComponent<Collider>());
    }

    void BuildCreatures()
    {
        SpawnTRex(new Vector3(-5f, -2f, 2f));
        SpawnTriceratops(new Vector3(0f, -2f, 0f));
        SpawnPterodactyl(new Vector3(5f, 2f, 1f));
    }

    void SpawnTRex(Vector3 pos)
    {
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "TRex";
        body.transform.position = pos + new Vector3(0f, 2.5f, 0f);
        body.transform.localScale = new Vector3(1.5f, 2f, 1.5f);
        Color green = new Color(0.3f, 0.55f, 0.25f);
        body.GetComponent<Renderer>().material = MakeMaterial(green);

        // Head
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
        head.transform.SetParent(body.transform, false);
        head.transform.localPosition = new Vector3(0f, 0.7f, 0.6f);
        head.transform.localScale = new Vector3(0.6f, 0.4f, 0.9f);
        head.GetComponent<Renderer>().material = MakeMaterial(green);
        Destroy(head.GetComponent<Collider>());

        // Tail
        GameObject tail = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tail.transform.SetParent(body.transform, false);
        tail.transform.localPosition = new Vector3(0f, 0.2f, -0.9f);
        tail.transform.localScale = new Vector3(0.35f, 0.35f, 1.2f);
        tail.GetComponent<Renderer>().material = MakeMaterial(green * 0.85f);
        Destroy(tail.GetComponent<Collider>());

        AttachDino(body, "T-Rex",
            "T-Rex was a giant meat-eater with tiny arms and huge teeth!",
            new Color(1f, 0.5f, 0.4f), bobAmount: 0.2f);
    }

    void SpawnTriceratops(Vector3 pos)
    {
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Triceratops";
        body.transform.position = pos + new Vector3(0f, 1.5f, 0f);
        body.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        body.transform.localScale = new Vector3(1.5f, 2f, 1.5f);
        Color brown = new Color(0.55f, 0.4f, 0.25f);
        body.GetComponent<Renderer>().material = MakeMaterial(brown);

        // Frill
        GameObject frill = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frill.transform.SetParent(body.transform, false);
        frill.transform.localPosition = new Vector3(-1.1f, 0f, 0f);
        frill.transform.localScale = new Vector3(0.3f, 1.2f, 1.2f);
        frill.GetComponent<Renderer>().material = MakeMaterial(brown * 1.15f);
        Destroy(frill.GetComponent<Collider>());

        // Horn
        GameObject horn = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        horn.transform.SetParent(body.transform, false);
        horn.transform.localPosition = new Vector3(-1.6f, 0.4f, 0f);
        horn.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        horn.transform.localScale = new Vector3(0.1f, 0.4f, 0.1f);
        horn.GetComponent<Renderer>().material = MakeMaterial(new Color(0.9f, 0.85f, 0.7f));
        Destroy(horn.GetComponent<Collider>());

        AttachDino(body, "Triceratops",
            "Triceratops had three horns and a huge bony frill to protect itself!",
            new Color(1f, 0.8f, 0.3f), bobAmount: 0.1f);
    }

    void SpawnPterodactyl(Vector3 pos)
    {
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Pterodactyl";
        body.transform.position = pos;
        body.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        body.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
        Color purple = new Color(0.55f, 0.35f, 0.6f);
        body.GetComponent<Renderer>().material = MakeMaterial(purple);

        // Wings
        GameObject leftWing = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftWing.transform.SetParent(body.transform, false);
        leftWing.transform.localPosition = new Vector3(0f, 0f, 1.2f);
        leftWing.transform.localScale = new Vector3(0.1f, 3f, 1.6f);
        leftWing.GetComponent<Renderer>().material = MakeMaterial(purple * 0.85f);
        Destroy(leftWing.GetComponent<Collider>());

        GameObject rightWing = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightWing.transform.SetParent(body.transform, false);
        rightWing.transform.localPosition = new Vector3(0f, 0f, -1.2f);
        rightWing.transform.localScale = new Vector3(0.1f, 3f, 1.6f);
        rightWing.GetComponent<Renderer>().material = MakeMaterial(purple * 0.85f);
        Destroy(rightWing.GetComponent<Collider>());

        AttachDino(body, "Pterodactyl",
            "Pterodactyls were flying reptiles with huge wings made of skin!",
            new Color(1f, 0.6f, 1f), bobAmount: 0.5f);
    }

    void AttachDino(GameObject body, string name, string fact, Color highlight, float bobAmount)
    {
        var bob = body.AddComponent<IdleBob>();
        bob.basePosition = body.transform.position;
        bob.amount = bobAmount;
        bob.speed = 1.2f + Random.value * 0.6f;

        var cc = body.AddComponent<ClickableCelestial>();
        cc.objectName = name;
        cc.funFact = fact;
        cc.highlightColor = highlight;
        cc.highlightIntensity = 1.8f;
    }

    Camera BuildCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            cam = camGO.AddComponent<Camera>();
            camGO.AddComponent<AudioListener>();
        }
        cam.transform.position = new Vector3(0f, 3f, -16f);
        cam.transform.rotation = Quaternion.Euler(10f, 0f, 0f);
        cam.backgroundColor = new Color(0.85f, 0.75f, 0.55f);
        cam.clearFlags = CameraClearFlags.SolidColor;

        if (cam.GetComponent<CameraFocusController>() == null)
            cam.gameObject.AddComponent<CameraFocusController>();
        return cam;
    }

    void BuildUI(Camera cam)
    {
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        CreateText(canvasGO.transform, "TitleText", "Dinosaur Park",
            new Vector2(0, -60), new Vector2(900, 90), 54, TextAnchor.MiddleCenter,
            new Color(0.4f, 0.2f, 0.05f), TextAnchor.UpperCenter);

        CreateText(canvasGO.transform, "HintText",
            "Click a dinosaur to learn about it!  (Press ESC or Back to return)",
            new Vector2(0, 60), new Vector2(1200, 50), 28, TextAnchor.MiddleCenter,
            new Color(0.2f, 0.1f, 0f), TextAnchor.LowerCenter);

        GameObject panel = new GameObject("InfoPanel");
        panel.transform.SetParent(canvasGO.transform, false);
        RectTransform panelRT = panel.AddComponent<RectTransform>();
        panelRT.anchorMin = panelRT.anchorMax = panelRT.pivot = new Vector2(0.5f, 0.5f);
        panelRT.anchoredPosition = new Vector2(0, -300);
        panelRT.sizeDelta = new Vector2(1100, 260);
        panel.AddComponent<Image>().color = new Color(0.3f, 0.18f, 0.05f, 0.9f);

        Text title = CreateText(panel.transform, "Title", "",
            new Vector2(0, -45), new Vector2(1000, 70), 48, TextAnchor.MiddleCenter,
            new Color(1f, 0.85f, 0.3f), TextAnchor.UpperCenter);

        Text fact = CreateText(panel.transform, "Fact", "",
            new Vector2(0, -20), new Vector2(1000, 140), 32, TextAnchor.MiddleCenter,
            Color.white, TextAnchor.MiddleCenter);

        panel.SetActive(false);

        GameObject btnGO = new GameObject("BackButton");
        btnGO.transform.SetParent(canvasGO.transform, false);
        RectTransform btnRT = btnGO.AddComponent<RectTransform>();
        btnRT.anchorMin = btnRT.anchorMax = btnRT.pivot = new Vector2(0, 1);
        btnRT.anchoredPosition = new Vector2(30, -30);
        btnRT.sizeDelta = new Vector2(220, 80);
        btnGO.AddComponent<Image>().color = new Color(0.75f, 0.4f, 0.1f, 0.95f);
        Button btn = btnGO.AddComponent<Button>();

        Text btnText = CreateText(btnGO.transform, "Text", "< Back to Park",
            Vector2.zero, new Vector2(220, 80), 30, TextAnchor.MiddleCenter, Color.white, TextAnchor.MiddleCenter);
        RectTransform btnTextRT = btnText.GetComponent<RectTransform>();
        btnTextRT.anchorMin = Vector2.zero; btnTextRT.anchorMax = Vector2.one;
        btnTextRT.offsetMin = btnTextRT.offsetMax = Vector2.zero;

        CameraFocusController ctrl = cam.GetComponent<CameraFocusController>();
        ctrl.backButton = btn;

        AudioClip click = MakeRoarClip();
        foreach (var c in Object.FindObjectsByType<ClickableCelestial>(FindObjectsSortMode.None))
        {
            c.infoPanel = panel;
            c.titleText = title;
            c.factText = fact;
            c.cameraController = ctrl;
            c.clickSound = click;
        }
    }

    Text CreateText(Transform parent, string name, string content,
        Vector2 pos, Vector2 size, int fontSize, TextAnchor align,
        Color color, TextAnchor anchorPreset)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();

        Vector2 min = new Vector2(0.5f, 1f), max = new Vector2(0.5f, 1f), piv = new Vector2(0.5f, 1f);
        if (anchorPreset == TextAnchor.LowerCenter) { min = new Vector2(0.5f, 0f); max = new Vector2(0.5f, 0f); piv = new Vector2(0.5f, 0f); }
        else if (anchorPreset == TextAnchor.MiddleCenter) { min = max = piv = new Vector2(0.5f, 0.5f); }

        rt.anchorMin = min; rt.anchorMax = max; rt.pivot = piv;
        rt.anchoredPosition = pos; rt.sizeDelta = size;

        Text t = go.AddComponent<Text>();
        t.text = content;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = fontSize;
        t.alignment = align;
        t.color = color;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        return t;
    }

    Material MakeMaterial(Color c)
    {
        Shader shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
        Material m = new Material(shader);
        if (m.HasProperty("_Color")) m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        return m;
    }

    // Low rumbling roar, falling pitch with growl modulation
    AudioClip MakeRoarClip()
    {
        int sr = 44100;
        float dur = 0.45f;
        int samples = (int)(sr * dur);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sr;
            float freq = Mathf.Lerp(180f, 70f, t / dur);
            float growl = 1f + 0.3f * Mathf.Sin(2f * Mathf.PI * 18f * t);
            float env = Mathf.Min(1f, t * 20f) * Mathf.Exp(-t * 3f);
            data[i] = Mathf.Sin(2f * Mathf.PI * freq * growl * t) * env * 0.55f;
        }
        AudioClip clip = AudioClip.Create("DinoRoar", samples, 1, sr, false);
        clip.SetData(data, 0);
        return clip;
    }

    // Gentle idle up-down bob with slight yaw wiggle
    private class IdleBob : MonoBehaviour
    {
        public Vector3 basePosition;
        public float amount = 0.2f;
        public float speed = 1.2f;
        private float offset;
        private Quaternion baseRotation;

        void Start()
        {
            offset = Random.value * 10f;
            baseRotation = transform.rotation;
        }

        void Update()
        {
            float t = Time.time * speed + offset;
            transform.position = basePosition + Vector3.up * Mathf.Sin(t) * amount;
            transform.rotation = baseRotation * Quaternion.Euler(0f, Mathf.Sin(t * 0.5f) * 4f, 0f);
        }
    }
}
