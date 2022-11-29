using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class CharacterDefeatedEvent:BaseEvent {

        public CharacterDefeatedEvent(IBattleCharacter victim,IBattleCharacter attcker) {
            this.victim = victim;
            this.attacker= attcker;
        }

        public IBattleCharacter victim;

        public IBattleCharacter attacker;

        public override string ToString() {
            return $"{victim.Name} was defeated by {attacker.Name}";
        }
    }
}