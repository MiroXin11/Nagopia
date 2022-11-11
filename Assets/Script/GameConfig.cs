using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(menuName ="Nagopia/Config")]
public class GameConfig : SerializedScriptableObject
{
    public readonly int MinLevel=1;

    public readonly int MaxLevel=100;

    public readonly int MinPosition=1;

    public readonly int MaxPosition = 999;

    public readonly byte MaxMental = 20;

    public readonly byte MinMental = 1;

    public readonly byte MaxTeamMember = 5;

    public int CurrentStage;

    public readonly double[] Equipment_RPair_Probability = new double[]{ 1.0, 0.7, 0.5, 0.3, 0.2, 0.1, 0.05, 0.025, 0.004, 0.015, 0.0005 };
}
