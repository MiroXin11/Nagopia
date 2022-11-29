using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class SubstitudeAttackEvent:BaseEvent{
        public SubstitudeAttackEvent(IBattleCharacter attacker,IBattleCharacter newTarget,IBattleCharacter orignal) {
            this.attacker = attacker;
            this.originalTarget = orignal;
            this.newTarget = newTarget;
        }

        public IBattleCharacter attacker;

        public IBattleCharacter newTarget;

        public IBattleCharacter originalTarget;

        public override string ToString() {
            return $"{attacker.Name} tried to kill {originalTarget.Name},but {newTarget.Name} guarded {originalTarget.Name}";
        }
    }
}

