using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class WizardAnimationControl : CharacterAnimatorController {
        public override void Attack(Action onCompleCallback = null) {
            this.animator.Play("Casting Spells");
            this.AttackEndCallback = onCompleCallback;
        }
    }
}