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
        /// �������¼�������鹥���¼��Ƿ񴥷����������¼�
        /// </summary>
        /// <param name="data"></param>
        public void AttackEvent(AttackEventData data)
        {

        }

        private BattleManager battleManager = SingletonMonobehaviour<BattleManager>.Instance;
    }
}