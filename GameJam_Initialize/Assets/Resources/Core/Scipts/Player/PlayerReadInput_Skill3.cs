using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerReadInput_Skill3 : MonoBehaviour
{
    [Header("时间限制配置")]
    [SerializeField] private float maxDefenseDuration = 3.0f;
    [SerializeField] private bool enableTimeLimit = true;

    [Header("耐久度限制配置")]
    [SerializeField] private int maxDurability = 5;
    [SerializeField] private bool enableDurabilityLimit = true;
    [SerializeField] private int currentDurability = 5;

    [Header("护盾视觉设置")]
    [SerializeField] private string shieldObjectName = "shield";
    [SerializeField] private float minShieldAlpha = 0.3f;
    [SerializeField] private float maxShieldAlpha = 0.8f;

    [Header("组件引用")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerInput playerInput;

    // 内部组件
    private InputAction defenseAction;
    private GameObject shieldObject;
    private SpriteRenderer shieldSprite;
    private Color originalShieldColor;

    PlayerReadInput_Skill2 skill2;
    PlayerReadInput_MoveAndJump moveAndJump;

    // 防御状态
    public enum DefenseState
    {
        Idle,
        StartDefense,
        Defending,
        EndDefense,
        Broken
    }
    public DefenseState currentState = DefenseState.Idle;

    // 计时相关
    private float defenseStartTime = 0f;
    private float currentDefenseDuration = 0f;

    // 输入状态
    private bool defenseInputPressed = false;

    // 动画哈希值
    private int isDefendingHash;
    private int defenseReadyHash;
    private int startDefenseTriggerHash;
    private int endDefenseTriggerHash;
    private int shieldBrokenHash;

    // 事件系统
    public event Action OnDefenseStart;
    public event Action OnDefenseEnd;
    public event Action OnDefenseBreak;
    public event Action<int> OnDurabilityChanged;
    public event Action OnDurabilityDepleted;

    //检测变量用于检查是否处于防御状态
    bool isDefendingCD = false;
    float defendingCD = 5f;

    void Start()
    {
        InitializeComponents();
        FindShieldObject();
        InitializeShieldAppearance();

        skill2 = GetComponent<PlayerReadInput_Skill2>();
        moveAndJump = GetComponent<PlayerReadInput_MoveAndJump>();
    }

    void Update()
    {
        defenseInputPressed = defenseAction.ReadValue<float>() > 0.5f;

        UpdateDefenseTimer();
        UpdateDefenseState();
        UpdateShieldTransparency();
    }

    /// <summary>
    /// 初始化组件
    /// </summary>
    private void InitializeComponents()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();

        defenseAction = playerInput.actions["Defense"];

        isDefendingHash = Animator.StringToHash("IsDefending");
        defenseReadyHash = Animator.StringToHash("DefenseReady");
        startDefenseTriggerHash = Animator.StringToHash("StartDefenseTrigger");
        endDefenseTriggerHash = Animator.StringToHash("EndDefenseTrigger");
        shieldBrokenHash = Animator.StringToHash("ShieldBroken");
    }

    /// <summary>
    /// 查找护盾对象并获取SpriteRenderer
    /// </summary>
    private void FindShieldObject()
    {
        Transform shieldTransform = transform.Find(shieldObjectName);

        if (shieldTransform == null)
        {
            shieldTransform = FindDeepChild(transform, shieldObjectName);
        }

        if (shieldTransform != null)
        {
            shieldObject = shieldTransform.gameObject;
            shieldSprite = shieldObject.GetComponent<SpriteRenderer>();

            if (shieldSprite != null)
            {
                originalShieldColor = shieldSprite.color;
                Debug.Log($"找到护盾SpriteRenderer: {shieldObject.name}");
            }
            else
            {
                Debug.LogWarning($"护盾对象 '{shieldObjectName}' 没有SpriteRenderer组件");
            }

            if (shieldObject.activeSelf)
                shieldObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"未找到名为 '{shieldObjectName}' 的护盾对象");
        }
    }

    /// <summary>
    /// 深度搜索子物体
    /// </summary>
    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    /// <summary>
    /// 初始化护盾外观
    /// </summary>
    private void InitializeShieldAppearance()
    {
        if (shieldSprite != null)
        {
            originalShieldColor = shieldSprite.color;

            if (currentState == DefenseState.Idle)
            {
                Color color = shieldSprite.color;
                color.a = 0f;
                shieldSprite.color = color;
            }
        }
    }

    /// <summary>
    /// 更新防御计时器
    /// </summary>
    private void UpdateDefenseTimer()
    {
        if (currentState == DefenseState.Defending)
        {
            currentDefenseDuration = Time.time - defenseStartTime;

            if (enableTimeLimit && currentDefenseDuration >= maxDefenseDuration)
            {
                Debug.Log($"防御时间耗尽 ({maxDefenseDuration}秒)");
                EndDefense(EndReason.TimeLimit);
            }
        }
        else
        {
            currentDefenseDuration = 0f;
        }
    }

    /// <summary>
    /// 更新防御状态机
    /// </summary>
    private void UpdateDefenseState()
    {
        switch (currentState)
        {
            case DefenseState.Idle:
                if (defenseAction.triggered && skill2.currentState ==PlayerReadInput_Skill2.ChargeState.Idle && moveAndJump._isGrounded)
                    StartDefense();
                break;

            case DefenseState.StartDefense:
                if (!defenseInputPressed)
                    CancelDefense();
                break;

            case DefenseState.Defending:
                if (enableDurabilityLimit && currentDurability <= 0)
                {
                    BreakShield();
                    return;
                }

                if (!defenseInputPressed)
                    EndDefense(EndReason.ManualRelease);
                break;

            case DefenseState.Broken:
                if (Time.time - defenseStartTime > 2f)
                    currentState = DefenseState.Idle;
                break;
        }
    }

    /// <summary>
    /// 开始防御
    /// </summary>
    private void StartDefense()
    {
        if (currentState == DefenseState.Broken)
        {
            Debug.Log("护盾已损坏，无法防御");
            return;
        }

        currentState = DefenseState.StartDefense;
        animator.SetBool(isDefendingHash, true);
        animator.SetTrigger(startDefenseTriggerHash);

        if (shieldObject != null)
        {
            shieldObject.SetActive(true);

            if (shieldSprite != null)
            {
                shieldSprite.color = originalShieldColor;
            }
        }

        OnDefenseStart?.Invoke();
        Debug.Log("开始防御动作");
    }

    /// <summary>
    /// 动画事件：防御就绪
    /// </summary>
    public void OnDefenseReady()
    {
        if (currentState == DefenseState.StartDefense && defenseInputPressed)
        {
            EnterDefendingState();
        }
        else
        {
            EndDefense(EndReason.Cancelled);
        }
    }

    /// <summary>
    /// 进入持续防御状态
    /// </summary>
    private void EnterDefendingState()
    {
        currentState = DefenseState.Defending;
        defenseStartTime = Time.time;
        animator.SetBool(defenseReadyHash, true);

        Debug.Log($"进入持续防御状态 (耐久:{currentDurability}/{maxDurability})");
    }

    /// <summary>
    /// 结束防御的原因
    /// </summary>
    private enum EndReason { ManualRelease, TimeLimit, DurabilityDepleted, Cancelled }

    /// <summary>
    /// 结束防御
    /// </summary>
    private void EndDefense(EndReason reason)
    {
        if (currentState == DefenseState.EndDefense || currentState == DefenseState.Idle)
            return;

        currentState = DefenseState.EndDefense;
        
        animator.SetBool(defenseReadyHash, false);
        animator.SetTrigger(endDefenseTriggerHash);

        if (shieldObject != null && reason != EndReason.DurabilityDepleted)
        {
            if (shieldSprite != null)
            {
                Color color = shieldSprite.color;
                color.a = 0f;
                shieldSprite.color = color;
            }
            shieldObject.SetActive(false);
        }

        Debug.Log($"防御结束 - 原因: {reason}");
        OnDefenseEnd?.Invoke();
    }

    /// <summary>
    /// 护盾破碎
    /// </summary>
    private void BreakShield()
    {
        currentState = DefenseState.Broken;
        defenseStartTime = Time.time;

        animator.SetBool(isDefendingHash, false);
        animator.SetBool(defenseReadyHash, false);
        animator.SetTrigger(endDefenseTriggerHash);
        animator.SetTrigger(shieldBrokenHash);

        if (shieldObject != null && shieldSprite != null)
        {
            StartCoroutine(PlayShieldBreakEffect());
        }

        Debug.Log("护盾耐久耗尽，已破碎!");
        OnDefenseBreak?.Invoke();
        OnDurabilityDepleted?.Invoke();
    }

    /// <summary>
    /// 播放护盾破碎特效
    /// </summary>
    private System.Collections.IEnumerator PlayShieldBreakEffect()
    {
        if (shieldSprite != null)
        {
            shieldSprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < 3; i++)
            {
                shieldSprite.enabled = false;
                yield return new WaitForSeconds(0.1f);
                shieldSprite.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }

            shieldObject.SetActive(false);
        }
    }

    /// <summary>
    /// 取消防御
    /// </summary>
    private void CancelDefense()
    {
        currentState = DefenseState.Idle;
        animator.SetBool(isDefendingHash, false);
        animator.SetBool(defenseReadyHash, false);
        animator.SetTrigger(endDefenseTriggerHash);

        if (shieldObject != null)
        {
            if (shieldSprite != null)
            {
                Color color = shieldSprite.color;
                color.a = 0f;
                shieldSprite.color = color;
            }
            shieldObject.SetActive(false);
        }

        Debug.Log("防御动作被取消");
    }

    /// <summary>
    /// 动画事件：防御完全结束
    /// </summary>
    public void OnDefenseEndComplete()
    {
        if (currentState != DefenseState.Broken)
        {
            currentState = DefenseState.Idle;
            animator.SetBool(isDefendingHash, false);
        }
    }

    /// <summary>
    /// 更新护盾透明度（只在持续防御状态下生效）
    /// </summary>
    private void UpdateShieldTransparency()
    {
        if (currentState == DefenseState.Defending && shieldSprite != null && enableDurabilityLimit)
        {
            float durabilityPercent = (float)currentDurability / maxDurability;
            float targetAlpha = Mathf.Lerp(minShieldAlpha, maxShieldAlpha, durabilityPercent);

            Color currentColor = shieldSprite.color;
            Color targetColor = originalShieldColor;
            targetColor.a = targetAlpha;

            shieldSprite.color = Color.Lerp(currentColor, targetColor, Time.deltaTime * 5f);

            if (durabilityPercent < 0.3f)
            {
                float redIntensity = 1.0f - (durabilityPercent * 3f);
                targetColor.r = Mathf.Lerp(originalShieldColor.r, 1.0f, redIntensity);
                targetColor.g = Mathf.Lerp(originalShieldColor.g, 0.3f, redIntensity);
                targetColor.b = Mathf.Lerp(originalShieldColor.b, 0.3f, redIntensity);
                shieldSprite.color = targetColor;
            }
        }
    }

    // ========== 公共接口方法 ==========

    /// <summary>
    /// 消耗耐久度
    /// </summary>
    public bool ConsumeDurability(int amount = 1)
    {
        if (!enableDurabilityLimit || currentState != DefenseState.Defending)
            return false;

        currentDurability = Mathf.Max(0, currentDurability - amount);

        Debug.Log($"耐久度消耗: {amount}, 剩余: {currentDurability}");
        OnDurabilityChanged?.Invoke(currentDurability);

        return true;
    }

    /// <summary>
    /// 恢复耐久度
    /// </summary>
    public void RestoreDurability(int amount)
    {
        if (!enableDurabilityLimit)
            return;

        int oldValue = currentDurability;
        currentDurability = Mathf.Min(maxDurability, currentDurability + amount);

        if (oldValue <= 0 && currentDurability > 0 && currentState == DefenseState.Broken)
        {
            currentState = DefenseState.Idle;
            animator.ResetTrigger(shieldBrokenHash);
        }

        Debug.Log($"耐久度恢复: {amount}, 当前: {currentDurability}");
        OnDurabilityChanged?.Invoke(currentDurability);
    }

    /// <summary>
    /// 设置最大耐久度
    /// </summary>
    public void SetMaxDurability(int value)
    {
        if (value <= 0) return;

        float percentage = (float)currentDurability / maxDurability;
        maxDurability = value;
        currentDurability = Mathf.RoundToInt(maxDurability * percentage);

        Debug.Log($"最大耐久度设置为: {maxDurability}");
    }

    /// <summary>
    /// 设置当前耐久度
    /// </summary>
    public void SetCurrentDurability(int value)
    {
        currentDurability = Mathf.Clamp(value, 0, maxDurability);
        OnDurabilityChanged?.Invoke(currentDurability);
    }

    /// <summary>
    /// 获取当前耐久度
    /// </summary>
    public int GetCurrentDurability()
    {
        return currentDurability;
    }

    /// <summary>
    /// 获取最大耐久度
    /// </summary>
    public int GetMaxDurability()
    {
        return maxDurability;
    }

    /// <summary>
    /// 获取耐久度百分比
    /// </summary>
    public float GetDurabilityPercentage()
    {
        return (float)currentDurability / maxDurability;
    }

    /// <summary>
    /// 设置最大防御时间
    /// </summary>
    public void SetMaxDefenseDuration(float duration)
    {
        maxDefenseDuration = Mathf.Max(0.1f, duration);
    }

    /// <summary>
    /// 启用/禁用时间限制
    /// </summary>
    public void SetTimeLimitEnabled(bool enabled)
    {
        enableTimeLimit = enabled;
    }

    /// <summary>
    /// 启用/禁用耐久度限制
    /// </summary>
    public void SetDurabilityLimitEnabled(bool enabled)
    {
        enableDurabilityLimit = enabled;
    }

    /// <summary>
    /// 检查是否正在防御
    /// </summary>
    public bool IsDefending()
    {
        return currentState == DefenseState.Defending;
    }

    /// <summary>
    /// 检查护盾是否已破碎
    /// </summary>
    public bool IsShieldBroken()
    {
        return currentState == DefenseState.Broken;
    }

    /// <summary>
    /// 获取当前防御状态
    /// </summary>
    public DefenseState GetCurrentDefenseState()
    {
        return currentState;
    }

    /// <summary>
    /// 获取剩余防御时间
    /// </summary>
    public float GetRemainingTime()
    {
        if (currentState == DefenseState.Defending && enableTimeLimit)
            return Mathf.Max(0, maxDefenseDuration - currentDefenseDuration);
        return 0;
    }

    /// <summary>
    /// 获取护盾SpriteRenderer
    /// </summary>
    public SpriteRenderer GetShieldSpriteRenderer()
    {
        return shieldSprite;
    }

    /// <summary>
    /// 手动设置护盾颜色
    /// </summary>
    public void SetShieldColor(Color color)
    {
        if (shieldSprite != null)
        {
            originalShieldColor = color;
            shieldSprite.color = color;
        }
    }

    /// <summary>
    /// 强制中断防御（外部调用）
    /// </summary>
    public void ForceBreakDefense()
    {
        if (currentState == DefenseState.Defending || currentState == DefenseState.StartDefense)
        {
            BreakShield();
        }
    }
}
