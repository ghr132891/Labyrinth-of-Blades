using System;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;

    [Header("世界切换设置")]
    [SerializeField] private float switchCooldown = 1f;
    private float lastSwitchTime = -Mathf.Infinity;

    [Header("时间世界设置")]
    [Range(0.01f, 2f)]
    public float timeWorldScale = 0.3f;

    public WorldType currentWorld = WorldType.Normal;
    public bool isMirrored { get; private set; }

    // 【新增】：全局时停标志位
    public bool isTimeStopped { get; private set; }

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3)) TrySwitchWorld(WorldType.Normal);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TrySwitchWorld(WorldType.Time);
        if (Input.GetKeyDown(KeyCode.Alpha5)) TrySwitchWorld(WorldType.Mirror);

        // 【新增】：按下 T 键触发时停开关
        if (Input.GetKeyDown(KeyCode.T)) TryToggleTimeStop();
    }
    private void TryToggleTimeStop()
    {
        // 只有在时间世界里，才能使用时停功能
        if (currentWorld == WorldType.Time)
        {
            isTimeStopped = !isTimeStopped;
            Debug.Log(isTimeStopped ? "时停已开启：万物静止！" : "时停已解除：时间继续流动！");
        }
    }

    private void TrySwitchWorld(WorldType inputType)
    {
        if (Time.unscaledTime >= lastSwitchTime + switchCooldown)
        {
            SwitchWorld(inputType);
        }
        else
        {
            float remainingTime = (lastSwitchTime + switchCooldown) - Time.unscaledTime;
            Debug.Log($"世界切换冷却中: {remainingTime:F1}秒");
        }
    }

    public void SwitchWorld(WorldType inputType)
    {
        lastSwitchTime = Time.unscaledTime;
        isTimeStopped = false;

        if (inputType == WorldType.Normal)
        {
            SetNormalWorld();
        }
        else if (inputType == WorldType.Mirror)
        {
            if (currentWorld == WorldType.Mirror)
            {
                SetNormalWorld();
            }
            else
            {
                // 关闭时间减速
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
                if (Player.instance != null) Player.instance.SetTimeImmunity(false);

                // 强制开启镜像
                isMirrored = true;
                currentWorld = WorldType.Mirror;
            }
        }
        else if (inputType == WorldType.Time)
        {
            if (currentWorld == WorldType.Time)
            {
                // 关闭时间，根据是否翻转决定退回哪个世界
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
                if (Player.instance != null) Player.instance.SetTimeImmunity(false);

                currentWorld = isMirrored ? WorldType.Mirror : WorldType.Normal;
            }
            else
            {
                // 进入时间世界
                Time.timeScale = timeWorldScale;
                Time.fixedDeltaTime = 0.02f * timeWorldScale;
                if (Player.instance != null) Player.instance.SetTimeImmunity(true);

                currentWorld = WorldType.Time;
            }
        }

        OnWorldChanged?.Invoke(currentWorld);
    }

    private void SetNormalWorld()
    {
        isMirrored = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        if (Player.instance != null) Player.instance.SetTimeImmunity(false);
        currentWorld = WorldType.Normal;
    }
}