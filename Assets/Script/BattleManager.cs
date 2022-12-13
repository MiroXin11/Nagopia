using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System;
using System.Linq;
using DG.Tweening;
using UnityEngine.Events;

namespace Nagopia {
    public class BattleManager : SingletonMonobehaviour<BattleManager> {
        public void Awake() {
            this.PlayerCharaPosition.Clear();
            this.EnemyCharaPosition.Clear();
            int maxMember = GameDataBase.Config.MaxTeamMember;
            for (int i = 0; i < maxMember; ++i) {
                this.PlayerCharaPosition.Add(new Vector3(-1.2f - 1.5f * i, -2f, 2f));
                this.EnemyCharaPosition.Add(new Vector3(1.2f + 1.5f * i, -2f, 3f));
            }
        }

        public void StartBattle(List<IBattleCharacter> player, List<IBattleCharacter> enemy, System.Action startFinishedCallback = null, System.Action battleFinishedCallback = null) {
            if (handle.IsRunning) {
                Timing.KillCoroutines(handle);
            }
            UpdateBattleConfig();
            this.allUsableCharacter.Clear();
            this.JudgePlayerEnemy.Clear();

            this.playerTeam = player.Where((x) => x.HP > 0).ToList();
            this.enemyTeam = enemy;

            this.playerByPos = new List<IBattleCharacter>(playerTeam); playerByPos.Sort(SortByPos);
            this.playerBySpe = new List<IBattleCharacter>(playerTeam); playerBySpe.Sort(SortBySpeed);
            this.enemyByPos = new List<IBattleCharacter>(enemyTeam); playerByPos.Sort(SortByPos);
            this.enemyBySpe = new List<IBattleCharacter>(enemyTeam); enemyBySpe.Sort(SortBySpeed);

            int count = player.Count;
            for (int i = 0; i < count; ++i) {
                IBattleCharacter character = player[i];
                JudgePlayerEnemy.Add(character, true);
                if (character.HP > 0) {
                    allUsableCharacter.Add(character);
                }

                character = playerByPos[i];
                if (character.HP > 0) {
                    character.avatar.transform.localPosition = PlayerCharaPosition[i];
                }
                else {
                    character.avatar.SetActive(false);
                }
            }
            count = enemy.Count;
            for (int i = 0; i < count; ++i) {
                IBattleCharacter character = enemy[i];
                JudgePlayerEnemy.Add(character, false);
                if (character.HP > 0) {
                    allUsableCharacter.Add(character);
                }

                character = enemyByPos[i];
                if (character.HP > 0) {
                    character.avatar.transform.localPosition = EnemyCharaPosition[i];
                    var transform = character.avatar.transform;
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
                else {
                    character.avatar.SetActive(false);
                }
            }

            allUsableCharacter.Sort(SortBySpeed);
            foreach (var character in allUsableCharacter) {//给一个起步值，让战斗节奏更快开始
                character.ATB = character.SPE * 10 + 100;
                onATBUpdate.Invoke(character);
            }

            charaPosInScene.Clear();
            foreach (var item in allUsableCharacter) {
                charaPosInScene.Add(item, item.avatar.transform.position);
            }
            startFinishedCallback?.Invoke();
            onBattleStart.Invoke(GetBattleInfo(player[0]));
            handle = Timing.RunCoroutine(BattleCoroutine(finishedCallback:()=>battleFinishedCallback?.Invoke()));
        }

        /// <summary>
        /// 每次都更新一下，方便可以做出每次都做出更改?
        /// </summary>
        private void UpdateBattleConfig() {
            GameConfig config = GameDataBase.Config;
            this.eventHandler = SingletonMonobehaviour<EventHandler>.Instance;
            this.MovedRequiredATB = config.MovedRequireATB;
            this.updateFrequency = 1.0f / config.BattleSysUpdateTimesPerSec;
            this.ATBUpPerSec = config.ATBUpPerSec;
            this.win_flag = false;
        }

        IEnumerator<float> BattleCoroutine(System.Action finishedCallback = null) {
            yield return Timing.WaitForOneFrame;
            #region 更新ATB并执行操作
            while (true) {
                int memberCount = allUsableCharacter.Count;
                bool flag = false;
                for (int i = 0; i < memberCount; ++i) {
                    var character = allUsableCharacter[i];
                    bool ATBupdateFinish = false;
                    UpdateATB(character, () => ATBupdateFinish = true);
                    Debug.Log("BeforeUpdate");
                    yield return Timing.WaitUntilTrue(() => ATBupdateFinish);
                    Debug.Log("UpdateFinish");
                    flag = ValidateOver();
                    if (flag) {
                        break;
                    }
                }
                if (flag) {
                    break;
                }
                CheckBattleData();
                yield return Timing.WaitForSeconds(updateFrequency);
            }
            #endregion
            Debug.Log($"win_flag={win_flag}");
            if (this.win_flag) {
                BattleWinEvent battleWin = new BattleWinEvent(playerTeam, enemyTeam);
                foreach (var item in enemyTeam) {
                    GameObject.DestroyImmediate(item.avatar);
                }
                onBattleEnd.Invoke(GetBattleInfo(playerTeam[0]));
                eventHandler.BattleWinEventHandler(battleWin, () => { finishedCallback?.Invoke(); });
                yield break;
            }
            else {
                GameOverEvent gameOverEvent = new GameOverEvent();
                eventHandler.GameOverEventHandler(gameOverEvent);
                yield break;
            }
        }

        private void UpdateATB(IBattleCharacter character, Action updateFinishCallback = null) {
            if (character.HP <= 0) {
                updateFinishCallback?.Invoke();
                SetCharaToBeClean(character);
                return;
            }
            character.ATB += updateFrequency * (ATBUpPerSec + character.SPE * 3.0f);
            if (Mathf.Abs(character.ATB - MovedRequiredATB) <= JudgeATBPrecision) {
                onATBUpdate.Invoke(character);
                character.ThinkMove(GetBattleInfo(character), () => {character.ATB=0; updateFinishCallback?.Invoke();onATBUpdate.Invoke(character); });
            }
            else {
                updateFinishCallback?.Invoke();
                onATBUpdate.Invoke(character);
            }
        }

        /// <summary>
        /// 清除一些无法战斗的角色
        /// </summary>
        private void CheckBattleData() {
            var list = this.allUsableCharacter.FindAll(x => x.HP <= 0);
            foreach (var item in list) {
                allUsableCharacter.Remove(item);
                if (JudgePlayerEnemy[item]) {
                    playerBySpe.Remove(item);
                    playerByPos.Remove(item);
                }
                else {
                    enemyByPos.Remove(item);
                    enemyBySpe.Remove(item);
                }
            }
        }

        /// <summary>
        /// 检查战斗是否结束，返回值为true时战斗结束，返回值为false时战斗继续
        /// </summary>
        /// <returns></returns>
        private bool ValidateOver() {
            bool flag = false;
            int count = enemyTeam.Count;//先计算敌人是否全部倒下
            for (int i = 0; i < count; ++i) {
                var chara = enemyTeam[i];
                if (chara.HP > 0) {
                    flag = true;
                    break;
                }
            }
            if (!flag) {//flag为false时，敌人全灭
                win_flag = true;//win_flag赋值为true
                return true;
            }
            
            count = playerTeam.Count;
            for (int i = 0; i < count; ++i) {
                var chara = playerTeam[i];
                if (chara.HP > 0) {
                    flag = true;
                    break;
                }
            }
            return !flag;
        }


        /// <summary>
        /// 根据角色速度进行降序排序的比较函数
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static int SortBySpeed(IBattleCharacter c1, IBattleCharacter c2) {
            return -c1.SPE.CompareTo(c2.SPE);
        }

        /// <summary>
        /// 根据角色站位进行降序排序的比较函数
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static int SortByPos(IBattleCharacter c1, IBattleCharacter c2) {
            return -c1.POSITION.CompareTo(c2.POSITION);
        }

        /// <summary>
        /// 根据角色获得当前战斗的信息
        /// </summary>
        /// <param name="myself"></param>
        /// <returns></returns>
        public BattleInfo GetBattleInfo(IBattleCharacter myself) {
            if (JudgePlayerEnemy[myself]) {//玩家方获得战斗信息
                var playerPos = this.playerByPos.FindAll(x => x.HP > 0);
                var playerSpe = this.playerBySpe.FindAll(x => x.HP > 0);
                var enemyPos = this.enemyByPos.FindAll(x => x.HP > 0);
                var enemySpe = this.enemyBySpe.FindAll(x => x.HP > 0);
                BattleInfo battleInfo = new BattleInfo(playerPos, playerSpe, enemyPos, enemySpe);
                return battleInfo;
            }
            else {//玩家的对手来获得战斗信息
                var teammatePos = this.enemyByPos.FindAll(x => x.HP > 0);
                var teammateSpe = this.enemyBySpe.FindAll(x => x.HP > 0);
                var opponentPos = this.playerByPos.FindAll(x => x.HP > 0);
                var opponentSpe = this.playerBySpe.FindAll(x => x.HP > 0);
                BattleInfo battleInfo = new BattleInfo(teammatePos, teammateSpe, opponentPos, opponentSpe);
                return battleInfo;
            }
        }

        /// <summary>
        /// 角色逃跑
        /// </summary>
        /// <param name="character">要逃跑的角色</param>
        /// <param name="completeCallback">结束后的回调函数</param>
        public void CharacterEscape(IBattleCharacter character, System.Action completeCallback = null) {
            SetCharaToBeClean(character);
            var transform = character.avatar.transform;
            float pos = JudgePlayerEnemy[character] ? transform.position.x - 2.0f : transform.position.x + 2.0f;
            character.animatorController.Fade(0f, 0.55f);
            transform.DOLocalMoveX(pos, 0.55f).OnComplete(() => { completeCallback?.Invoke();Debug.Log($"Error?{completeCallback==null}");onCharacterDefeated.Invoke(character); });
        }

        public void SubstitudeAttack(SubstitudeAttackEvent eventData, System.Action completeCallback) {
            Timing.RunCoroutine(SubstitudeCoroutine(eventData, completeCallback));
        }

        private IEnumerator<float> SubstitudeCoroutine(SubstitudeAttackEvent eventData, System.Action completeCallback = null) {
            var substituder = eventData.newTarget;
            bool flag = false;
            var destination = (eventData.originalTarget.avatar.transform.position + eventData.attacker.avatar.transform.position) / 2;
            substituder.animatorController.StartWalking();
            substituder.avatar.transform.DOMove(destination, 0.1f).OnComplete(() => { flag = true; substituder.animatorController.ResetAnimation(); });
            yield return Timing.WaitUntilTrue(() => flag);
            yield return Timing.WaitForSeconds(0.2f);
            flag = false;
            eventData.attacker.animatorController.Attack(() => flag = true);
            yield return Timing.WaitUntilTrue(() => flag);
            int damage = CalculateDamage(eventData.attacker, substituder);
            CharacterHurt(ref eventData.attacker, ref substituder, damage, () => flag = false);
            yield return Timing.WaitUntilFalse(() => flag);
            yield return Timing.WaitForSeconds(0.15f);
            flag = false;
            ResetCharaPosition(substituder, 0.1f);
            ResetCharaPosition(eventData.attacker, 0.15f, completeCallback: () => { flag = true; });
            yield return Timing.WaitUntilTrue(() => flag);
            yield return Timing.WaitForSeconds(0.2f);
            completeCallback?.Invoke();
        }

        public void CharacterAttack(AttackEventData eventData, System.Action completeCallback) {
            Timing.RunCoroutine(AttackCoroutine(eventData, completeCallback));
        }

        private IEnumerator<float> AttackCoroutine(AttackEventData eventData, System.Action completeCallback = null) {
            bool flag = false;
            eventData.attacker.animatorController.Attack(() => flag = true);//角色执行攻击动画
            yield return Timing.WaitUntilTrue(() => flag);

            CharacterHurt(ref eventData.attacker, ref eventData.target, eventData.Damage, () => flag = false);//角色受击结算并播放受伤动画
            yield return Timing.WaitUntilFalse(() => flag);
            yield return Timing.WaitForSeconds(0.6f);
            ResetCharaPosition(eventData.attacker, 0.15f, completeCallback: () => { completeCallback?.Invoke(); });//重置角色位置并完成回调函数
            //yield return Timing.WaitForSeconds(0.5f);
        }

        /// <summary>
        /// 传入的角色将被标记为有待处理
        /// </summary>
        /// <param name="character"></param>
        private void SetCharaToBeClean(IBattleCharacter character) {
            character.HP = -1;
        }

        /// <summary>
        /// 角色攻击事件中去移动角色到指定位置
        /// </summary>
        /// <param name="eventData"></param>
        public void AttackCharaMove(AttackEventData eventData, System.Action completeCallback = null) {
            var target = eventData.target;
            var attackerAvatar = eventData.attacker.avatar;
            if (eventData.animationType == GameDataBase.AttackAnimationType.REMOTE) {
                completeCallback?.Invoke();
            }
            else if (eventData.animationType == GameDataBase.AttackAnimationType.CLOSE) {
                var targetTransform = eventData.target.avatar.transform;
                var targetPos = targetTransform.position;
                var attackerTransform = attackerAvatar.transform;
                if (attackerTransform.position.x < targetPos.x) {
                    targetPos.x -= 1.0f;
                }
                else {
                    targetPos.x += 1.0f;
                }
                var animatorController = eventData.attacker.animatorController;
                animatorController.StartWalking();
                attackerTransform.DOMoveX(targetPos.x, 0.25f).OnComplete(() => { animatorController.ResetAnimation(); completeCallback?.Invoke(); });
            }
        }

        /// <summary>
        /// 重置角色回它的默认位置
        /// </summary>
        /// <param name="character"></param>
        /// <param name="completeCallback"></param>
        public void ResetCharaPosition(IBattleCharacter character, float duration = 0.25f, System.Action completeCallback = null) {
            charaPosInScene.TryGetValue(character, out var targetPos);
            var avatar = character.avatar;
            var transform = avatar.transform;
            if (targetPos != transform.position) {
                var scale = transform.localScale;
                scale.x = -scale.x;
                transform.localScale = scale;
                scale.x = -scale.x;
                var animatorController = character.animatorController;
                animatorController.StartWalking();
                transform.DOMove(targetPos, duration).OnComplete(() => { transform.localScale = scale; animatorController.ResetAnimation(); completeCallback?.Invoke(); });
            }
            else {
                completeCallback?.Invoke();
            }
        }

        private void CharacterHurt(ref IBattleCharacter attacker, ref IBattleCharacter target, int damage, System.Action completeCallback = null) {
            Timing.RunCoroutine(HurtCoroutineHandler(attacker,target,damage, completeCallback));
            //target.HP -= damage;
            //if (target.HP <= 0) {//角色已死
            //    if (!JudgePlayerEnemy[target]) {//敌人角色
            //        target.animatorController.Die();
            //        target.animatorController.Fade(time:0.3f, completeCallback:() => completeCallback?.Invoke());
            //        eventHandler.outputEventInfo(new CharacterDeathEvent(target, attacker));
            //        onCharacterDefeated.Invoke(target);
            //        return;
            //    }

            //    //玩家阵营角色
            //    double fixedParam = 1.0 + (damage * 1.0 / attacker.MaxHP);
            //    if (RandomNumberGenerator.Happened(fixedParam * GameDataBase.Config.CharacterDiedProbability)) {//角色直接死亡
            //        CharacterDeathEvent deathEvent = new CharacterDeathEvent(target, attacker);
            //        eventHandler.outputEventInfo(deathEvent);
            //        //TeamInfo.RemoveCharacter((target as BattleCharacter).data);
            //        target.animatorController.Die();
            //        target.animatorController.Fade(time:0.3f, completeCallback: () => completeCallback?.Invoke());
            //        onCharacterDefeated.Invoke(target);
            //    }
            //    else {
            //        CharacterDefeatedEvent defeatedEvent = new CharacterDefeatedEvent(target, attacker);
            //        eventHandler.outputEventInfo(defeatedEvent);
            //        target.animatorController.Fade(time: 0.3f, completeCallback: () => completeCallback?.Invoke());
            //        onCharacterDefeated.Invoke(target);
            //    }
            //}
            //else {
            //    target.animatorController.Hurt(() => completeCallback?.Invoke());
            //    CharacterHurtEvent characterHurtEvent = new CharacterHurtEvent(target, damage);
            //    eventHandler.outputEventInfo(characterHurtEvent);
            //}
        }

        private IEnumerator<float> HurtCoroutineHandler(IBattleCharacter attacker,IBattleCharacter target,int damage,System.Action completeCallback=null) {
            target.HP -= damage;
            if (target.HP <= 0) {//角色已死
                if (!JudgePlayerEnemy[target]) {//敌人角色
                    target.animatorController.Die();
                    yield return Timing.WaitForSeconds(0.5f);
                    target.animatorController.Fade(time: 0.3f, completeCallback: () => completeCallback?.Invoke());
                    eventHandler.outputEventInfo(new CharacterDeathEvent(target, attacker));
                    onCharacterDefeated.Invoke(target);
                    yield break;
                }

                //玩家阵营角色
                double fixedParam = 1.0 + (damage * 1.0 / attacker.MaxHP);
                if (RandomNumberGenerator.Happened(fixedParam * GameDataBase.Config.CharacterDiedProbability)) {//角色直接死亡
                    CharacterDeathEvent deathEvent = new CharacterDeathEvent(target, attacker);
                    eventHandler.outputEventInfo(deathEvent);
                    yield return Timing.WaitForSeconds(0.5f);
                    //TeamInfo.RemoveCharacter((target as BattleCharacter).data);
                    target.animatorController.Die();
                    target.animatorController.Fade(time: 0.3f, completeCallback: () => completeCallback?.Invoke());
                    onCharacterDefeated.Invoke(target);
                }
                else {
                    CharacterDefeatedEvent defeatedEvent = new CharacterDefeatedEvent(target, attacker);
                    eventHandler.outputEventInfo(defeatedEvent);
                    target.animatorController.Fade(time: 0.3f, completeCallback: () => completeCallback?.Invoke());
                    yield return Timing.WaitForSeconds(0.8f);
                    onCharacterDefeated.Invoke(target);
                }
            }
            else {
                target.animatorController.Hurt(() => completeCallback?.Invoke());
                CharacterHurtEvent characterHurtEvent = new CharacterHurtEvent(target, damage);
                eventHandler.outputEventInfo(characterHurtEvent);
            }
            yield break;
        }

        public void CharacterCure(ref CureEvent cureEvent, System.Action completeCallback = null) {
            Timing.RunCoroutine(CharacterCureCoroutine(cureEvent, () => { completeCallback?.Invoke(); }));
        }

        private IEnumerator<float>CharacterCureCoroutine(CureEvent eventData,System.Action completeCallback = null) {
            var curer = eventData.curer;
            var target = eventData.target;
            bool flag = false;
            curer.animatorController.Attack(() => { target.HP += (int)curer.ATK;flag = true; });
            yield return Timing.WaitUntilTrue(() => flag);
            yield return Timing.WaitForSeconds(0.4f);
            completeCallback?.Invoke();
        }

        public static int CalculateDamage(IBattleCharacter attacker, IBattleCharacter target, bool considerLuck = false) {
            int baseDamage = (int)attacker.ATK - (int)target.DEF;
            baseDamage = Mathf.Clamp(baseDamage, 1, int.MaxValue);
            return baseDamage;
        }

        public bool ValidateIdentification(IBattleCharacter character) {
            return JudgePlayerEnemy.TryGetValue(character, out var ans) ? ans : false;
        }

        private Dictionary<IBattleCharacter, bool> JudgePlayerEnemy = new Dictionary<IBattleCharacter, bool>();

        private List<IBattleCharacter> allUsableCharacter = new List<IBattleCharacter>();

        private List<IBattleCharacter> playerTeam = new List<IBattleCharacter>();//玩家方可用角色

        private List<IBattleCharacter> enemyTeam = new List<IBattleCharacter>();//敌方可用角色

        private List<IBattleCharacter> playerByPos;

        private List<IBattleCharacter> enemyByPos;

        private List<IBattleCharacter> playerBySpe;

        private List<IBattleCharacter> enemyBySpe;

        private Dictionary<IBattleCharacter, Vector3> charaPosInScene = new Dictionary<IBattleCharacter, Vector3>();

        private float MovedRequiredATB;

        private float updateFrequency;

        private float ATBUpPerSec;

        private bool win_flag = false;

        private const float JudgeATBPrecision = 0.002f;

        private CoroutineHandle handle;

        private EventHandler eventHandler;

        private List<Vector3> PlayerCharaPosition = new List<Vector3>();
        private List<Vector3> EnemyCharaPosition = new List<Vector3>();

        public UnityEvent<BattleInfo> onBattleStart;

        public UnityEvent<BattleInfo> onBattleEnd;

        public UnityEvent<IBattleCharacter> onATBUpdate;

        public UnityEvent<IBattleCharacter> onCharacterDefeated;
    }
}