using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace Nagopia {
    public class EnemyTemplate : SerializedScriptableObject {

        [Tooltip("���˵ķּ�")]
        public readonly GameDataBase.EnemyRarity rank;

        [Tooltip("��ɫ�Ķ�λ��Ӱ�칥���߼�")]
        public readonly GameDataBase.EnemyDuty duty;

        /// <summary>
        /// Ѫ���ı���
        /// </summary>
        [MinValue(0)]
        [BoxGroup("HP")]
        [HorizontalGroup("HP/hori")]
        [Tooltip("Ѫ������������")]
        public readonly float HP_Rate;

        [MinValue(0)]
        [BoxGroup("HP")]
        [HorizontalGroup("HP/hori")]
        [Tooltip("Ѫ���Ļ���ֵ")]
        /// <summary>
        /// �������Ļ���ֵ��������׶ε�Ϊ��׼
        /// </summary>
        public readonly float BasisHP;

        /// <summary>
        /// �������ı���
        /// </summary>
        [MinValue(0)]
        [BoxGroup("ATK")]
        [HorizontalGroup("ATK/hori")]
        [Tooltip("����������������")]
        public readonly float ATK_Rate;

        /// <summary>
        /// �������Ļ���ֵ
        /// </summary>
        [MinValue(0)]
        [BoxGroup("ATK")]
        [HorizontalGroup("ATK/hori")]
        [Tooltip("�������Ļ���ֵ")]
        public readonly float BasisATK;

        /// <summary>
        /// �������ı���
        /// </summary>
        [MinValue(0)]
        [BoxGroup("DEF")]
        [HorizontalGroup("DEF/hori")]
        [Tooltip("�������ı���")]
        public readonly float DEF_Rate;

        /// <summary>
        /// �������Ļ���ֵ
        /// </summary>
        [MinValue(0)]
        [BoxGroup("DEF")]
        [HorizontalGroup("DEF/hori")]
        [Tooltip("�������Ļ���ֵ")]
        public readonly float BasisDEF;

        /// <summary>
        /// �ٶȵı���
        /// </summary>
        [MinValue(0)]
        [BoxGroup("SPE")]
        [HorizontalGroup("SPE/hori")]
        [Tooltip("�ٶȵ���������")]
        public readonly float SPE_Rate;

        /// <summary>
        /// �ٶȵĻ���ֵ
        /// </summary>
        [MinValue(0)]
        [BoxGroup("SPE")]
        [HorizontalGroup("SPE/hori")]
        [Tooltip("�ٶȵĻ���ֵ")]
        public readonly float BasisSPE;

        [MinValue(0)]
        [BoxGroup("Others")]
        public readonly int Position;
    }
}
