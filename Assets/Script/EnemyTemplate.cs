using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace Nagopia {
    public class EnemyTemplate : SerializedScriptableObject {

        [Tooltip("敌人的分级")]
        public readonly GameDataBase.EnemyRarity rank;

        [Tooltip("角色的定位，影响攻击逻辑")]
        public readonly GameDataBase.EnemyDuty duty;

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
    }
}
