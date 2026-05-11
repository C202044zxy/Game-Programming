#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

/// <summary>
/// Runs once after script compilation and builds both scenes automatically.
/// To re-run: Tools → Re-run Space Shooter Setup.
/// </summary>
[InitializeOnLoad]
public class ProjectSetup
{
    const string CHASER_SPAWNER   = "Assets/Prefabs/Enemies/Spawners/EnemySpawnerChaser.prefab";
    const string STRAIGHT_SPAWNER = "Assets/Prefabs/Enemies/Spawners/EnemySpawnerStraight.prefab";
    const string DIAGONAL_SPAWNER = "Assets/Prefabs/Enemies/Spawners/EnemySpawnerDiagonal.prefab";
    const string PLAYER_HIT_FX    = "Assets/Prefabs/Effects/Player/PlayerHitEffect.prefab";
    const string PLAYER_DEATH_FX  = "Assets/Prefabs/Effects/Player/PlayerDeathEffect.prefab";
    const string GAMEOVER_FX      = "Assets/Prefabs/Effects/Win&Lose/GameOverEffect.prefab";
    const string VICTORY_FX       = "Assets/Prefabs/Effects/Win&Lose/VictoryEffect.prefab";
    const string PLAYER_PROJ      = "Assets/Prefabs/Projectiles/Player_Projectiles/Player_Projectile.prefab";
    const string PLAYER_SPRITE    = "Assets/Art/Player/Player Sprites.png";
    const string BG_SPRITE        = "Assets/Art/Environment/Background/A_CompleteSpaceBackground.png";
    const string SONG_A           = "Assets/Audio/Music/SongA.wav";
    const string SONG_MENU        = "Assets/Audio/Music/Menu.wav";
    const string SPEEDBOOST_PF    = "Assets/Prefabs/PowerUps/SpeedBoost.prefab";
    const string FIRERATE_PF      = "Assets/Prefabs/PowerUps/FireRateBoost.prefab";
    const string HEALTHRESTORE_PF = "Assets/Prefabs/PowerUps/HealthRestore.prefab";

    static ProjectSetup() => EditorApplication.delayCall += TrySetup;

    [MenuItem("Tools/Setup Space Shooter Project")]
    public static void TrySetup()
    {
        if (EditorPrefs.GetBool("SpaceShooterSetupDone_v4", false)) return;
        Run();
    }

    [MenuItem("Tools/Re-run Space Shooter Setup")]
    public static void ForceSetup()
    {
        EditorPrefs.DeleteKey("SpaceShooterSetupDone_v4");
        Run();
    }

    static void Run()
    {
        AssetDatabase.Refresh();
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            AssetDatabase.CreateFolder("Assets", "Scenes");

        BuildGameScene();
        BuildMainMenuScene();
        SetBuildSettings();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorPrefs.SetBool("SpaceShooterSetupDone_v4", true);
        Debug.Log("[ProjectSetup] Done. Open MainMenu scene and press Play.");
    }

    // ════════════════════════════════════════════════════════════════════════
    // GAME SCENE
    // ════════════════════════════════════════════════════════════════════════
    static void BuildGameScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Background
        var bg = new GameObject("Background");
        var bgSr = bg.AddComponent<SpriteRenderer>();
        var bgSp = LoadSprite(BG_SPRITE);
        if (bgSp != null) bgSr.sprite = bgSp; else bgSr.color = new Color(0.04f, 0.04f, 0.1f);
        bgSr.sortingOrder = -10;
        bg.transform.localScale = new Vector3(35f, 20f, 1f);

        // Player
        var player = new GameObject("Player");
        player.tag = "Player";
        var pSr = player.AddComponent<SpriteRenderer>();
        var pSp = LoadSprite(PLAYER_SPRITE);
        if (pSp != null) pSr.sprite = pSp; else pSr.color = Color.cyan;
        pSr.sortingOrder = 1;

        var rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f; rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        player.AddComponent<CircleCollider2D>().radius = 0.4f;

        var ctrl = player.AddComponent<Controller>();
        ctrl.moveSpeed = 8f;
        ctrl.movementMode = Controller.MovementModes.FreeRoam;
        ctrl.aimMode     = Controller.AimModes.AimTowardsMouse;
        ctrl.myRigidbody = rb;
        ctrl.moveAction  = BuildWASDAction();
        ctrl.lookAction  = BuildMousePosAction("Look");
        EditorUtility.SetDirty(ctrl);

        var gun = player.AddComponent<ShootingController>();
        gun.isPlayerControlled = true;
        gun.fireRate            = 0.15f;
        gun.projectileSpread    = 2f;
        gun.projectilePrefab    = Load<GameObject>(PLAYER_PROJ);
        gun.fireAction          = new InputAction("Fire", InputActionType.Button, "<Mouse>/leftButton");
        EditorUtility.SetDirty(gun);

        var health = player.AddComponent<Health>();
        health.teamId = 0; health.defaultHealth = 3; health.maximumHealth = 3;
        health.currentHealth = 3; health.invincibilityTime = 1.5f;
        health.useLives = true; health.currentLives = 3; health.maximumLives = 5;
        health.hitEffect   = Load<GameObject>(PLAYER_HIT_FX);
        health.deathEffect = Load<GameObject>(PLAYER_DEATH_FX);

        // ProjectileHolder
        var projHolder = new GameObject("ProjectileHolder");

        // Camera
        var camGO = new GameObject("Main Camera"); camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true; cam.orthographicSize = 8f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.04f, 0.04f, 0.08f);
        camGO.transform.position = new Vector3(0, 0, -10);
        camGO.AddComponent<AudioListener>();

        var camCtrl = camGO.AddComponent<CameraController>();
        camCtrl.target = player.transform;
        camCtrl.cameraMovementStyle = CameraController.CameraStyles.Overhead;
        camCtrl.cameraZCoordinate = -10f;
        camCtrl.lookAction = BuildMousePosAction("CameraLook");
        EditorUtility.SetDirty(camCtrl);

        var camAudio = camGO.AddComponent<AudioSource>();
        camAudio.clip = Load<AudioClip>(SONG_A);
        camAudio.loop = true; camAudio.volume = 0.5f; camAudio.playOnAwake = true;

        // GameManager
        var gmGO = new GameObject("GameManager");
        var gm = gmGO.AddComponent<GameManager>();
        gm.victoryEffect  = Load<GameObject>(VICTORY_FX);
        gm.gameOverEffect = Load<GameObject>(GAMEOVER_FX);
        SetField(gm, "player", player);
        SetField(gm, "enemiesToDefeat", 15);

        // Enemy spawners
        SpawnEnemySpawner(CHASER_SPAWNER,   new Vector3(-9, 0, 0), player.transform, null);
        SpawnEnemySpawner(STRAIGHT_SPAWNER, new Vector3( 9, 0, 0), player.transform, projHolder.transform);
        SpawnEnemySpawner(DIAGONAL_SPAWNER, new Vector3( 0, 9, 0), player.transform, projHolder.transform);

        // Canvas
        var canvasGO = new GameObject("GameCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 10;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // HUD panel
        var hud = Child(canvasGO, "HUD"); FillParent(hud);
        var scoreGO = TMP(hud, "ScoreText",     "Score: 0",  20, TextAlignmentOptions.TopLeft,
            new Vector2(10,-10),  new Vector2(300,40), new Vector2(0,1), new Vector2(0,1));
        var hsGO    = TMP(hud, "HighScoreText", "High: 0",   20, TextAlignmentOptions.TopLeft,
            new Vector2(10,-55),  new Vector2(300,40), new Vector2(0,1), new Vector2(0,1));
        var hpGO    = TMP(hud, "HealthText",    "HP: 3 / 3", 20, TextAlignmentOptions.TopRight,
            new Vector2(-10,-10), new Vector2(300,40), new Vector2(1,1), new Vector2(1,1));
        var livGO   = TMP(hud, "LivesText",     "Lives: 3",  20, TextAlignmentOptions.TopRight,
            new Vector2(-10,-55), new Vector2(300,40), new Vector2(1,1), new Vector2(1,1));
        var objGO   = TMP(hud, "ObjectiveText", "",          18, TextAlignmentOptions.Center,
            new Vector2(0, 90),   new Vector2(900,80), new Vector2(0.5f,0f), new Vector2(0.5f,0f));

        scoreGO.AddComponent<ScoreDisplay>().displayText    = scoreGO.GetComponent<TextMeshProUGUI>();
        hsGO.AddComponent<HighScoreDisplay>().displayText   = hsGO.GetComponent<TextMeshProUGUI>();
        hpGO.AddComponent<HealthDisplay>().displayText      = hpGO.GetComponent<TextMeshProUGUI>();
        livGO.AddComponent<LivesDisplay>().displayText      = livGO.GetComponent<TextMeshProUGUI>();
        var objScript = objGO.AddComponent<ObjectiveText>();
        objScript.objectiveText = objGO.GetComponent<TextMeshProUGUI>();
        objScript.objectiveMessage =
            "GOAL: Destroy 15 enemies to win!\n" +
            "WASD = Move   Mouse = Aim   Left-Click = Fire   Esc = Pause\n" +
            "Collect power-ups: Yellow=Speed   Blue=Fire Rate   Green=Health";

        // UIManager
        var uiMgrGO = new GameObject("UIManager");
        var uiMgr = uiMgrGO.AddComponent<UIManager>();
        uiMgr.pauseAction = new InputAction("Pause", InputActionType.Button, "<Keyboard>/escape");
        EditorUtility.SetDirty(uiMgr);

        // UI Pages
        var victoryGO  = UIPage(canvasGO, "VictoryScreen",  false);
        var pauseGO    = UIPage(canvasGO, "PauseScreen",    false);
        var gameOverGO = UIPage(canvasGO, "GameOverScreen", false);

        // Victory page
        TMP(victoryGO, "Title", "YOU WIN!", 60, TextAlignmentOptions.Center,
            new Vector2(0,120), new Vector2(700,80), V2half, V2half);
        var vRetry = Btn(victoryGO, "PlayAgain", "Play Again", new Vector2(0,-20),  new Vector2(250,55));
        var vMenu  = Btn(victoryGO, "MainMenu",  "Main Menu",  new Vector2(0,-90),  new Vector2(250,55));

        // Pause page
        TMP(pauseGO, "Title", "PAUSED", 60, TextAlignmentOptions.Center,
            new Vector2(0,120), new Vector2(600,80), V2half, V2half);
        var pResume = Btn(pauseGO, "Resume",   "Resume",    new Vector2(0,-20), new Vector2(200,55));
        var pQuit   = Btn(pauseGO, "QuitGame", "Quit Game", new Vector2(0,-90), new Vector2(200,55));

        // Game Over page
        TMP(gameOverGO, "Title", "GAME OVER", 60, TextAlignmentOptions.Center,
            new Vector2(0,120), new Vector2(700,80), V2half, V2half);
        var goRetry = Btn(gameOverGO, "TryAgain", "Try Again", new Vector2(0,-20), new Vector2(250,55));
        var goMenu  = Btn(gameOverGO, "MainMenu", "Main Menu", new Vector2(0,-90), new Vector2(250,55));

        // Wire UIManager pages list
        var soUM = new SerializedObject(uiMgr);
        var pagesProp = soUM.FindProperty("pages");
        pagesProp.arraySize = 3;
        pagesProp.GetArrayElementAtIndex(0).objectReferenceValue = victoryGO.GetComponent<UIPage>();
        pagesProp.GetArrayElementAtIndex(1).objectReferenceValue = pauseGO.GetComponent<UIPage>();
        pagesProp.GetArrayElementAtIndex(2).objectReferenceValue = gameOverGO.GetComponent<UIPage>();
        soUM.FindProperty("pausePageIndex").intValue = 1;
        soUM.ApplyModifiedProperties();

        // Wire GameManager
        SetField(gm, "gameVictoryPageIndex", 0);
        SetField(gm, "gameOverPageIndex",    2);

        // Wire Victory buttons
        var llb_vR = vRetry.AddComponent<LevelLoadButton>();
        var sr_vR  = vRetry.AddComponent<ScoreReseter>();
        UnityEventTools.AddStringPersistentListener(vRetry.GetComponent<Button>().onClick,
            llb_vR.LoadLevelByName, "SampleScene");
        UnityEventTools.AddVoidPersistentListener(vRetry.GetComponent<Button>().onClick, sr_vR.ResetScore);

        var llb_vM = vMenu.AddComponent<LevelLoadButton>();
        UnityEventTools.AddStringPersistentListener(vMenu.GetComponent<Button>().onClick,
            llb_vM.LoadLevelByName, "MainMenu");

        // Wire Pause buttons
        UnityEventTools.AddVoidPersistentListener(pResume.GetComponent<Button>().onClick, uiMgr.TogglePause);
        var qgb = pQuit.AddComponent<QuitGameButton>();
        UnityEventTools.AddVoidPersistentListener(pQuit.GetComponent<Button>().onClick, qgb.QuitGame);

        // Wire Game Over buttons
        var llb_gR = goRetry.AddComponent<LevelLoadButton>();
        var sr_gR  = goRetry.AddComponent<ScoreReseter>();
        UnityEventTools.AddStringPersistentListener(goRetry.GetComponent<Button>().onClick,
            llb_gR.LoadLevelByName, "SampleScene");
        UnityEventTools.AddVoidPersistentListener(goRetry.GetComponent<Button>().onClick, sr_gR.ResetScore);

        var llb_gM = goMenu.AddComponent<LevelLoadButton>();
        UnityEventTools.AddStringPersistentListener(goMenu.GetComponent<Button>().onClick,
            llb_gM.LoadLevelByName, "MainMenu");

        // EventSystem
        var esGO = new GameObject("EventSystem");
        esGO.AddComponent<EventSystem>();
        esGO.AddComponent<InputSystemUIInputModule>();

        // PowerManager
        var pmGO   = new GameObject("PowerManager");
        var pSpawn = pmGO.AddComponent<PowerUpSpawner>();
        pSpawn.powerUpPrefabs    = new GameObject[] {
            Load<GameObject>(SPEEDBOOST_PF), Load<GameObject>(FIRERATE_PF), Load<GameObject>(HEALTHRESTORE_PF)
        };
        pSpawn.minSpawnInterval  = 8f;
        pSpawn.maxSpawnInterval  = 14f;
        pSpawn.spawnRangeX       = 7f;
        pSpawn.spawnRangeY       = 4f;
        pSpawn.powerUpLifetime   = 10f;
        pmGO.AddComponent<DifficultyScaler>();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/SampleScene.unity");
        Debug.Log("[ProjectSetup] Saved SampleScene");
    }

    // ════════════════════════════════════════════════════════════════════════
    // MAIN MENU SCENE
    // ════════════════════════════════════════════════════════════════════════
    static void BuildMainMenuScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var bg = new GameObject("Background");
        var bgSr = bg.AddComponent<SpriteRenderer>();
        var bgSp = LoadSprite(BG_SPRITE);
        if (bgSp != null) bgSr.sprite = bgSp; else bgSr.color = new Color(0.04f, 0.04f, 0.1f);
        bgSr.sortingOrder = -10;
        bg.transform.localScale = new Vector3(35f, 20f, 1f);

        var camGO = new GameObject("Main Camera"); camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true; cam.orthographicSize = 8f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.04f, 0.04f, 0.08f);
        camGO.transform.position = new Vector3(0, 0, -10);
        camGO.AddComponent<AudioListener>();
        var camAudio = camGO.AddComponent<AudioSource>();
        camAudio.clip = Load<AudioClip>(SONG_MENU); camAudio.loop = true;
        camAudio.volume = 0.5f; camAudio.playOnAwake = true;

        var gmGO = new GameObject("GameManager");
        gmGO.AddComponent<GameManager>();

        var canvasGO = new GameObject("UICanvas");
        var cnv = canvasGO.AddComponent<Canvas>();
        cnv.renderMode = RenderMode.ScreenSpaceOverlay;
        var sc = canvasGO.AddComponent<CanvasScaler>();
        sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        sc.referenceResolution = new Vector2(1920, 1080); sc.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        var uiMgrGO = new GameObject("UIManager");
        var uiMgr = uiMgrGO.AddComponent<UIManager>();
        SetField(uiMgr, "allowPause", false);

        // Main menu page
        var menuPage = UIPage(canvasGO, "MainMenuPage", true);
        TMP(menuPage, "GameTitle",  "SPACE SHOOTER", 72, TextAlignmentOptions.Center,
            new Vector2(0,140), new Vector2(900,100), V2half, V2half);
        TMP(menuPage, "Subtitle",   "Defeat all enemies to win!", 28, TextAlignmentOptions.Center,
            new Vector2(0,60),  new Vector2(700,50),  V2half, V2half);
        var newGameBtn  = Btn(menuPage, "NewGameBtn",   "New Game",     new Vector2(0,-20),  new Vector2(260,55));
        var instrBtn    = Btn(menuPage, "InstructBtn",  "Instructions", new Vector2(0,-90),  new Vector2(260,55));
        var exitBtn     = Btn(menuPage, "ExitBtn",      "Exit",         new Vector2(0,-160), new Vector2(260,55));

        // Instructions page
        var instrPage = UIPage(canvasGO, "InstructionsPage", false);
        TMP(instrPage, "Title", "HOW TO PLAY", 48, TextAlignmentOptions.Center,
            new Vector2(0,150), new Vector2(700,70), V2half, V2half);
        TMP(instrPage, "Body",
            "WASD — Move your ship\n" +
            "Mouse — Aim\n" +
            "Left Click — Fire\n" +
            "Esc — Pause / Unpause\n\n" +
            "Collect power-ups:\n" +
            "  Yellow sphere = Speed Boost (5 sec)\n" +
            "  Blue sphere   = Rapid Fire  (5 sec)\n" +
            "  Green sphere  = Restore 2 HP\n\n" +
            "Destroy 15 enemies to win!\n" +
            "Difficulty increases every 50 points.",
            22, TextAlignmentOptions.Left,
            new Vector2(0,-10), new Vector2(680,420), V2half, V2half);
        var backBtn = Btn(instrPage, "BackBtn", "Back", new Vector2(0,-215), new Vector2(200,50));

        // Wire UIManager pages
        var soUM = new SerializedObject(uiMgr);
        var pp = soUM.FindProperty("pages");
        pp.arraySize = 2;
        pp.GetArrayElementAtIndex(0).objectReferenceValue = menuPage.GetComponent<UIPage>();
        pp.GetArrayElementAtIndex(1).objectReferenceValue = instrPage.GetComponent<UIPage>();
        soUM.FindProperty("defaultPage").intValue  = 0;
        soUM.FindProperty("allowPause").boolValue  = false;
        soUM.ApplyModifiedProperties();

        // Wire main menu buttons
        var llb_new = newGameBtn.AddComponent<LevelLoadButton>();
        var sr_new  = newGameBtn.AddComponent<ScoreReseter>();
        UnityEventTools.AddStringPersistentListener(newGameBtn.GetComponent<Button>().onClick,
            llb_new.LoadLevelByName, "SampleScene");
        UnityEventTools.AddVoidPersistentListener(newGameBtn.GetComponent<Button>().onClick, sr_new.ResetScore);

        // Instructions button: GoToPage(1) via SerializedObject
        AddIntPersistentListener(instrBtn.GetComponent<Button>(), uiMgr, "GoToPage", 1);

        // Back button: GoToPage(0)
        AddIntPersistentListener(backBtn.GetComponent<Button>(), uiMgr, "GoToPage", 0);

        var qgb = exitBtn.AddComponent<QuitGameButton>();
        UnityEventTools.AddVoidPersistentListener(exitBtn.GetComponent<Button>().onClick, qgb.QuitGame);

        var esGO = new GameObject("EventSystem");
        esGO.AddComponent<EventSystem>();
        esGO.AddComponent<InputSystemUIInputModule>();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
        Debug.Log("[ProjectSetup] Saved MainMenu");
    }

    // ════════════════════════════════════════════════════════════════════════
    // BUILD SETTINGS
    // ════════════════════════════════════════════════════════════════════════
    static void SetBuildSettings()
    {
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity",    true),
            new EditorBuildSettingsScene("Assets/Scenes/SampleScene.unity", true),
        };
    }

    // ════════════════════════════════════════════════════════════════════════
    // HELPERS
    // ════════════════════════════════════════════════════════════════════════
    static readonly Vector2 V2half = new Vector2(0.5f, 0.5f);

    static void SpawnEnemySpawner(string path, Vector3 pos, Transform target, Transform projHolder)
    {
        var prefab = Load<GameObject>(path);
        if (prefab == null) return;
        var go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        go.transform.position = pos;
        var es = go.GetComponent<EnemySpawner>();
        if (es != null)
        {
            es.target = target;
            es.spawnRangeX = 6f;
            es.spawnRangeY = 6f;
            if (projHolder != null) es.projectileHolder = projHolder;
        }
    }

    static Sprite LoadSprite(string path)
    {
        foreach (var o in AssetDatabase.LoadAllAssetsAtPath(path))
            if (o is Sprite s) return s;
        return null;
    }

    static T Load<T>(string path) where T : Object
    {
        var o = AssetDatabase.LoadAssetAtPath<T>(path);
        if (o == null) Debug.LogWarning("[ProjectSetup] Missing: " + path);
        return o;
    }

    // Set a serialized field by name (bypasses access modifiers)
    static void SetField(Object target, string field, object value)
    {
        var so = new SerializedObject(target);
        var p  = so.FindProperty(field);
        if (p == null) { Debug.LogWarning("[ProjectSetup] Field not found: " + field); return; }
        switch (value)
        {
            case int    i: p.intValue        = i; break;
            case bool   b: p.boolValue       = b; break;
            case float  f: p.floatValue      = f; break;
            case string s: p.stringValue     = s; break;
            case Object o: p.objectReferenceValue = o; break;
        }
        so.ApplyModifiedProperties();
    }

    static void FillParent(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    static GameObject Child(GameObject parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        return go;
    }

    static GameObject TMP(GameObject parent, string name, string text, float size,
        TextAlignmentOptions align, Vector2 pos, Vector2 sd, Vector2 aMin, Vector2 aMax)
    {
        var go = Child(parent, name);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = aMin; rt.anchorMax = aMax;
        rt.anchoredPosition = pos; rt.sizeDelta = sd;
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text; t.fontSize = size; t.color = Color.white; t.alignment = align;
        return go;
    }

    static GameObject Btn(GameObject parent, string name, string label, Vector2 pos, Vector2 sd)
    {
        var go = Child(parent, name);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = V2half; rt.anchorMax = V2half;
        rt.anchoredPosition = pos; rt.sizeDelta = sd;
        var img = go.AddComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.35f, 0.9f);
        go.AddComponent<Button>();

        var lbl = Child(go, "Text");
        var lrt = lbl.AddComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;
        var t = lbl.AddComponent<TextMeshProUGUI>();
        t.text = label; t.fontSize = 22; t.color = Color.white;
        t.alignment = TextAlignmentOptions.Center;
        return go;
    }

    static GameObject UIPage(GameObject parent, string name, bool active)
    {
        var go = Child(parent, name);
        FillParent(go);
        var img = go.AddComponent<Image>();
        img.color = active ? new Color(0,0,0,0) : new Color(0,0,0,0.75f);
        go.AddComponent<UIPage>();
        go.SetActive(active);
        return go;
    }

    // Add a persistent listener that calls method(int) on a UnityEvent (Button.onClick)
    static void AddIntPersistentListener(Button btn, Object target, string method, int arg)
    {
        var so    = new SerializedObject(btn);
        var calls = so.FindProperty("m_OnClick.m_PersistentCalls.m_Calls");
        int idx   = calls.arraySize++;
        var call  = calls.GetArrayElementAtIndex(idx);
        call.FindPropertyRelative("m_Target").objectReferenceValue       = target;
        call.FindPropertyRelative("m_TargetAssemblyTypeName").stringValue =
            target.GetType().AssemblyQualifiedName;
        call.FindPropertyRelative("m_MethodName").stringValue            = method;
        call.FindPropertyRelative("m_Mode").intValue                     = 3; // Int
        call.FindPropertyRelative("m_Arguments.m_IntArgument").intValue  = arg;
        call.FindPropertyRelative("m_CallState").intValue                = 2; // RuntimeOnly
        so.ApplyModifiedProperties();
    }

    // Build WASD 2D Vector composite InputAction
    static InputAction BuildWASDAction()
    {
        var a = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
        a.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
        a.AddCompositeBinding("2DVector")
            .With("Up",    "<Keyboard>/upArrow").With("Down",  "<Keyboard>/downArrow")
            .With("Left",  "<Keyboard>/leftArrow").With("Right", "<Keyboard>/rightArrow");
        return a;
    }

    static InputAction BuildMousePosAction(string name) =>
        new InputAction(name, InputActionType.Value, "<Mouse>/position", expectedControlType: "Vector2");
}
#endif
