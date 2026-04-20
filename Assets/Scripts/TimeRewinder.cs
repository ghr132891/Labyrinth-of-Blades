using System.Collections.Generic;
using UnityEngine;

public struct PointInTime
{
    // 【修复1】：必须记录全局坐标(position)，绝对不能用局部坐标！
    public Vector3 position;
    public Vector2 velocity;
    public int facingDir;
}

public class TimeRewinder : MonoBehaviour
{
    [Header("倒流设置")]
    [Tooltip("最多记录多少秒的物理轨迹")]
    public float recordTime = 5f;

    [Header("回放控制")]
    [Tooltip("1.0表示自然跟随时间流速回放；数值越大倒放越快")]
    public float rewindSpeedMultiplier = 1.0f;

    private List<PointInTime> pointsInTime;
    private Rigidbody2D rb;
    private Entity entity;

    private bool isDead;
    private bool isRewinding = false;
    private RigidbodyType2D originalBodyType;

    private float recordInterval = 0.02f;
    private float frameAccumulator = 0f;

    void Start()
    {
        pointsInTime = new List<PointInTime>();
        rb = GetComponent<Rigidbody2D>();
        entity = GetComponent<Entity>();

        if (WorldManager.Instance != null)
            WorldManager.Instance.OnWorldChanged += CheckTimeWorld;
    }

    private void CheckTimeWorld(WorldType worldType)
    {
        if (isDead) return;

        if (worldType == WorldType.Time)
            StartRewind();
        else
            StopRewind();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (isRewinding)
        {
            // ======== 【核心新增：时停拦截】 ========
            // 如果在时间世界中按下了时停，直接冻结当前状态，不弹出录像帧！
            if (WorldManager.Instance != null && WorldManager.Instance.isTimeStopped)
            {
                // 确保速度归零，稳稳悬停
                if (rb != null) rb.linearVelocity = Vector2.zero;
                return;
            }
            // ========================================

            frameAccumulator += rewindSpeedMultiplier;

            while (frameAccumulator >= 1f && pointsInTime.Count > 0)
            {
                ApplyRewindFrame();
                frameAccumulator -= 1f;
            }

            if (pointsInTime.Count == 0 && rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
        else
        {
            Record();
        }
    }

    private void ApplyRewindFrame()
    {
        PointInTime pointInTime = pointsInTime[0];

        if (rb != null && rb.bodyType == RigidbodyType2D.Kinematic)
        {
            // 使用世界坐标 MovePosition
            rb.MovePosition(pointInTime.position);
        }
        else
        {
            transform.position = pointInTime.position;
        }

        if (entity != null && pointInTime.facingDir != entity.facingDir)
        {
            entity.Flip();
        }

        pointsInTime.RemoveAt(0);
    }

    void Record()
    {
        if (pointsInTime.Count > Mathf.Round(recordTime / recordInterval))
        {
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        }

        PointInTime point = new PointInTime();
        // 记录世界坐标
        point.position = transform.position;
        point.velocity = rb != null ? rb.linearVelocity : Vector2.zero;
        point.facingDir = entity != null ? entity.facingDir : 1;

        pointsInTime.Insert(0, point);
    }

    public void StartRewind()
    {
        if (isDead) return;

        isRewinding = true;
        frameAccumulator = 0f;

        if (rb != null)
        {
            originalBodyType = rb.bodyType;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        if (entity != null) entity.enabled = false;
    }

    public void StopRewind()
    {
        isRewinding = false;

        if (rb != null)
        {
            rb.bodyType = originalBodyType;
            if (pointsInTime.Count > 0) rb.linearVelocity = pointsInTime[0].velocity;
        }
        if (entity != null) entity.enabled = true;
    }

    public void SetDead()
    {
        isDead = true;
        pointsInTime.Clear();
        if (rb != null)
            rb.bodyType = originalBodyType;
    }

    private void OnDestroy()
    {
        if (WorldManager.Instance != null)
            WorldManager.Instance.OnWorldChanged -= CheckTimeWorld;
    }
}