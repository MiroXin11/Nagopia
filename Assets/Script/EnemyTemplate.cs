using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Sirenix.Serialization;

namespace Nagopia {
    public class EnemyTemplate : SerializedScriptableObject {

        [Tooltip("敌人的分级")]
        [HorizontalGroup("a")]
        [LabelWidth(40)]
        public readonly GameDataBase.EnemyRarity rank;

        [Tooltip("角色的定位，影响攻击逻辑")]
        [HorizontalGroup("a")]
        [LabelWidth(40)]
        public readonly GameDataBase.EnemyDuty duty;

        [HorizontalGroup("a")]
        [Tooltip("角色攻击动画的类型")]
        [LabelWidth(80)]
        [LabelText("AttackType")]
        public readonly GameDataBase.AttackAnimationType attackAnimationType;

        [Tooltip("经验比例")]
        [HorizontalGroup("c")]
        public readonly float exp_rate;

        [HorizontalGroup("c")]
        public readonly string Name;

        [PreviewField(Alignment =ObjectFieldAlignment.Left)]
        [NonSerialized,OdinSerialize]
        [HorizontalGroup("b")]
        public GameObject prefab;

        [PreviewField(Alignment =ObjectFieldAlignment.Left)]
        [NonSerialized,OdinSerialize]
        [HorizontalGroup("b")]
        public Sprite HeadImage;

        /// <summary>
        /// 血量的倍率
        /// </summary>
        [MinValue(0)]
        [BoxGroup("HP")]
        [HorizontalGroup("HP/hori")]
        [Tooltip("血量的增长倍率")]
        public readonly float HP_Rate;

        [MinValue(0)]
        [BoxGroup("HP")]
        [HorizontalGroup("HP/hori")]
        [Tooltip("血量的基础值")]
        /// <summary>
        /// 攻击力的基础值，以最初阶段的为基准
        /// </summary>
        public readonly float BasisHP;

        /// <summary>
        /// 攻击力的倍率
        /// </summary>
        [MinValue(0)]
        [BoxGroup("ATK")]
        [HorizontalGroup("ATK/hori")]
        [Tooltip("攻击力的增长倍率")]
        public readonly float ATK_Rate;

        /// <summary>
        /// 攻击力的基础值
        /// </summary>
        [MinValue(0)]
        [BoxGroup("ATK")]
        [HorizontalGroup("ATK/hori")]
        [Tooltip("攻击力的基础值")]
        public readonly float BasisATK;

        /// <summary>
        /// 防御力的倍率
        /// </summary>
        [MinValue(0)]
        [BoxGroup("DEF")]
        [HorizontalGroup("DEF/hori")]
        [Tooltip("防御力的倍率")]
        public readonly float DEF_Rate;

        /// <summary>
        /// 防御力的基础值
        /// </summary>
        [MinValue(0)]
        [BoxGroup("DEF")]
        [HorizontalGroup("DEF/hori")]
        [Tooltip("防御力的基础值")]
        public readonly float BasisDEF;

        /// <summary>
        /// 速度的倍率
        /// </summary>
        [MinValue(0)]
        [BoxGroup("SPE")]
        [HorizontalGroup("SPE/hori")]
        [Tooltip("速度的增长倍率")]
        public readonly float SPE_Rate;

        /// <summary>
        /// 速度的基础值
        /// </summary>
        [MinValue(0)]
        [BoxGroup("SPE")]
        [HorizontalGroup("SPE/hori")]
        [Tooltip("速度的基础值")]
        public readonly float BasisSPE;

        [MinValue(0)]
        [BoxGroup("Others")]
        public readonly int Position;

        [ShowInInspector]
        [NonSerialized,OdinSerialize]
        [BoxGroup("Mental")]
        [Tooltip("精神属性的范围，如果未添加在该字典的属性，将按照GameConfig中设置的最大值与最小值范围来随机生成")]
        public Dictionary<GameDataBase.MentalType, MinMaxPair<byte>> MentalRange = new Dictionary<GameDataBase.MentalType, MinMaxPair<byte>>();
    }
}
