using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    [CreateAssetMenu(fileName ="teamTemplate",menuName ="Nagopia/Enemy Team")]
    public class EnemyTeamTemplate : SerializedScriptableObject {
        public readonly List<EnemyTemplate>team=new List<EnemyTemplate>();

        /// <summary>
        /// 可出现在的游戏阶段
        /// </summary>
        public readonly int[] apperanceStage;

        public readonly GameDataBase.EnemyTeamDescribtion describtion;

        public bool CanAppear(int item) {
            int length=apperanceStage.Length;
            for(int i = 0; i < length; ++i) {
                if (item == apperanceStage[i]) {
                    return true;
                }
            }
            return false;
        }
    }
}