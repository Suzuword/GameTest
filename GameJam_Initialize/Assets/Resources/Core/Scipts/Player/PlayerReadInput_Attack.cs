using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerReadInput_Attack : MonoBehaviour
{
    [Header("连击配置")]
    [SerializeField] private float comboWindow = 0.5f; // 连击窗口时间
    [SerializeField] private int maxComboCount = 3;    // 最大连击数

    [Header("组件引用")]
    [SerializeField] private Animator animator;
    private PlayerInput playerInput;
    private InputAction attackAction;

    // 连击状态变量
    public int currentComboStep = 0;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;

    // 动画哈希值（提升性能）
    private int comboStepHash;
    private int attackTriggerHash;
    private int attackSpeedHash;

    PlayerReadInput_MoveAndJump moveAndJump;

    void Start()
    {
        // 初始化组件
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();

        // 获取输入动作
        attackAction = playerInput.actions["Attack"];

        // 设置动画参数哈希
        comboStepHash = Animator.StringToHash("ComboStep");
        attackTriggerHash = Animator.StringToHash("AttackTrigger");
        attackSpeedHash = Animator.StringToHash("AttackSpeed");

        //设置连击窗口时间
        comboWindow = 0.7f;

        //
        moveAndJump = GetComponent<PlayerReadInput_MoveAndJump>();
    }

    void Update()
    {
        // 1. 检测连击超时
        CheckComboTimeout();

        // 2. 检测攻击输入
        if (attackAction.triggered && moveAndJump._isGrounded)
        {
            HandleAttackInput();
        }

        // 3. 更新攻击状态
        UpdateAttackState();
    }

    /// <summary>
    /// 处理攻击输入
    /// </summary>
    private void HandleAttackInput()
    {
        // 如果当前没有在攻击，开始第一段攻击
        if (!isAttacking)
        {
            StartNewCombo();
        }
        // 如果正在攻击且在连击窗口内，进入下一段
        else if (Time.time - lastAttackTime <= comboWindow)
        {
            AdvanceCombo();
        }
        // 连击超时，重新开始
        else
        {
            ResetCombo();
            StartNewCombo();
        }

        lastAttackTime = Time.time;
    }

    /// <summary>
    /// 开始新的连击
    /// </summary>
    private void StartNewCombo()
    {
        currentComboStep = 1;
        isAttacking = true;

        // 触发动画
        animator.SetInteger(comboStepHash, currentComboStep);
        animator.SetTrigger(attackTriggerHash);

        Debug.Log($"连击开始: 第{currentComboStep}段");
    }

    /// <summary>
    /// 推进到下一段连击
    /// </summary>
    private void AdvanceCombo()
    {
        if (currentComboStep < maxComboCount)
        {
            currentComboStep++;
            animator.SetInteger(comboStepHash, currentComboStep);
            animator.SetTrigger(attackTriggerHash);

            Debug.Log($"连击继续: 第{currentComboStep}段");
        }
        else
        {
            Debug.Log("已达到最大连击段数");
        }
    }

    /// <summary>
    /// 检查连击是否超时
    /// </summary>
    private void CheckComboTimeout()
    {
        if (isAttacking && Time.time - lastAttackTime > comboWindow)
        {
            ResetCombo();
        }
    }

    /// <summary>
    /// 重置连击状态
    /// </summary>
    private void ResetCombo()
    {
        if (isAttacking)
        {
            currentComboStep = 0;
            isAttacking = false;
            animator.SetInteger(comboStepHash, currentComboStep);

            Debug.Log("连击超时，已重置");
        }
    }

    /// <summary>
    /// 更新攻击状态（通过动画事件调用）
    /// </summary>
    private void UpdateAttackState()
    {
        // 检查动画是否播放完毕
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isInAttackState = stateInfo.IsName("PlayerAttack1") ||
                               stateInfo.IsName("PlayerAttack2") ||
                               stateInfo.IsName("PlayerAttack3");

        // 如果不在攻击动画中，但标记为攻击状态，则重置
        if (!isInAttackState && isAttacking)
        {
            // 这里可以添加额外的判断，比如等待一小段时间
            // 防止动画过渡期间的误判
        }
    }

    /// <summary>
    /// 动画事件：攻击动画开始
    /// </summary>
    public void OnAttackStart(int comboStep)
    {
        isAttacking = true;
        Debug.Log($"攻击{comboStep}开始，可以在此处添加攻击判定");

        // 示例：根据连击段数调整攻击力
        // float damage = 10f * comboStep;
        // ApplyDamage(damage);
    }

    /// <summary>
    /// 动画事件：攻击动画结束
    /// </summary>
    public void OnAttackEnd(int comboStep)
    {
        Debug.Log($"攻击{comboStep}结束");

        // 如果是最后一段攻击，准备重置
        if (comboStep == maxComboCount)
        {
            // 可以设置一个标记，延迟重置
            StartCoroutine(DelayedComboReset());
        }
    }

    /// <summary>
    /// 延迟重置连击（协程）
    /// </summary>
    private System.Collections.IEnumerator DelayedComboReset()
    {
        yield return new WaitForSeconds(0.2f);

        // 检查是否没有新的连击输入
        if (Time.time - lastAttackTime > comboWindow)
        {
            ResetCombo();
        }
    }

    /// <summary>
    /// 外部调用：强制取消攻击
    /// </summary>
    public void CancelAttack()
    {
        ResetCombo();
        // 可以在这里添加取消攻击的动画过渡
    }
}
