using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class CharacterDeathEvent : BaseEvent {

        public CharacterDeathEvent(IBattleCharacter victim,IBattleCharacter killer) {
            this.victim = victim;
            this.killer = killer;
            this.eventType = GameDataBase.EventType.CHARACTER_DIED;
        }

        public IBattleCharacter victim;

        public IBattleCharacter killer;

        public override string ToString() {
            return $"{victim.Name}��{killer.Name}ɱ����";
        }
    }
}