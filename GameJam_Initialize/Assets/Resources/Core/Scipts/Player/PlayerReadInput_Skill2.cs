using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerReadInput_Skill2 : MonoBehaviour
{
    [Header("蓄力配置")]
    [SerializeField] private float chargeTimeRequired = 0.95f; // 所需蓄力时间
    [SerializeField] private float chargingAnimationLength = 1.0f; // 蓄力动画原始长度

    [Header("组件引用")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform attackPoint; // 攻击生成点

    [Header("剑气配置")]
    [SerializeField] private GameObject swordAuraPrefab; // 剑气预制体
    [SerializeField] private float auraSpeed = 15f; // 剑气速度
    [SerializeField] private Vector2 auraSize = new Vector2(3f, 1f); // 剑气大小
    [SerializeField] private float auraMaxDistance = 10f; // 剑气最大距离

    private PlayerInput playerInput;
    private InputAction chargeAction;

    // 蓄力状态
    public enum ChargeState { Idle, Charging, Charged, Slashing }
    public ChargeState currentState = ChargeState.Idle;

    // 蓄力计时
    private float currentChargeTime = 0f;
    public bool chargeInputPressed = false;

    PlayerReadInput_MoveAndJump moveAndJump;
    PlayerReadInput_Attack attackScript;
    PlayerReadInput_Skill3 skill3;

    private SwardEffect currentAura; // 当前剑气实例

    public float skillCD = 1f;
    public bool skillReady;

    // 动画哈希值
    private int chargeProgressHash;
    private int isChargingHash;
    private int isChargedHash;
    private int slashTriggerHash;

    void Start()
    {
        // 初始化组件
        animator = GetComponent<Animator>();
        attackPoint = this.transform;

        playerInput = GetComponent<PlayerInput>();

        // 获取输入动作
        chargeAction = playerInput.actions["GaLiBang"];

        // 设置动画参数哈希
        chargeProgressHash = Animator.StringToHash("ChargeProgress");
        isChargingHash = Animator.StringToHash("IsCharging");
        isChargedHash = Animator.StringToHash("IsCharged");
        slashTriggerHash = Animator.StringToHash("SlashTrigger");

        // 计算并设置动画速度
        float animationSpeed = chargeTimeRequired / chargingAnimationLength;
        animator.SetFloat("ChargeSpeed", animationSpeed);

        moveAndJump = GetComponent<PlayerReadInput_MoveAndJump>();
        attackScript = GetComponent<PlayerReadInput_Attack>();
        skill3 = GetComponent<PlayerReadInput_Skill3>();

        skillReady = true;
    }

    void Update()
    {
        // 检测输入状态
        chargeInputPressed = chargeAction.ReadValue<float>() > 0.5f;

        //某些状态下禁止蓄力
        if (moveAndJump._isGrounded == false) chargeInputPressed = false;
        if (attackScript.currentComboStep != 0) chargeInputPressed = false;
        if (skill3.currentState != PlayerReadInput_Skill3.DefenseState.Idle) chargeInputPressed = false;
        if (!skillReady) chargeInputPressed = false;

        // 状态机更新
        switch (currentState)
        {
            case ChargeState.Idle:
                HandleIdleState();
                break;
            case ChargeState.Charging:
                AudioMgr.Instance.PlaySound("咖喱棒二技能");
                HandleChargingState();
                break;
            case ChargeState.Charged:
                HandleChargedState();
                break;
            case ChargeState.Slashing:
                AudioMgr.Instance.PlaySound("平a一段");
                HandleSlashingState();
                break;
        }

        // 更新动画参数
        UpdateAnimatorParameters();
    }

    /// <summary>
    /// 空闲状态处理
    /// </summary>
    private void HandleIdleState()
    {
        // 按下K键开始蓄力
        if (chargeInputPressed)
        {
            StartCharging();
        }
    }

    /// <summary>
    /// 开始蓄力
    /// </summary>
    private void StartCharging()
    {
        currentState = ChargeState.Charging;
        currentChargeTime = 0f;

        Debug.Log("开始蓄力");
    }

    /// <summary>
    /// 蓄力中状态处理
    /// </summary>
    private void HandleChargingState()
    {
        // 累积蓄力时间
        if (chargeInputPressed)
        {
            currentChargeTime += Time.deltaTime;

            // 检查是否蓄力完成
            if (currentChargeTime >= chargeTimeRequired)
            {
                CompleteCharging();
            }
        }
        else // 提前松开按键
        {
            CancelCharging();
        }
    }

    /// <summary>
    /// 完成蓄力
    /// </summary>
    private void CompleteCharging()
    {
        currentState = ChargeState.Charged;
        currentChargeTime = chargeTimeRequired; // 确保不会超过

        Debug.Log("蓄力完成！");
    }

    /// <summary>
    /// 蓄力完成状态处理
    /// </summary>
    private void HandleChargedState()
    {
        // 松开按键释放斩击
        if (!chargeInputPressed)
        {
            ReleaseSlash();
        }

        // 可选：在蓄力完成状态保持一段时间后自动释放或取消
        // 这里保持等待按键释放
    }

    /// <summary>
    /// 释放斩击
    /// </summary>
    private void ReleaseSlash()
    {
        currentState = ChargeState.Slashing;
        animator.SetTrigger(slashTriggerHash);

        // 进入冷却
        StartCoroutine(getInCD());

        Debug.Log("释放斩击！");
    }

    /// <summary>
    /// 斩击状态处理
    /// </summary>
    private void HandleSlashingState()
    {
        // 斩击动画播放中，等待动画结束
        // 动画事件会调用OnSlashEnd()来返回Idle状态
    }

    /// <summary>
    /// 取消蓄力
    /// </summary>
    private void CancelCharging()
    {
        currentState = ChargeState.Idle;
        currentChargeTime = 0f;

        Debug.Log("取消蓄力");
    }

    /// <summary>
    /// 更新Animator参数
    /// </summary>
    private void UpdateAnimatorParameters()
    {
        // 计算蓄力进度(0-1)
        float progress = Mathf.Clamp01(currentChargeTime / chargeTimeRequired);

        animator.SetFloat(chargeProgressHash, progress);
        animator.SetBool(isChargingHash, currentState == ChargeState.Charging);
        animator.SetBool(isChargedHash, currentState == ChargeState.Charged);
    }

    /// <summary>
    /// 动画事件：蓄力完成开始
    /// 由PlayerSkill2_GaliBangHold动画中的OnChargedStart事件调用
    /// </summary>
    public void OnChargedStart()
    {
        // 这里可以添加蓄力完成时的特效、音效等
        Debug.Log("蓄力完成动画事件触发");

        // 示例：播放蓄力完成特效
        // Instantiate(chargeCompleteEffect, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// 动画事件：斩击动画开始
    /// 由斩击动画中的事件调用
    /// </summary>
    public void OnSlashStart()//蓄力斩击攻击帧起点执行
    {
        // 生成剑气
        if (swordAuraPrefab != null && attackPoint != null)
        {
            // 确定飞行方向（根据角色面向）
            Vector3 flightDirection = GetFacingDirection();

            // 生成剑气实例
            GameObject auraObj = Instantiate(swordAuraPrefab, attackPoint.position, Quaternion.identity);

            // 获取剑气组件
            currentAura = auraObj.GetComponent<SwardEffect>();

            if (currentAura != null)
            {
                // 设置剑气参数
                currentAura.SetFlightSpeed(auraSpeed);
                currentAura.SetFinalSize(auraSize);
                currentAura.SetMaxFlightDistance(auraMaxDistance);

                // 开始飞行（使用攻击点位置和角色面向方向）
                currentAura.StartFlight(attackPoint.position + new Vector3(0,1.5f,0), flightDirection);
            }
            else
            {
                Debug.LogWarning("剑气预制体上没有找到SwordAura组件！");
            }
        }
        else
        {
            Debug.LogWarning("剑气预制体或攻击点未设置！");
        }
    }

    /// <summary>
    /// 获取角色面向方向
    /// </summary>
    private Vector3 GetFacingDirection()
    {
        // 根据角色的旋转判断面向方向
        // 假设角色在Y轴旋转0度时面向右，180度时面向左
        bool isFacingRight = transform.rotation.eulerAngles.y == 0;
        return isFacingRight ? Vector3.left : Vector3.right;
    }

    /// <summary>
    /// 获取当前蓄力进度(0-1)
    /// </summary>
    public float GetChargeProgress()
    {
        return Mathf.Clamp01(currentChargeTime / chargeTimeRequired);
    }

    /// <summary>
    /// 获取当前状态
    /// </summary>
    public ChargeState GetCurrentState()
    {
        return currentState;
    }

    IEnumerator getInCD()
    {
        skillReady = false;
        yield return new WaitForSeconds(skillCD);
        skillReady = true;
    }

    /// <summary>
    /// 动画事件：斩击动画结束
    /// </summary>
    public void OnSlashEnd()
    {
        // 返回空闲状态
        currentState = ChargeState.Idle;
        currentChargeTime = 0f;
    }
}