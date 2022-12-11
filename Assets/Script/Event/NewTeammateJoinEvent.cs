using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class NewTeammateJoinEvent : BaseEvent {
        public NewTeammateJoinEvent() {
            this.eventType = GameDataBase.EventType.NEW_TEAMMATE_JOIN;
        }
        public NewTeammateJoinEvent(CharacterData character) {
            this.character = character;
            this.eventType = GameDataBase.EventType.NEW_TEAMMATE_JOIN;
        }

        public CharacterData character;

        public override string ToString() {
            return $"{character.name} 加入队伍!";
        }
    }
}