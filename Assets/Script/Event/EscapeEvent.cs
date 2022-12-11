using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class EscapeEvent : BaseEvent {
        public EscapeEvent(IBattleCharacter character) {
            this.character = character;
            this.eventType = GameDataBase.EventType.CHARACTER_ESCAPE;
        }

        public override string ToString() {
            return $"{character.Name} escape!";
        }
        public IBattleCharacter character;
    }
}