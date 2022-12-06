using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class BattleStartEvent : BaseEvent {

        public BattleStartEvent(List<IBattleCharacter> playerTeam, List<IBattleCharacter> enemyTeam) {
            this.PlayerTeam = playerTeam;
            this.EnemyTeam = enemyTeam;
        }

        public List<IBattleCharacter> PlayerTeam;

        public List<IBattleCharacter> EnemyTeam;

        public override string ToString() {
            return $"战斗开始";
        }
    }
}