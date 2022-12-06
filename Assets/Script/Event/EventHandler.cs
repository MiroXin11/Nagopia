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
            //if (SingletonMonobehaviour<EventHandler>.Instance!=null&&SingletonMonobehaviour)
            //{
            //    DestroyImmediate(this);
            //    return;
            //}
            DontDestroyOnLoad(this);
            battleManager=SingletonMonobehaviour<BattleManager>.Instance;
        }

        public void StartBattle(BattleStartEvent eventData,System.Action startFinishedCallback=null,System.Action battleEndCallback=null) {
            SingletonMonobehaviour<BattleATBIndicator>.Instance.gameObject.SetActive(true);
            battleManager.StartBattle(eventData.PlayerTeam, eventData.EnemyTeam, startFinishedCallback, battleEndCallback);
        }

        /// <summary>
        /// 处理攻击事件，并检查攻击事件是否触发其它特殊事件
        /// </summary>
        /// <param name="data"></param>
        public void AttackEventHandle(AttackEventData data,System.Action completeCallback=null){
            if(attackCoroutine!= null) {
                Timing.KillCoroutines(attackCoroutine);
            }
            attackCoroutine=Timing.RunCoroutine(AttackEventCoroutine(data,completeCallback));
        }

        /// <summary>
        /// 完整的攻击事件的回调，分步完成攻击事件以及里面的变种事件
        /// 目前变种事件：受击角色逃跑、受击方有人承担伤害
        /// </summary>
        /// <param name="data"></param>
        /// <param name="completeCallback"></param>
        /// <returns></returns>
        IEnumerator<float>AttackEventCoroutine(AttackEventData data,System.Action completeCallback=null) {
            var attackerAvatar = data.attacker.avatar;
            bool flag = false;
            var target = data.target;

            //角色移动到攻击目标处并检查受击目标有无逃跑事件
            battleManager.AttackCharaMove(data, () => flag = true);
            target.UnderAttack(data, out var escape);
            yield return Timing.WaitUntilTrue(() => flag);//等待移动动画结束后进入下一阶段
            yield return Timing.WaitForSeconds(0.5f);

            //检查逃跑事件
            if (!ReferenceEquals(escape, null)) {
                bool t = false;
                var handle = Timing.RunCoroutine(CharacterEscapeCoroutine(escape,()=>t=true));
                yield return Timing.WaitUntilTrue(() => t);
                completeCallback?.Invoke();
                battleManager.ResetCharaPosition(data.attacker);
                yield break;
            }

            //攻击，检查受击方有无队友愿意承担伤害
            var target_team = data.battleInfo.enemy_sortByPos;
            foreach (var item in target_team) {
                if (!ReferenceEquals(item, target)) {
                    if (item.TeammateUnderAttack(data)) {
                        SubstitudeAttackEvent substitude = new SubstitudeAttackEvent(data.attacker, item, target);
                        flag = false;
                        battleManager.SubstitudeAttack(substitude, ()=>flag=true);
                        yield return Timing.WaitUntilTrue(() => flag);
                        outputEventInfo(substitude);
                        completeCallback?.Invoke();
                        yield break;
                    }
                }
            }
            
            //无人愿意承担伤害，攻击事件正常发生
            flag = false;
            battleManager.CharacterAttack(data, () => flag = true);
            yield return Timing.WaitUntilTrue(() => flag);
            completeCallback?.Invoke();
            outputEventInfo(data);
            yield break;
        }

        public void CharacterEscapeEventHandle(ref EscapeEvent escape,System.Action completeCallback=null) {
            battleManager.CharacterEscape(escape.character,completeCallback);
        }

        private IEnumerator<float> CharacterEscapeCoroutine(EscapeEvent eventData,System.Action completeCallback=null) {
            var character = eventData.character;
            bool flag = false;
            battleManager.CharacterEscape(character, () => { flag = true; });
            yield return Timing.WaitUntilTrue(() => flag);
            completeCallback?.Invoke(); 
            outputEventInfo(eventData);
            yield break;
        }

        public void CureEventHandle(ref CureEvent cure,System.Action completeCallback=null) {
            Timing.RunCoroutine(CureCoroutine(cure, completeCallback));
        }

        private IEnumerator<float>CureCoroutine(CureEvent eventData,System.Action completeCallback=null) {
            bool flag = false;
            battleManager.CharacterCure(ref eventData,()=>flag=true);
            yield return Timing.WaitUntilTrue(() => flag);
            completeCallback?.Invoke();
            outputEventInfo(eventData);
            yield break;
        }

        public void RestoreHPEventHandle(RestoreHPEvent restoreHP) {
            var datas = TeamInfo.CharacterDatas;
            bool isRate = restoreHP.IsRate;
            float rate=restoreHP.Rate;
            foreach (var item in datas) {
                if (isRate) {
                    item.CurrentHP += (int)(item.CurrentHP * rate);
                }
                else {
                    item.CurrentHP += (int)rate;
                }
            }
        }

        public void outputEventInfo(BaseEvent baseEvent) {
            Debug.Log(baseEvent.ToString());
        }

        private CoroutineHandle attackCoroutine;

        private BattleManager battleManager;
    }
}