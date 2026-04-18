using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;
    [SerializeField] private GameObject levelRoot;

    [Header("区域翻转设置")]
    [Tooltip("当前所在的区域中心点。如果不填，默认围绕 (0,0,0) 翻转")]
    public Transform currentFlipCenter;

    [Header("世界切换设置")]
    [SerializeField] private float switchCooldown = 1f;
    private float lastSwitchTime = -Mathf.Infinity;

    [Header("时间世界设置")]
    [Range(0.01f, 2f)]
    public float timeWorldScale = 0.3f; // 注意：必须是 public，让玩家读取

    public WorldType currentWorld = WorldType.Normal;

    // 【核心修复1】：单独记录镜像状态，使其可以和时间减速叠加！
    public bool isMirrored { get; private set; }

    public event Action<WorldType> OnWorldChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject newRoot = GameObject.Find("LevelRoot");
        if (newRoot != null) levelRoot = newRoot;
        ApplyWorldVisuals();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3)) TrySwitchWorld(WorldType.Normal);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TrySwitchWorld(WorldType.Time);
        if (Input.GetKeyDown(KeyCode.Alpha5)) TrySwitchWorld(WorldType.Mirror);
    }

    private void TrySwitchWorld(WorldType inputType)
    {
        if (Time.unscaledTime >= lastSwitchTime + switchCooldown)
        {
            SwitchWorld(inputType);
        }
        else
        {
            //控制台打印冷却提示
            float remainingTime = (lastSwitchTime + switchCooldown) - Time.unscaledTime;
            Debug.Log($"世界切换冷却中: {remainingTime:F1}秒");
        }
    }

    public void SwitchWorld(WorldType inputType)
    {
        lastSwitchTime = Time.unscaledTime;

        if (inputType == WorldType.Normal)
        {
            // 按 3 键：强制回到普通世界
            SetNormalWorld();
        }
        else if (inputType == WorldType.Mirror)
        {
            if (currentWorld == WorldType.Mirror)
            {
                // 如果当前已经在纯粹的“镜像世界”，再按 5 则关闭镜像，退回普通世界
                SetNormalWorld();
            }
            else
            {
                // 【核心修复】：无论从普通世界还是时间世界过来，按5就是强制进入镜像世界
                // 1. 关闭时间减速（如果有的话）
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
                if (Player.instance != null)
                    Player.instance.SetTimeImmunity(false);

                // 2. 强制开启镜像
                isMirrored = true;
                currentWorld = WorldType.Mirror;
            }
        }
        else if (inputType == WorldType.Time)
        {
            if (currentWorld == WorldType.Time)
            {
                // 如果当前已经在“时间世界”，再按 4 则关闭时间
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
                if (Player.instance != null)
                    Player.instance.SetTimeImmunity(false);

                // 【人性化细节】：关闭时间后，根据当前的画面状态，决定退回到镜像世界还是普通世界
                currentWorld = isMirrored ? WorldType.Mirror : WorldType.Normal;
            }
            else
            {
                // 从 普通 或 镜像 切换到时间世界
                Time.timeScale = timeWorldScale;
                Time.fixedDeltaTime = 0.02f * timeWorldScale;
                if (Player.instance != null)
                    Player.instance.SetTimeImmunity(true);

                // 注意：这里【不修改】 isMirrored！如果刚才地图是翻转的，它就继续保持翻转
                currentWorld = WorldType.Time;
            }
        }

        ApplyWorldVisuals();
        OnWorldChanged?.Invoke(currentWorld);
    }
    private void SetNormalWorld()
    {
        isMirrored = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        if (Player.instance != null)
            Player.instance.SetTimeImmunity(false);
        currentWorld = WorldType.Normal;
    }
    // ======== 【核心修改：支持围绕任意中心点翻转】 ========
    private void ApplyWorldVisuals()
    {
        if (levelRoot == null) return;

        // 获取当前的中心点 X 坐标
        float centerX = currentFlipCenter != null ? currentFlipCenter.position.x : 0f;

        bool wasMirrored = levelRoot.transform.localScale.x < 0;

        if (isMirrored)
        {
            // 地图缩放翻转
            levelRoot.transform.localScale = new Vector3(-1, 1, 1);
            // 为了让指定区域看起来在原地翻转，我们需要把整个大地图平移 "2倍的中心点距离"
            levelRoot.transform.position = new Vector3(2 * centerX, levelRoot.transform.position.y, levelRoot.transform.position.z);
        }
        else
        {
            // 恢复正常
            levelRoot.transform.localScale = new Vector3(1, 1, 1);
            levelRoot.transform.position = new Vector3(0, levelRoot.transform.position.y, levelRoot.transform.position.z);
        }

        // 同步玩家的位置
        if (wasMirrored != isMirrored && Player.instance != null)
        {
            Vector3 playerPos = Player.instance.transform.position;

            // 数学魔法：围绕 centerX 翻转玩家坐标
            playerPos.x = (2 * centerX) - playerPos.x;

            Player.instance.transform.position = playerPos;

            Rigidbody2D rb = Player.instance.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // 防穿模
            }

            Debug.Log($"已围绕 X={centerX} 翻转地图。玩家传送到: {playerPos.x}");
        }
    }
}