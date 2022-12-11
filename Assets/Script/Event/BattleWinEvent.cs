using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class BattleWinEvent : BaseEvent {
        public BattleWinEvent() {

        }

        public BattleWinEvent(List<IBattleCharacter> players, List<IBattleCharacter> enemies) {
            this.players = players;
            this.enemies = enemies;
        }

        public List<IBattleCharacter> players;

        public List<IBattleCharacter>enemies;
    }
}