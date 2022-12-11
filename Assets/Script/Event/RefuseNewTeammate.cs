using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class RefuseNewTeammate : BaseEvent {
        public RefuseNewTeammate(CharacterData data) {
            this.character= data;
            this.eventType = GameDataBase.EventType.NEW_TEAMMATE_REFUSE;
        }
        public CharacterData character;
        public override string ToString() {
            return $"拒绝了{character.name}的加入";
        }
    }
}