using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(menuName ="Nagopia/Config")]
public class GameConfig : SerializedScriptableObject
{
    [Min(0)]
    public int MaximumStage = 10;

    [BoxGroup("Equipment")]
    [Tooltip("装备词条出现的概率")]
    public readonly double[] Equipment_RPair_Probability = new double[] { 1.0, 0.7, 0.5, 0.3, 0.2, 0.1, 0.05, 0.025, 0.004, 0.015, 0.0005 };

    /// <summary>
    /// 表示执行一次行动所需要的ATB的量
    /// </summary>
    [PropertyRange(min: 0, max: 10000)]
    [BoxGroup("Battle")]
    public readonly float MovedRequireATB = 1000;

    public float GameSpeed = 1.0f;

    public int CurrentStage;

    [BoxGroup("Battle")]
    /// <summary>
    /// 表示战斗系统中，1s中更新数值的次数
    /// </summary>
    public readonly int BattleSysUpdateTimesPerSec = 100;

    [BoxGroup("Battle")]
    /// <summary>
    /// 表示战斗系统中，1s中ATB数值上涨的基础量(不考虑速度的情况下)
    /// </summary>
    [MinValue(0)]
    public float ATBUpPerSec = 150;

    [BoxGroup("Character")]
    public readonly int MinLevel=1;

    [BoxGroup("Character")]
    public readonly int MaxLevel=100;

    [BoxGroup("Character")]
    public readonly int MinPosition=1;

    [BoxGroup("Character")]
    public readonly int MaxPosition = 999;

    [BoxGroup("Character")]
    public readonly byte MaxMental = 20;

    [BoxGroup("Character")]
    public readonly byte MinMental = 1;

    [MaxValue(1.0)]
    public readonly float MentalBuffFloor = 0.5f;

    [MinValue(1.0)]
    public readonly float MentalBuffCeiling = 1.5f;

    [PropertyRange(1,10)]
    [BoxGroup("Teaminfo")]
    [LabelText("队伍人数上限")]
    public readonly byte MaxTeamMember = 5;

    [MinValue(1)]
    [LabelText("全队可携带最大装备")]
    [BoxGroup("Teaminfo")]
    public readonly int CarriedMaximumEquipment = 30;

    [PropertyRange(0,1)]
    [LabelText("挡刀事件发生的基础概率")]
    [BoxGroup("Probability")]
    [BoxGroup("Probability/Substitude")]
    public readonly double SubstitudeEventBaseProbability = 0.15;

    [PropertyRange(0, 1)]
    [Tooltip("在自身承伤会死的情况，愿意去挡刀的概率")]
    [LabelText("牺牲生命挡刀概率")]
    [BoxGroup("Probability")]
    [BoxGroup("Probability/Substitude")]
    public readonly double SubstituteOnDeadProbability = 0.1;

    [PropertyRange(0, 1)]
    [LabelText("逃跑事件发生的基础概率")]
    [BoxGroup("Probability")]
    [BoxGroup("Probability/Escape")]
    public readonly double EscapeEventBaseProbability = 0.2;

    [PropertyRange(0,1)]
    [LabelText("慌张状态下逃跑事件发生的基础概率")]
    [Tooltip("仅限于自己不是攻击对象且下一攻击不会致死自己的情况下")]
    [BoxGroup("Probability")]
    [BoxGroup("Probability/Escape")]
    public readonly double EscapeUnderPressureProbability = 0.002;

    [PropertyRange(0,1)]
    [Tooltip("这个参数表示，自身在成为受击对象时，自己这轮行动会身死但能击败对方全体时，自己逃跑的参数")]
    [LabelText("受击死亡逃跑概率")]
    [BoxGroup("Probability")]
    [BoxGroup("Probability/Escape")]
    public readonly double EscapeDieAndDefeatProbability = 0.1;

    [PropertyRange(0, 1)]
    [LabelText("角色死亡概率")]
    [BoxGroup("Probability")]
    [BoxGroup("Probability/Death")]
    public readonly float CharacterDiedProbability = 0.15f;

    [PropertyRange(0, 1)]
    [LabelText("出现新队友的概率")]
    [BoxGroup("Probability")]
    [BoxGroup("Probability/Special")]
    public readonly double NewTeammateProbability = 0.2;

    [PropertyRange(0, 1)]
    [LabelText("团队回血的概率")]
    [BoxGroup("Probability")]
    [BoxGroup("Probability/Special")]
    public readonly double RestoreProbability = 0.3;

    [Tooltip("角色血量低于百分之多少时，处于危机状态")]
    public readonly float SingleCharaLowHPParam = 0.2f;

    [Tooltip("队伍血量低于百分之多少时，处于危机状态")]
    public readonly float PlayerTeamLowHPParam = 0.4f;

    [BoxGroup("Relation")]
    [MaxValue(0)]
    public readonly sbyte MinRelation = -100;

    [BoxGroup("Relation")]
    [MinValue(0)]
    public readonly sbyte MaxRelation = 100;

    
}
