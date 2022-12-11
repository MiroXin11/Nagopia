using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    /// <summary>
    /// 有两个字段，要加经验的角色与增加的经验
    /// 如果加经验的角色为空，则默认为全队加经验
    /// </summary>
    public class ExpGainedEvent : BaseEvent {
        public ExpGainedEvent() {
            this.character= null;
            this.exp = 0;
            this.eventType = GameDataBase.EventType.EXP_GAINED;
        }
        public ExpGainedEvent(int exp) {
            this.exp = exp;
            this.character= null;
            this.eventType = GameDataBase.EventType.EXP_GAINED;
        }
        public ExpGainedEvent(CharacterData character,int exp) {
            this.exp = exp;
            this.character= character;
            this.eventType = GameDataBase.EventType.EXP_GAINED;
        }
        public ExpGainedEvent(IBattleCharacter character,int exp) {
            if(character is BattleCharacter) {
                var temp = character as BattleCharacter;
                this.character = temp.data;
            }
            else {
                this.character = null;
            }
            this.exp = exp;
            this.eventType = GameDataBase.EventType.EXP_GAINED;
        }
        public CharacterData character;

        public int exp;

        public override string ToString() {
            if (this.character == null) {
                return $"玩家队伍获得了经验{exp}点";
            }
            return $"{character.name}获得了经验{exp}点";
        }
    }
}