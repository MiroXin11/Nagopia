using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia
{
    public class EventHandler:SingletonMonobehaviour<EventHandler>
    {
        public void AttackEvent(AttackEventData data)
        {

        }

        private BattleManager battleManager = SingletonMonobehaviour<BattleManager>.Instance;
    }
}