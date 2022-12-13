using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class SubstitudeAttackEvent:BaseEvent{
        public SubstitudeAttackEvent(IBattleCharacter attacker,IBattleCharacter newTarget,IBattleCharacter orignal) {
            this.attacker = attacker;
            this.originalTarget = orignal;
            this.newTarget = newTarget;
            this.eventType = GameDataBase.EventType.CHARACTER_SUBSITITUDE;
        }

        public IBattleCharacter attacker;

        public IBattleCharacter newTarget;

        public IBattleCharacter originalTarget;

        public override string ToString() {
            return $"{attacker.Name}���Թ���{originalTarget.Name},��{newTarget.Name}������{originalTarget.Name}";
        }
    }
}

