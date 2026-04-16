using System.Collections.Generic;
using UnityEngine;

public struct PointInTime
{
    public Vector3 position;
    public Vector2 velocity;
    public int facingDir;
}

public class TimeRewinder : MonoBehaviour
{
    [Header("倒流设置")]
    public float recordTime = 5f;
    private List<PointInTime> pointsInTime;
    private Rigidbody2D rb;
    private Entity entity;

    private bool isRewinding = false;

    // 【新增】：用于记录真实的刚体类型，避免 isKinematic 警告
    private RigidbodyType2D originalBodyType;

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
        if (worldType == WorldType.Time)
            StartRewind();
        else
            StopRewind();
    }

    void FixedUpdate()
    {
        if (isRewinding)
            Rewind();
        else
            Record();
    }

    void Rewind()
    {
        if (pointsInTime.Count > 0)
        {
            PointInTime pointInTime = pointsInTime[0];

            transform.position = pointInTime.position;
            if (entity != null && pointInTime.facingDir != entity.facingDir)
            {
                entity.Flip();
            }

            pointsInTime.RemoveAt(0);
        }
        else
        {
            // 【修复】：使用 bodyType 替代 isKinematic
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    void Record()
    {
        if (pointsInTime.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
        {
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        }

        PointInTime point = new PointInTime();
        point.position = transform.position;
        point.velocity = rb != null ? rb.linearVelocity : Vector2.zero;
        point.facingDir = entity != null ? entity.facingDir : 1;

        pointsInTime.Insert(0, point);
    }

    public void StartRewind()
    {
        isRewinding = true;
        // 【修复】：记录原始状态，并使用 bodyType 替代 isKinematic
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
        // 【修复】：还原原始的 bodyType
        if (rb != null)
        {
            rb.bodyType = originalBodyType;
            if (pointsInTime.Count > 0) rb.linearVelocity = pointsInTime[0].velocity;
        }
        if (entity != null) entity.enabled = true;
    }

    private void OnDestroy()
    {
        if (WorldManager.Instance != null)
            WorldManager.Instance.OnWorldChanged -= CheckTimeWorld;
    }
}