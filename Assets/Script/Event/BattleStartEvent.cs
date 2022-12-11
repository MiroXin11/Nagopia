using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class BattleStartEvent : BaseEvent {

        public BattleStartEvent(List<IBattleCharacter> playerTeam, List<IBattleCharacter> enemyTeam) {
            this.PlayerTeam = playerTeam;
            this.EnemyTeam = enemyTeam;
            this.eventType = GameDataBase.EventType.BATTLE_START;
        }

        public List<IBattleCharacter> PlayerTeam;

        public List<IBattleCharacter> EnemyTeam;

        public override string ToString() {
            return $"战斗开始";
        }

        /// <summary>
        /// 根据关卡进度去生成对战
        /// </summary>
        /// <returns></returns>
        public static BattleStartEvent GenerateRandomBattleEvent() {
            var stage = GameDataBase.GameStage;
            var difficulty = stage / 10 + 1;
            //根据当前阶段生成敌人
            var template = GameDataBase.GetEnemyTeamTemplate(difficulty);
            var playerTeamData = TeamInfo.CharacterDatas;
            List<IBattleCharacter> playerTeam = new List<IBattleCharacter>();
            List<IBattleCharacter> enemyTeam = new List<IBattleCharacter>();
            foreach (var item in playerTeamData) {
                if (item.CurrentHP > 0) {
                    BattleCharacter bc = new BattleCharacter(item, playerTeam, enemyTeam);
                    playerTeam.Add(bc);
                }
            }
            var enemyTeamTemplate = template.team;
            foreach (var item in enemyTeamTemplate) {
                EnemyData enemyData = new EnemyData(item, stage);
                EnemyBattleCharacter eBC = new EnemyBattleCharacter(enemyData, enemyTeam, playerTeam);
                enemyTeam.Add(eBC);
            }
            BattleStartEvent battleStartEvent = new BattleStartEvent(playerTeam, enemyTeam);
            return battleStartEvent;
        }

        public static BattleStartEvent GenerateBossFightEvent() {
            var stage = GameDataBase.GameStage;
            var difficulty = stage / 10 + 1;
            var template = GameDataBase.GetEnemyTeamTemplate(difficulty, 5);
            var playerTeamData = TeamInfo.CharacterDatas;
            List<IBattleCharacter> playerTeam = new List<IBattleCharacter>();
            List<IBattleCharacter> enemyTeam = new List<IBattleCharacter>();
            foreach (var item in playerTeamData) {
                if (item.CurrentHP > 0) {
                    BattleCharacter bc = new BattleCharacter(item, playerTeam, enemyTeam);
                    playerTeam.Add(bc);
                }
            }
            var enemyTeamTemplate = template.team;
            foreach (var item in enemyTeamTemplate) {
                EnemyData enemyData = new EnemyData(item, stage);
                EnemyBattleCharacter eBC = new EnemyBattleCharacter(enemyData, enemyTeam, playerTeam);
                enemyTeam.Add(eBC);
            }
            BattleStartEvent battleStartEvent = new BattleStartEvent(playerTeam, enemyTeam);
            return battleStartEvent;
        }
    }
}