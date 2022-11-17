using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class WarriorAnimationControl : CharacterAnimatorController {
        public override void Attack(Action onCompleCallback = null) {
            animator.Play("Slashing");
            this.AttackEndCallback= onCompleCallback;
        }
    }
}