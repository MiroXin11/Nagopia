using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class CharacterHurtEvent : BaseEvent {
        public CharacterHurtEvent(IBattleCharacter victim,int damage) {
            this.victim = victim;
            this.damage = damage;
            this.eventType = GameDataBase.EventType.CHARACTER_HURT;
        }
        public IBattleCharacter victim;
        public int damage;

        public override string ToString() {
            return $"{victim.Name}受到{damage}点伤害,剩余血量{victim.HP}";
        }
    }
}