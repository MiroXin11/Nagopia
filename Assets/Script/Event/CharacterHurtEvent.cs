using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class CharacterHurtEvent : BaseEvent {
        public CharacterHurtEvent(IBattleCharacter victim,int damage) {
            this.victim = victim;
            this.damage = damage;
        }
        public IBattleCharacter victim;
        public int damage;

        public override string ToString() {
            return $"{victim.Name} lost HP {damage},current HP is {victim.HP}";
        }
    }
}