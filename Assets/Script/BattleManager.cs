using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System;

namespace Nagopia {
    public class BattleManager : SingletonMonobehaviour<BattleManager> {
        public void Awake() {
            DontDestroyOnLoad(this);
        }

        public void StartBattle(List<IBattleCharacter>player,List<IBattleCharacter>enemy) {
            if (handle.IsRunning) {
                Timing.KillCoroutines(handle);
            }
            UpdateBattleConfig();
            this.allUsableCharacter.Clear();
            this.JudgePlayerEnemy.Clear();

            this.playerTeam = player;
            this.enemyTeam = enemy;

            int count = player.Count;
            for(int i = 0; i < count; ++i) {
                IBattleCharacter character = player[i];
                JudgePlayerEnemy.Add(character, true);
                if(character.HP>0)
                    allUsableCharacter.Add(character);
            }
            count = enemy.Count;
            for(int i = 0; i < count; ++i) {
                IBattleCharacter character = enemy[i];
                JudgePlayerEnemy.Add(character, false);
                if (character.HP > 0)//其实没啥意义，不过严谨点
                    allUsableCharacter.Add(character);
            }

            allUsableCharacter.Sort(SortBySpeed);
            foreach (var character in allUsableCharacter) {//给一个起步值，让战斗节奏更快开始
                character.ATB = character.SPE * 10+100;
            }

            handle=Timing.RunCoroutine(BattleCoroutine());
        }

        private void UpdateBattleConfig() {
            GameConfig config = GameDataBase.Config;
            this.MovedRequiredATB = config.MovedRequireATB;
            this.updateFrequency = 1.0f / config.BattleSysUpdateTimesPerSec;
            this.ATBUpPerSec = config.ATBUpPerSec;
        }

        IEnumerator<float> BattleCoroutine() {
            #region 更新ATB并执行操作
            while (true) {
                int memberCount = allUsableCharacter.Count;
                bool flag = false;
                for(int i = 0; i < memberCount; ++i) {
                    var character = allUsableCharacter[i];
                    UpdateATB(ref character);
                    flag = ValidateOver();
                }
                if (flag) {
                    break;
                }
                allUsableCharacter.RemoveAll((item) => item.HP <= 0);
                yield return Timing.WaitForSeconds(updateFrequency);
            }
            #endregion
            if (win_flag) {

            }
            yield return Timing.WaitForOneFrame;
        }

        private void UpdateATB(ref IBattleCharacter character) {
            if (character.HP < 0) {
                return;
            }
            character.ATB += character.SPE * 10 +updateFrequency*ATBUpPerSec;
            //character.ATB += System.Convert.ToInt32(character.ATB*10*GameDataBase.Config.GameSpeed);
            if (Mathf.Abs(character.ATB - MovedRequiredATB) <= JudgeATBPrecision) {

            }
        }

        /// <summary>
        /// 检查战斗是否结束，返回值为true时战斗结束，返回值为false时战斗继续
        /// </summary>
        /// <returns></returns>
        private bool ValidateOver() {
            bool flag = false;
            int count = enemyTeam.Count;//理论上还是玩家赢面大嘛，就先算敌人的
            for(int i = 0; i < count; ++i) {
                var chara = enemyTeam[i];
                if (chara.HP > 0) {
                    flag = true;
                    break;
                }
            }
            if (flag) {//flag为真时，敌人全灭
                win_flag = flag;//win_flag赋值为true
                return flag;
            }

            count = playerTeam.Count;
            for(int i = 0; i < count; ++i) {
                var chara = playerTeam[i];
                if (chara.HP > 0) {
                    flag = true;
                    break;
                }
            }
            //这里之所以不检查flag，因为flag为true时，代表玩家输，此时win_flag为false表示玩家输。flag为false时也不更新win_flag
            return flag;
        }

        /// <summary>
        /// 根据角色速度进行降序排序的比较函数
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static int SortBySpeed(IBattleCharacter c1,IBattleCharacter c2) {
            return -c1.SPE.CompareTo(c2.SPE);
        }

        private Dictionary<IBattleCharacter, bool> JudgePlayerEnemy = new Dictionary<IBattleCharacter, bool>();

        private List<IBattleCharacter> allUsableCharacter = new List<IBattleCharacter>();

        private List<IBattleCharacter> playerTeam = new List<IBattleCharacter>();

        private List<IBattleCharacter> enemyTeam = new List<IBattleCharacter>();

        private float MovedRequiredATB = GameDataBase.Config.MovedRequireATB;

        private float updateFrequency = 1.0f / GameDataBase.Config.BattleSysUpdateTimesPerSec;

        private float ATBUpPerSec = GameDataBase.Config.ATBUpPerSec;

        private bool win_flag=false;

        private const float JudgeATBPrecision = 0.002f;

        private CoroutineHandle handle;
    }
}