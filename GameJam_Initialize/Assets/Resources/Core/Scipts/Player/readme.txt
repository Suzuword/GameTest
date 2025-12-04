PlayerReadInput_Attack(攻击方法):
代码格式:
1、开始攻击判定的代码
    public void OnAttackStart(int comboStep) //参数表示当前攻击为第comboStep个连段
    //该方法为第comboStep个连段,刀锋开始落下时(攻击判定帧起点)执行
    //位于177行代码处,已经写好switch_case(186行处)

2、结束攻击判定的代码
    public void OnAttackEnd(int comboStep)  //参数表示当前攻击为第comboStep个连段
     //该方法为第comboStep个连段,刀锋完全落下时(攻击判定帧结束点)执行
    //位于209行代码处,已经写好switch_case(214行处)


PlaerReadInput_Skill2(咖喱棒方法):
    代码格式:
1、开始攻击判定的代码
    public void OnSlashStart()//蓄力斩击攻击帧起点执行
    //位于211行代码处

2、结束攻击判定的代码
    public void OnSlashEnd()//蓄力斩击攻击帧结束点执行
    //位于224行代码处,

    //可以用一个剑气远程判定加一个近战判定