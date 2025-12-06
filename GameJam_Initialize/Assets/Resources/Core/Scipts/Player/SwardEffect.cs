using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwardEffect : MonoBehaviour
{
    [Header("剑气设置")]
    [SerializeField] private float flightSpeed = 10f; // 飞行速度
    [SerializeField] private Vector2 finalSize = new Vector2(2f, 2f); // 最终大小
    [SerializeField] private float sizeLerpSpeed = 5f; // 大小变化速度
    [SerializeField] private float maxFlightDistance = 20f; // 最大飞行距离
    [SerializeField] private bool autoDestroyOnStop = true; // 停止时是否自动销毁

    [Header("组件引用")]
    [SerializeField] private SpriteRenderer auraSpriteRenderer; // 剑气贴图渲染器
    [SerializeField] private Collider2D auraCollider; // 可选的碰撞器

    // 私有变量
    private Vector3 startPosition;
    private Vector3 flightDirection;
    private bool isFlying = false;
    private Vector3 currentDirection;

    // 属性
    public bool IsFlying => isFlying;
    public float CurrentSpeed => flightSpeed;
    public Vector2 CurrentSize => auraSpriteRenderer != null ?
        new Vector2(auraSpriteRenderer.transform.localScale.x, auraSpriteRenderer.transform.localScale.y) : Vector2.zero;

    private void Awake()
    {
        // 自动获取组件（如果未手动赋值）
        if (auraSpriteRenderer == null)
        {
            auraSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (auraCollider == null)
        {
            auraCollider = GetComponent<Collider2D>();
        }

        // 初始禁用（如果需要）
        if (auraSpriteRenderer != null)
        {
            auraSpriteRenderer.enabled = false;
        }

        if (auraCollider != null)
        {
            auraCollider.enabled = false;
        }
    }

    /// <summary>
    /// 开始飞行剑气
    /// </summary>
    /// <param name="startPos">起始位置</param>
    /// <param name="direction">飞行方向（如果不提供则使用当前朝向）</param>
    public void StartFlight(Vector3 startPos, Vector3? direction = null)
    {
        // 设置起始位置
        transform.position = startPos;
        startPosition = startPos;

        // 确定飞行方向
        if (direction.HasValue && direction.Value != Vector3.zero)
        {
            currentDirection = direction.Value.normalized;
        }
        else
        {
            // 使用当前对象的正前方作为方向
            currentDirection = transform.right;
        }

        // 确保方向不为零
        if (currentDirection == Vector3.zero)
        {
            currentDirection = Vector3.right;
        }

        // 设置朝向
        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 激活状态
        isFlying = true;

        // 启用组件
        if (auraSpriteRenderer != null)
        {
            auraSpriteRenderer.enabled = true;
        }

        if (auraCollider != null)
        {
            auraCollider.enabled = true;
        }

        // 重置大小到初始状态
        if (auraSpriteRenderer != null)
        {
            auraSpriteRenderer.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 开始飞行剑气（使用当前位置和当前朝向）
    /// </summary>
    public void StartFlight()
    {
        StartFlight(transform.position, transform.right);
    }

    /// <summary>
    /// 停止飞行剑气
    /// </summary>
    /// <param name="immediate">是否立即停止并隐藏</param>
    public void StopFlight(bool immediate = false)
    {
        if (!isFlying) return;

        isFlying = false;

        if (immediate)
        {
            // 立即隐藏
            if (auraSpriteRenderer != null)
            {
                auraSpriteRenderer.enabled = false;
            }

            if (auraCollider != null)
            {
                auraCollider.enabled = false;
            }

            if (autoDestroyOnStop)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // 可以在这里添加淡出效果等
            // 暂时先直接隐藏
            if (auraSpriteRenderer != null)
            {
                auraSpriteRenderer.enabled = false;
            }

            if (auraCollider != null)
            {
                auraCollider.enabled = false;
            }

            if (autoDestroyOnStop)
            {
                Destroy(gameObject, 0.5f); // 延迟销毁，以便可能添加特效
            }
        }
    }

    private void Update()
    {
        if (!isFlying) return;

        // 飞行移动
        Vector3 movement = currentDirection * flightSpeed * Time.deltaTime;
        transform.position += movement;

        // 渐变到最终大小
        if (auraSpriteRenderer != null)
        {
            Vector3 currentScale = auraSpriteRenderer.transform.localScale;
            Vector3 targetScale = new Vector3(finalSize.x, finalSize.y, 1f);
            auraSpriteRenderer.transform.localScale = Vector3.Lerp(currentScale, targetScale, Time.deltaTime * sizeLerpSpeed);
        }

        // 检查是否超出最大距离
        if (maxFlightDistance > 0)
        {
            float distanceFlown = Vector3.Distance(startPosition, transform.position);
            if (distanceFlown >= maxFlightDistance)
            {
                StopFlight();
            }
        }
    }

    /// <summary>
    /// 设置飞行速度
    /// </summary>
    /// <param name="speed">新的速度值</param>
    public void SetFlightSpeed(float speed)
    {
        flightSpeed = Mathf.Max(0, speed); // 确保速度不为负
    }

    /// <summary>
    /// 设置剑气最终大小
    /// </summary>
    /// <param name="size">新的最终大小</param>
    public void SetFinalSize(Vector2 size)
    {
        finalSize = new Vector2(
            Mathf.Max(0.1f, size.x), // 确保最小大小
            Mathf.Max(0.1f, size.y)
        );
    }

    /// <summary>
    /// 设置最大飞行距离
    /// </summary>
    /// <param name="distance">最大飞行距离（0表示无限）</param>
    public void SetMaxFlightDistance(float distance)
    {
        maxFlightDistance = Mathf.Max(0, distance);
    }

    /// <summary>
    /// 立即设置当前大小（不渐变）
    /// </summary>
    /// <param name="size">目标大小</param>
    public void SetSizeImmediate(Vector2 size)
    {
        if (auraSpriteRenderer != null)
        {
            auraSpriteRenderer.transform.localScale = new Vector3(size.x, size.y, 1f);
        }
    }

    /// <summary>
    /// 获取当前飞行方向
    /// </summary>
    /// <returns>归一化的飞行方向向量</returns>
    public Vector3 GetFlightDirection()
    {
        return currentDirection;
    }

    /// <summary>
    /// 获取已飞行的距离
    /// </summary>
    /// <returns>已飞行的距离</returns>
    public float GetFlownDistance()
    {
        return isFlying ? Vector3.Distance(startPosition, transform.position) : 0f;
    }

    private void OnBecameInvisible()
    {
        // 如果剑气离开屏幕，可以停止飞行（可选）
        if (isFlying)
        {
            StopFlight();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 碰撞检测示例（可根据需要扩展）
        if (isFlying)
        {
            // 检测到碰撞时可以停止或继续飞行
            // 例如：碰到敌人或障碍物时停止
            // if (other.CompareTag("Enemy") || other.CompareTag("Obstacle"))
            // {
            //     StopFlight();
            // }
        }
    }
}
