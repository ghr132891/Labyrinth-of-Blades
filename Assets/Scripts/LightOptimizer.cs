using UnityEngine;
using UnityEngine.Rendering.Universal; // 引入 2D URP 命名空间

public class LightOptimizer : MonoBehaviour
{
    [Header("核心配置")]
    [Tooltip("Lumi 的 Transform。如果不手动拖拽，代码会自动寻找")]
    public Transform lumi;

    [Tooltip("光源的剔除距离。建议比摄像机的视野范围稍微大一点，防止屏幕边缘出现光源突然亮起的突兀感")]
    public float cullDistance = 25f;

    [Tooltip("检测频率（秒）。不需要每帧检测，0.2秒检测一次足够了")]
    public float checkInterval = 0.2f;

    private Light2D targetLight;
    private float timer;

    void Start()
    {
        // 自动获取当前物体上的 Light2D 组件
        targetLight = GetComponent<Light2D>();

        // 如果没有手动赋值 Lumi，尝试通过标签寻找（假设 Lumi 的 Tag 设置为 Player）
        if (lumi == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                lumi = player.transform;
            }
        }

        // 增加一点随机初始时间，防止场景里所有的灯光在同一帧进行距离计算造成卡顿
        timer = Random.Range(0f, checkInterval);
    }

    void Update()
    {
        if (lumi == null || targetLight == null) return;

        // 降频检测优化
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            CheckDistance();
        }
    }

    private void CheckDistance()
    {
        // 计算当前光源和 Lumi 的距离（如果是严格的 2D 游戏，可以使用 Vector2.Distance 忽略 Z 轴）
        float distance = Vector2.Distance(transform.position, lumi.position);

        // 如果距离小于设定值，说明该亮；否则关闭
        bool shouldBeOn = distance <= cullDistance;

        // 只有在状态发生变化时才修改 enabled，避免重复赋值
        if (targetLight.enabled != shouldBeOn)
        {
            targetLight.enabled = shouldBeOn;
        }
    }
}