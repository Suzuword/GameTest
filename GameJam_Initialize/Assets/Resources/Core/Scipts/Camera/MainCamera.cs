using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [Header("目标设置")]
    [Tooltip("摄像机要跟踪的目标对象")]
    public Transform target;

    [Header("跟随设置")]
    [Tooltip("摄像机跟随的平滑速度")]
    public float smoothSpeed = 0.125f;

    [Header("摄像机设置")]
    [Tooltip("正交摄像机的大小 - 控制视野范围")]
    public float orthographicSize = 5f;

    [Tooltip("摄像机Z轴位置")]
    public float cameraZPosition = -10f;

    [Header("边界限制")]
    [Tooltip("是否启用边界限制")]
    public bool useBounds = false;

    [Tooltip("摄像机移动的最小边界")]
    public Vector2 minBounds = new Vector2(-10f, -10f);

    [Tooltip("摄像机移动的最大边界")]
    public Vector2 maxBounds = new Vector2(10f, 10f);

    [Header("缩放设置")]
    [Tooltip("是否启用缩放功能")]
    public bool enableZoom = true;

    [Tooltip("目标缩放大小")]
    public float targetZoom = 5.75f;

    [Tooltip("最小缩放值")]
    public float minZoom = 3f;

    [Tooltip("最大缩放值")]
    public float maxZoom = 8f;

    [Tooltip("缩放平滑速度")]
    public float zoomSmoothSpeed = 2f;

    private Camera cam;
    private Vector3 velocity = Vector3.zero;

    /// <summary>
    /// 初始化组件
    /// </summary>
    public void Initialize()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        // 设置初始正交大小
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = orthographicSize;
        }

        // 设置摄像机Z轴位置
        transform.position = new Vector3(transform.position.x, transform.position.y, cameraZPosition);
    }

    /// <summary>
    /// 设置新的跟踪目标
    /// </summary>
    /// <param name="newTarget">新的目标Transform</param>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    /// <summary>
    /// 平滑跟随目标
    /// </summary>
    public void FollowTarget()
    {
        ////手动差值计算
        //Vector3 tarGetPosition = target.position;
        //tarGetPosition = ApplyBounds(tarGetPosition);
        //Vector3 moveToward = ((target.position - transform.position) * Vector3.Distance(target.position, transform.position)).normalized;
        //moveToward.z = 0;
        //this.transform.Translate(moveToward* 3*Vector3.Distance(target.position, transform.position) * Time.deltaTime);
        if (target == null) return;
        

        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, cameraZPosition);
        if (target.name == "Player") desiredPosition.y = desiredPosition.y + 2.3f;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

        // 应用边界限制
        if (useBounds)
        {
            smoothedPosition = ApplyBounds(smoothedPosition);
        }

        transform.position = smoothedPosition;
    }

    /// <summary>
    /// 应用边界限制
    /// </summary>
    /// <param name="position">要限制的位置</param>
    /// <returns>限制后的位置</returns>
    public Vector3 ApplyBounds(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
        return position;
    }

    /// <summary>
    /// 处理摄像机缩放
    /// </summary>
    public void HandleZoom()
    {
        if (!enableZoom || cam == null) return;

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSmoothSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 设置缩放级别
    /// </summary>
    /// <param name="zoomLevel">新的缩放级别</param>
    public void SetZoom(float zoomLevel)
    {
        targetZoom = Mathf.Clamp(zoomLevel, minZoom, maxZoom);
    }

    /// <summary>
    /// 立即跳转到目标位置（无平滑过渡）
    /// </summary>
    public void SnapToTarget()
    {
        if (target == null) return;

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, cameraZPosition);

        if (useBounds)
        {
            targetPosition = ApplyBounds(targetPosition);
        }

        transform.position = targetPosition;
    }

    /// <summary>
    /// 设置摄像机正交大小
    /// </summary>
    /// <param name="size">新的正交大小</param>
    public void SetOrthographicSize(float size)
    {
        orthographicSize = size;
        if (cam != null)
        {
            cam.orthographicSize = orthographicSize;
        }
    }

    /// <summary>
    /// 设置摄像机Z轴位置
    /// </summary>
    /// <param name="zPosition">新的Z轴位置</param>
    public void SetCameraZPosition(float zPosition)
    {
        cameraZPosition = zPosition;
        transform.position = new Vector3(transform.position.x, transform.position.y, cameraZPosition);
    }

    /// <summary>
    /// 设置边界
    /// </summary>
    /// <param name="min">最小边界</param>
    /// <param name="max">最大边界</param>
    public void SetBounds(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
        useBounds = true;
    }

    /// <summary>
    /// 禁用边界限制
    /// </summary>
    public void DisableBounds()
    {
        useBounds = false;
    }

    void Start()
    {
        Initialize();
    }

    void LateUpdate()
    {
        FollowTarget();

        if (enableZoom)
        {
            HandleZoom();
        }
    }

    // 在编辑器中绘制边界Gizmos
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (useBounds)
        {
            Gizmos.color = Color.green;
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) * 0.5f, (minBounds.y + maxBounds.y) * 0.5f, transform.position.z);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0.1f);
            Gizmos.DrawWireCube(center, size);
        }
    }
#endif

}
