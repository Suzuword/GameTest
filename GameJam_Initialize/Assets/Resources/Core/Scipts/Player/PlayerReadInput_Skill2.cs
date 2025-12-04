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
    // 动画哈希值
    private int chargeProgressHash;
    private int isChargingHash;
    private int isChargedHash;
    private int slashTriggerHash;

    void Start()
    {
        // 初始化组件
        animator = GetComponent<Animator>();
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
    }

    void Update()
    {
        // 检测输入状态
        chargeInputPressed = chargeAction.ReadValue<float>() > 0.5f;

        //某些状态下禁止蓄力
        if(moveAndJump._isGrounded == false)chargeInputPressed = false;
        if(attackScript.currentComboStep != 0) chargeInputPressed = false;

        // 状态机更新
        switch (currentState)
        {
            case ChargeState.Idle:
                HandleIdleState();
                break;
            case ChargeState.Charging:
                HandleChargingState();
                break;
            case ChargeState.Charged:
                HandleChargedState();
                break;
            case ChargeState.Slashing:
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
    /// 动画事件：斩击动画开始
    /// </summary>
    public void OnSlashStart()
    {
        Debug.Log("斩击开始 - 可在此处添加攻击判定");

        // 示例：根据蓄力时间计算伤害
        // float damageMultiplier = 1.0f + (currentChargeTime / chargeTimeRequired);
        // ApplyDamage(damageMultiplier);
    }

    /// <summary>
    /// 动画事件：斩击动画结束
    /// </summary>
    public void OnSlashEnd()
    {
        // 斩击完成，回到空闲状态
        currentState = ChargeState.Idle;
        currentChargeTime = 0f;

        Debug.Log("斩击结束，回到站立状态");
    }

    /// <summary>
    /// 动画事件：蓄力完成动画开始
    /// </summary>
    public void OnChargedStart()
    {
        Debug.Log("进入蓄力完成状态，等待释放");
        // 可以在此处添加特效、音效等
    }

    /// <summary>
    /// 外部调用：更新蓄力时间要求
    /// </summary>
    public void UpdateChargeTime(float newChargeTime)
    {
        chargeTimeRequired = newChargeTime;

        // 重新计算动画速度
        float animationSpeed = chargeTimeRequired / chargingAnimationLength;
        animator.SetFloat("ChargeSpeed", animationSpeed);

        Debug.Log($"更新蓄力时间: {chargeTimeRequired}秒, 动画速度: {animationSpeed:F2}");
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

    /// <summary>
    /// 调试用GUI显示
    /// </summary>
    //void OnGUI()
    //{
    //    GUI.Label(new Rect(10, 10, 300, 20), $"状态: {currentState}");
    //    GUI.Label(new Rect(10, 30, 300, 20), $"蓄力时间: {currentChargeTime:F2}/{chargeTimeRequired:F1}秒");
    //    GUI.Label(new Rect(10, 50, 300, 20), $"蓄力进度: {GetChargeProgress() * 100:F0}%");
    //    GUI.Label(new Rect(10, 70, 300, 20), $"按键状态: {(chargeInputPressed ? "按下" : "松开")}");

    //    // 进度条可视化
    //    Rect progressBarBg = new Rect(10, 95, 200, 20);
    //    GUI.Box(progressBarBg, "");

    //    Rect progressBar = new Rect(12, 97, 196 * GetChargeProgress(), 16);
    //    GUI.Box(progressBar, "");
    //}
}
