using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MEC;
namespace Nagopia
{
    public class EventHandler:SingletonMonobehaviour<EventHandler>
    {
        public void Awake(){
            if (SingletonMonobehaviour<EventHandler>.Instance!=this)
            {
                DestroyImmediate(this);
                return;
            }
            DontDestroyOnLoad(this);
        }

        /// <summary>
        /// 处理攻击事件，并检查攻击事件是否触发其它特殊事件
        /// </summary>
        /// <param name="data"></param>
        public void AttackEvent(AttackEventData data){
            if(attackCoroutine!= null) {
                Timing.KillCoroutines(attackCoroutine);
            }
            attackCoroutine=Timing.RunCoroutine(AttackEventCoroutine(data));
        }

        IEnumerator<float>AttackEventCoroutine(AttackEventData data) {
            
            yield return 0;
        }

        public void CharacterEscape(ref EscapeEvent escape) {
            battleManager.CharacterEscape(escape.character);
        }

        private CoroutineHandle attackCoroutine;

        private BattleManager battleManager = SingletonMonobehaviour<BattleManager>.Instance;
    }
}