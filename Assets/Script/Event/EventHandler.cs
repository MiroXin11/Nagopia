using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MEC;
using System.Media;
using UnityEngine.UI;
using Sirenix.OdinInspector;

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
        }

        public void Start() {
            this.uiCanvas = InGameManager.Instance.UiCanvas;
            battleManager = SingletonMonobehaviour<BattleManager>.Instance;
            this.battleUIRoot = this.uiCanvas.transform.Find("BattleUIGroup").gameObject;
            if(ReferenceEquals(this.confirmMessageBox, null)) {
                confirmMessageBox=GameObject.Find("ConfirmMessageRoot").GetComponent<ConfirmMessageBox>();
            }
            //this.uiManager = SingletonMonobehaviour<InGameUIManager>.Instance;
        }

        public void StartBattle(BattleStartEvent eventData,System.Action startFinishedCallback=null,System.Action battleEndCallback=null) {
            Timing.RunCoroutine(StartBattleCoroutine(eventData, startFinishedCallback, battleEndCallback));
        }

        private IEnumerator<float>StartBattleCoroutine(BattleStartEvent eventData,System.Action startFinishedCallback=null,System.Action battleEndCallback = null) {
            confirmMessageBox.ShowUp();
            bool flag = false;
            bool flag2 = false;
            confirmMessageBox.AddConfirmCallback(() => { flag = true; flag2 = true; });
            confirmMessageBox.AddCancelCallback(() => { flag = true; });
            confirmMessageBox.ConfirmText = "开始战斗";
            confirmMessageBox.CancelText= "Cancel";
            confirmMessageBox.DescriptionText = "战斗遭遇";

            yield return Timing.WaitUntilTrue(()=>flag);
            confirmMessageBox.Hide();
            if (!flag2) {
                foreach (var item in eventData.EnemyTeam) {
                    DestroyImmediate(item.avatar);
                }
                battleEndCallback?.Invoke();
                yield break;
            }
            battleUIRoot.SetActive(true);
            yield return Timing.WaitForOneFrame;
            yield return Timing.WaitForOneFrame;
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
            data.attacker.animatorController.SetRenderOrder(4);
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
                //Debug.Log($"Part3:{ReferenceEquals(completeCallback,null)}");
                battleManager.ResetCharaPosition(data.attacker);
                completeCallback?.Invoke();
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
            //battleUIRoot.SetActive(false);
            data.attacker.animatorController.SetRenderOrder(0);
            completeCallback?.Invoke();
            //outputEventInfo(data);
            yield break;
        }

        public void CharacterEscapeEventHandle(ref EscapeEvent escape,System.Action completeCallback=null) {
            //battleManager.CharacterEscape(escape.character,completeCallback);
            Timing.RunCoroutine(CharacterEscapeCoroutine(escape,completeCallback));
        }

        private IEnumerator<float> CharacterEscapeCoroutine(EscapeEvent eventData,System.Action completeCallback=null) {
            var character = eventData.character;
            bool flag = false;
            battleManager.CharacterEscape(character, () => { flag = true; });
            yield return Timing.WaitUntilTrue(() => flag);
            Debug.Log($"Part2{ReferenceEquals(completeCallback,null)}");
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

        public void RestoreHPEventHandle(RestoreHPEvent eventData,System.Action completeCallback=null) {
            var datas = TeamInfo.CharacterDatas;
            bool isRate = eventData.IsRate;
            float rate=eventData.Rate;
            foreach (var item in datas) {
                if (isRate) {
                    item.CurrentHP += (int)(item.CurrentHP * rate);
                }
                else {
                    item.CurrentHP += (int)rate;
                }
            }
            outputEventInfo(eventData);
            completeCallback?.Invoke();
        }

        public void BattleWinEventHandler(BattleWinEvent eventData,System.Action completeCallback = null) {
            battleUIRoot.SetActive(false);
            //outputEventInfo(eventData);
            var enemies = eventData.enemies;
            int expGained = 0;
            foreach (var item in enemies) {
                EnemyBattleCharacter enemy = item as EnemyBattleCharacter;
                var data = enemy.data;
                var rank = (int)data.rank+1;
                expGained += rank * (int)(data.expRate * item.MaxHP);
                Debug.Log($"exp:{expGained},rank={rank},reality={(int)(data.expRate*item.MaxHP)}");
            }
            ExpGainedEvent expGainedEvent = new ExpGainedEvent(expGained);
            ExpGainedEventHandler(expGainedEvent, completeCallback);
        }

        public void GameOverEventHandler(GameOverEvent eventData,System.Action completeCallback=null) {

        }

        public void ExpGainedEventHandler(ExpGainedEvent eventData,System.Action completeCallback = null) {
            var character = eventData.character;
            int exp = eventData.exp;
            outputEventInfo(eventData);
            if (ReferenceEquals(character, null)) {
                var allChara = TeamInfo.CharacterDatas;
                foreach (var item in allChara) {
                    if (item.CurrentHP > 0) {
                        item.AddExp(exp);
                    }
                }
                completeCallback?.Invoke();
                return;
            }
            else {
                var allChara = TeamInfo.CharacterDatas;
                foreach (var item in allChara) {
                    if (ReferenceEquals(item, character) && item.CurrentHP > 0) {
                        item.AddExp(exp);
                        break;
                    }
                }
                completeCallback?.Invoke();
            }
        }

        public void MeetNewTeammateHandler(GenerateNewTeammateEvent eventData,System.Action completeCallback=null) {
            //confirmMessageBox.gameObject.SetActive(true);
            confirmMessageBox.ShowUp();
            var character = eventData.data;
            NewTeammateJoinEvent joinEvent = new NewTeammateJoinEvent(character);
            RefuseNewTeammate refuseEvent=new RefuseNewTeammate(character);
            confirmMessageBox.AddConfirmCallback(() => NewTeammateJoinHandler(joinEvent,completeCallback));
            confirmMessageBox.AddCancelCallback(() => RefuseNewTeammateJoinHandler(refuseEvent, completeCallback));
            confirmMessageBox.ConfirmText = "确定";
            confirmMessageBox.CancelText = "拒绝";
            confirmMessageBox.DescriptionText = $"确定要{character.name}加入吗?";
            //outputEventInfo(eventData);
        }

        public void NewTeammateJoinHandler(NewTeammateJoinEvent eventData,System.Action completeCallback=null) {
            TeamInfo.AddCharacter(eventData.character);
            confirmMessageBox.Hide();
            outputEventInfo(eventData);
            completeCallback?.Invoke();
        }

        public void RefuseNewTeammateJoinHandler(RefuseNewTeammate eventData,System.Action completeCallback=null) {
            confirmMessageBox.Hide();
            outputEventInfo(eventData);
            completeCallback?.Invoke();
        }

        public void NothingEventHandler(ref NothingHappenedEvent eventData,System.Action completeCallback=null) {
            Timing.RunCoroutine(NothingButWaitCoroutine(eventData,completeCallback));
        }

        private IEnumerator<float> NothingButWaitCoroutine(NothingHappenedEvent eventData,System.Action completeCallback=null) {
            outputEventInfo(eventData);
            yield return Timing.WaitForSeconds(eventData.waitTime);
            completeCallback?.Invoke();
        }

        public void CharacterLevelUpHandler(CharacterLevelUpEvent eventData) {
            outputEventInfo(eventData);
        }

        public void outputEventInfo(BaseEvent baseEvent) {
            uiManager.infoShower.AddNewInfo(baseEvent.ToString());
        }

        private CoroutineHandle attackCoroutine;

        private BattleManager battleManager;

        private Canvas uiCanvas;

        private GameObject battleUIRoot;

        [SerializeField]
        private ConfirmMessageBox confirmMessageBox;

        [SerializeField]
        private InGameUIManager uiManager;
    }
}