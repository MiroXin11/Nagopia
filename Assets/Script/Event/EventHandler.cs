using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia
{
    public class EventHandler:SingletonMonobehaviour<EventHandler>
    {
        public void Awake()
        {
            if (SingletonMonobehaviour<EventHandler>.Instance!=this)
            {
                DestroyImmediate(this);
            }
            DontDestroyOnLoad(this);
        }

        /// <summary>
        /// 处理攻击事件，并检查攻击事件是否触发其它特殊事件
        /// </summary>
        /// <param name="data"></param>
        public void AttackEvent(AttackEventData data)
        {

        }

        private BattleManager battleManager = SingletonMonobehaviour<BattleManager>.Instance;
    }
}