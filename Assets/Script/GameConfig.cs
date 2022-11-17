using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(menuName ="Nagopia/Config")]
public class GameConfig : SerializedScriptableObject
{
    public readonly double[] Equipment_RPair_Probability = new double[] { 1.0, 0.7, 0.5, 0.3, 0.2, 0.1, 0.05, 0.025, 0.004, 0.015, 0.0005 };

    /// <summary>
    /// ��ʾִ��һ���ж�����Ҫ��ATB����
    /// </summary>
    [PropertyRange(min: 0, max: 10000)]
    public readonly float MovedRequireATB = 1000;

    public float GameSpeed = 1.0f;

    /// <summary>
    /// ��ʾս��ϵͳ�У�1s�и�����ֵ�Ĵ���
    /// </summary>
    public int BattleSysUpdateTimesPerSec = 100;

    /// <summary>
    /// ��ʾս��ϵͳ�У�1s��ATB��ֵ���ǵĻ�����(�������ٶȵ������)
    /// </summary>
    [MinValue(0)]
    public float ATBUpPerSec = 150;

    public int CurrentStage;

    public readonly int MinLevel=1;

    public readonly int MaxLevel=100;

    public readonly int MinPosition=1;

    public readonly int MaxPosition = 999;

    public readonly byte MaxMental = 20;

    public readonly byte MinMental = 1;

    [MaxValue(1.0)]
    public readonly float MentalBuffFloor = 0.5f;

    [MinValue(1.0)]
    public readonly float MentalBuffCeiling = 1.5f;

    [PropertyRange(1,10)]
    public readonly byte MaxTeamMember = 5;

    [PropertyRange(0,1)]
    public readonly double EscapeEventBaseProbability = 0.2;

}
