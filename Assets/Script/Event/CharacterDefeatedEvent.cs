using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class CharacterDefeatedEvent:BaseEvent {

        public CharacterDefeatedEvent(IBattleCharacter victim,IBattleCharacter attcker) {
            this.victim = victim;
            this.attacker= attcker;
            this.eventType = GameDataBase.EventType.CHARACTER_DEFEATED;
        }

        public IBattleCharacter victim;

        public IBattleCharacter attacker;

        public override string ToString() {
            return $"{victim.Name}±»{attacker.Name}»÷µ¹ÁË";
        }
    }
}