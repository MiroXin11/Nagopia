using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class CharacterDeathEvent : BaseEvent {

        public CharacterDeathEvent(IBattleCharacter victim,IBattleCharacter killer) {
            this.victim = victim;
            this.killer = killer;
        }

        IBattleCharacter victim;

        IBattleCharacter killer;

        public override string ToString() {
            return $"{victim.Name} was killed by {killer.Name}";
        }
    }
}