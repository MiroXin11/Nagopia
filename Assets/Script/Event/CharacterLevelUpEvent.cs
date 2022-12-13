using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class CharacterLevelUpEvent :BaseEvent{
        public CharacterLevelUpEvent() {
            this.eventType = GameDataBase.EventType.CHARACTER_LEVELUP;
        }

        public CharacterLevelUpEvent(CharacterData character,ref int originalLevel) {
            this.character = character;
            this.eventType = GameDataBase.EventType.CHARACTER_LEVELUP;
            this.originalLevel= originalLevel;
        }

        public int originalLevel = 0;

        public CharacterData character;

        public override string ToString() {
            return $"{character.name} 的等级从{originalLevel}级提升到了{character.Level}级";
        }
    }
}