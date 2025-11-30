using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerReadInput : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // 移动速度

    private Vector2 _movementInput; // 存储输入值的变量
    private Rigidbody2D _rb; // 刚体引用

    // 在Start中获取组件
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // 在FixedUpdate中处理物理移动
    void FixedUpdate()
    {
        // 创建一个移动方向向量
        Vector2 movement = new Vector3(_movementInput.x,  _movementInput.y) * moveSpeed;

        // 注意：如果你的角色不需要物理碰撞，可以直接用Transform：
         transform.Translate(movement * Time.deltaTime, Space.World);
    }

    // 这个函数将由Player Input组件在收到"Move"输入时自动调用
    public void OnMove(InputAction.CallbackContext context)
    {
        // 从输入系统中读取Vector2类型的输入值
        _movementInput = context.ReadValue<Vector2>();
    }
}
