using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerReadInput : MonoBehaviour
{
    PlayerInput input;
    InputAction run;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInput>();
        InputActionAsset asset = Resources.Load<InputActionAsset>("Core/config/PlayerInputMap");
        if (asset == null)
        {
            Debug.LogError("InputActionAsset 加载失败！请检查路径和资源名称。");
            return;
        }
        else
        {
            Debug.Log("InputActionAsset 加载成功！");
        }

        input.actions = asset;
        input.actions.Enable();

        // 检查是否存在Move动作
        if (input.actions["Move"] == null)
        {
            Debug.LogError("未找到名为 'Move' 的输入动作！");
            return;
        }

        run = input.actions["Move"];
        run.performed += OnMovePerformed;
        run.Enable();

        Debug.Log("Move 动作已注册和启用。");
    }

    void OnMovePerformed(InputAction.CallbackContext context)
    {
        Debug.Log("OnMovePerformed 被调用！");
        print(context.ReadValue<Vector2>());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
