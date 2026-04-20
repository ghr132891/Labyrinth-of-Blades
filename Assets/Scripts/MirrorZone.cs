using UnityEngine;

public class MirrorZone : MonoBehaviour
{
    private bool isCurrentlyMirrored = false;

    private void Start()
    {
        if (WorldManager.Instance != null)
        {
            WorldManager.Instance.OnWorldChanged += HandleWorldChanged;

            // 初始化检查：如果游戏刚加载时就是镜像状态，直接翻转一次
            if (WorldManager.Instance.isMirrored)
            {
                ExecuteFlip();
            }
        }
    }

    private void HandleWorldChanged(WorldType newWorld)
    {
        bool targetMirrored = WorldManager.Instance.isMirrored;

        // 状态发生实质改变时执行翻转
        if (isCurrentlyMirrored != targetMirrored)
        {
            ExecuteFlip();
        }
    }

    private void ExecuteFlip()
    {
        isCurrentlyMirrored = !isCurrentlyMirrored;

        // 遍历当前镜像区域内的所有子物体（平台、箱子、敌人）
        foreach (Transform child in transform)
        {
            // 1. 物理位置翻转：以 MirrorZone 中心点为基准，X 轴坐标对调
            child.localPosition = new Vector3(-child.localPosition.x, child.localPosition.y, child.localPosition.z);

            // 2. 状态与朝向翻转
            Entity entity = child.GetComponent<Entity>();
            if (entity != null)
            {
                // 如果是敌人/实体：调用你的 Entity 原生翻转逻辑，AI 和视线都会完美转过来
                entity.Flip();

                // 物理惯性补偿：如果敌人正在冲锋，翻转后速度也要反向，防止滑步
                Rigidbody2D rb = child.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
                }
            }
            else
            {
                // 如果是普通的平台、墙体、地刺：直接翻转 X 缩放实现视觉和碰撞体的镜像
                child.localScale = new Vector3(-child.localScale.x, child.localScale.y, child.localScale.z);
            }
        }
    }

    private void OnDestroy()
    {
        if (WorldManager.Instance != null)
        {
            WorldManager.Instance.OnWorldChanged -= HandleWorldChanged;
        }
    }
}