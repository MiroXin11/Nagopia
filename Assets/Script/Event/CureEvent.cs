using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class CureEvent : BaseEvent {
        public IBattleCharacter curer;

        public IBattleCharacter target;

        public int HPResotred;

        public CureEvent(IBattleCharacter curer,IBattleCharacter target,int HP) {
            this.curer = curer;
            this.target= target;
            this.HPResotred = HP;
        }

        public override string ToString() {
            return $"{curer.Name} 治疗 {target.Name},{target}当前血量是{target.HP}";
        }
    }
}

