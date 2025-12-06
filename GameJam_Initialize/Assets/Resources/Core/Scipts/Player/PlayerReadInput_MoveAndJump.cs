using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerReadInput_MoveAndJump : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // 移动速度

    [Header("Jump Settings")]
    public float jumpForce = 10f; // 跳跃力量
    public float groundCheckDistance = 0.2f; // 地面检测距离
    public LayerMask groundLayer; // 地面图层，需要在Inspector中设置
    public Transform groundCheckPoint; // 地面检测点，需要在Inspector中设置

    private Vector2 _movementInput; // 存储输入值的变量
    private Rigidbody2D _rb; // 刚体引用
    public bool _isGrounded; // 是否在地面上

    Animator playerAni;

    bool isJump;

    PlayerReadInput_Attack attackScript;
    PlayerReadInput_Skill2 skill2;
    PlayerReadInput_Skill3 skill3;
   public Shanxian shanxian;

    // 在Start中获取组件
    void Start()
    {
        playerAni = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();

        isJump = false;
    
        attackScript = GetComponent<PlayerReadInput_Attack>();
        skill2 = GetComponent<PlayerReadInput_Skill2>();
        skill3 = GetComponent<PlayerReadInput_Skill3>();
        shanxian = GetComponent<Shanxian>();

        // 使用射线向下检测地面

        // 如果没有设置地面检测点，使用角色自身的位置
        if (groundCheckPoint == null)
        {
            groundCheckPoint = transform;
        }
    }

    // 在FixedUpdate中处理物理移动和地面检测
    void FixedUpdate()
    {
        //更新朝向
        UpdateToward();

        //// 检测是否在地面上
        //CheckGrounded();

        // 创建一个移动方向向量
        Vector2 movement = new Vector2(_movementInput.x, 0) * moveSpeed;

        // 使用刚体设置水平速度，保持垂直速度不变
        _rb.velocity = new Vector2(movement.x, _rb.velocity.y);

        if (skill2.currentState != PlayerReadInput_Skill2.ChargeState.Idle)
        {
            _rb.velocity = new Vector2(0, 0);
        }

        if (skill3.currentState != PlayerReadInput_Skill3.DefenseState.Idle)
        {
            _rb.velocity = new Vector2(0, 0);
        }
    }

    // 更新时检测地面（更准确）
    void Update()
    {
        CheckGrounded();

        if (attackScript.currentComboStep == 0 && skill2.currentState == PlayerReadInput_Skill2.ChargeState.Idle && skill3.currentState == PlayerReadInput_Skill3.DefenseState.Idle)
        {
            if (_isGrounded)
            {
                if (_movementInput.x == 0f)
                {
                    playerAni.Play("PlayerStand");
                    AudioMgr.Instance.StopSound("主角走路音效");
                }
                else
                {
                    playerAni.Play("PlayerRun");
                    AudioSource loopSource = AudioMgr.Instance.PlaySoundLoop("主角走路音效");
                }
            }
        }
        else
        {
            AudioMgr.Instance.StopSound("主角走路音效");
        }


        if(isJump&& _rb.velocity.y < 0)
        {
            playerAni.Play("PlayerJumpToFall");
            playerAni.SetBool("isJump", false);
            isJump = false;
        }
    }

    void UpdateToward()
    {
        switch (_movementInput.x) {
            case 1.0f:
                this.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                shanxian.Destination=new Vector2(math.abs( shanxian.Destination.x), shanxian.Destination.y);
                shanxian.Offset=new Vector2(math.abs(shanxian.Offset.x), shanxian.Offset.y);
                break;
            case-1.0f:
                this.transform.rotation = Quaternion.Euler(Vector3.zero);
                shanxian.Destination = new Vector2(-math.abs(shanxian.Destination.x), shanxian.Destination.y);
                shanxian.Offset = new Vector2(-math.abs(shanxian.Offset.x), shanxian.Offset.y);
                break;
            default:
                break;
        }

    }

    // 地面检测方法
    void CheckGrounded()
    {
            RaycastHit2D hit = Physics2D.Raycast(
            groundCheckPoint.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
          );
        // 或者使用圆形检测（通常更可靠）
        // Collider2D[] colliders = Physics2D.OverlapCircleAll(
        //     groundCheckPoint.position, 
        //     groundCheckDistance, 
        //     groundLayer
        // );
        // _isGrounded = colliders.Length > 0;

        _isGrounded = hit.collider != null;

        // 可选：可视化调试射线
        Debug.DrawRay(
            groundCheckPoint.position,
            Vector2.down * groundCheckDistance,
            _isGrounded ? Color.green : Color.red
        );
    }

    // 这个函数将由Player Input组件在收到"Move"输入时自动调用
    public void OnMove(InputAction.CallbackContext context)
    {
        // 从输入系统中读取Vector2类型的输入值
        _movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context){
        playerAni.SetBool("isJump", true);
        isJump = true;
        // 只在按键按下的瞬间执行
        if (context.started)
        {
            // 只有在地面上才能跳跃
            if (_isGrounded)
            {
                // 方法1：直接设置垂直速度（简单直接）
                //_rb.velocity = new Vector2(_rb.velocity.x, jumpForce);

                // 方法2：使用AddForce（更物理化）
                // _rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

                // 方法3：使用AddForce并保留部分水平动量
                _rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

                // 可选：跳跃音效或粒子效果
                // PlayJumpSound();
                // SpawnJumpParticles();
            }
            else
            {
                // 可选：播放无法跳跃的反馈（声音、UI提示等）
                //Debug.Log("Not grounded!");
            }
        }
    }

    // 可选：二段跳功能
    // private int _jumpCount = 0;
    // private int _maxJumps = 2; // 最大跳跃次数

    // public void JumpWithDoubleJump(InputAction.CallbackContext context)
    // {
    //     if (context.started)
    //     {
    //         // 重置跳跃计数当接触地面时
    //         if (_isGrounded)
    //         {
    //             _jumpCount = 0;
    //         }
    //         
    //         // 如果还有跳跃次数
    //         if (_jumpCount < _maxJumps)
    //         {
    //             // 重置垂直速度以获得一致的跳跃高度
    //             _rb.velocity = new Vector2(_rb.velocity.x, 0f);
    //             
    //             // 执行跳跃
    //             _rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    //             
    //             _jumpCount++;
    //         }
    //     }
    // }

    // 可选：土狼时间（Coyote Time）功能 - 允许离开平台后短时间内仍能跳跃
    // private float _coyoteTime = 0.1f;
    // private float _coyoteTimeCounter;
    // 
    // void Update()
    // {
    //     CheckGrounded();
    //     
    //     if (_isGrounded)
    //     {
    //         _coyoteTimeCounter = _coyoteTime;
    //     }
    //     else
    //     {
    //         _coyoteTimeCounter -= Time.deltaTime;
    //     }
    // }
    // 
    // public void JumpWithCoyoteTime(InputAction.CallbackContext context)
    // {
    //     if (context.started && _coyoteTimeCounter > 0f)
    //     {
    //         _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
    //         _coyoteTimeCounter = 0f;
    //     }
    // }
}